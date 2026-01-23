#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEditor;
using UnityEditor.ShortcutManagement;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
using UnityEditor.IMGUI.Controls;
using Type = System.Type;
using static VHierarchy.Libs.VUtils;
using static VHierarchy.Libs.VGUI;
// using static VTools.VDebug;
using static VHierarchy.VHierarchy;
using static VHierarchy.VHierarchyData;
using static VHierarchy.VHierarchyCache;

#if UNITY_6000_3_OR_NEWER
using TreeViewItem = UnityEditor.IMGUI.Controls.TreeViewItem<UnityEngine.EntityId>;
using TreeViewState = UnityEditor.IMGUI.Controls.TreeViewState<UnityEngine.EntityId>;
#elif UNITY_6000_2_OR_NEWER
using TreeViewItem = UnityEditor.IMGUI.Controls.TreeViewItem<int>;
using TreeViewState = UnityEditor.IMGUI.Controls.TreeViewState<int>;
#endif




namespace VHierarchy
{
    public class VHierarchyController
    {

        public void UpdateExpandQueue()
        {
            if (treeViewAnimatesExpansion) return;

            if (!expandQueue_toAnimate.Any())
            {
                if (!expandQueue_toCollapseAfterAnimation.Any()) return;

                foreach (var r in expandQueue_toCollapseAfterAnimation)
                    SetExpanded_withoutAnimation(r, false);

                expandQueue_toCollapseAfterAnimation.Clear();

                return;

            }


            var id = expandQueue_toAnimate.First().id;
            var expand = expandQueue_toAnimate.First().expand;



            var itemIndex = treeViewControllerData.InvokeMethod<int>("GetRow", id.ToIdType());
            var items = treeViewControllerData.GetMemberValue<List<TreeViewItem>>("m_Rows");

            var stuckCollapsing = itemIndex != -1 && items[itemIndex].id != id; // happens when collapsing long hierarchies due to a bug in TreeViewController

            if (stuckCollapsing) { window.SendEvent(new Event() { type = EventType.KeyDown, keyCode = KeyCode.None }); return; }




            if (expandedIds.Contains(id) != expand)
                SetExpanded_withAnimation(id, expand);

            expandQueue_toAnimate.RemoveAt(0);


            window.Repaint();


            // must be called from gui because reflected methods rely on Event.current 

        }

        public List<ExpandQueueEntry> expandQueue_toAnimate = new();
        public List<int> expandQueue_toCollapseAfterAnimation = new();

        public struct ExpandQueueEntry { public int id; public bool expand; }

        public bool animatingExpansion => expandQueue_toAnimate.Any() || expandQueue_toCollapseAfterAnimation.Any();






        public void UpdateScrollAnimation()
        {
            if (!animatingScroll) return;


            var lerpSpeed = 10;

            var lerpedScrollPos = MathUtil.SmoothDamp(currentScrollPos, targetScrollPos, lerpSpeed, ref scrollPosDerivative, editorDeltaTime);

            SetScrollPos(lerpedScrollPos);

            window.Repaint();



            if (lerpedScrollPos.DistanceTo(targetScrollPos) > .4f) return;

            SetScrollPos(targetScrollPos);

            animatingScroll = false;


        }

        public float targetScrollPos;
        public float scrollPosDerivative;

        public bool animatingScroll;







        public void UpdateHighlightAnimation()
        {
            if (!animatingHighlight) return;


            var lerpSpeed = 1.3f;

            MathUtil.SmoothDamp(ref highlightAmount, 0, lerpSpeed, ref highlightDerivative, editorDeltaTime);

            window.Repaint();



            if (highlightAmount > .05f) return;

            highlightAmount = 0;

            animatingHighlight = false;


        }

        public float highlightAmount;
        public float highlightDerivative;

        public bool animatingHighlight;

        public GameObject objectToHighlight;







        public void UpdateState()
        {
            var sceneHierarchy = window?.GetFieldValue("m_SceneHierarchy");

            treeViewController = sceneHierarchy.GetFieldValue("m_TreeView");
            treeViewControllerData = treeViewController.GetMemberValue("data");



            var treeViewControllerState = treeViewController?.GetPropertyValue<TreeViewState>("state");

            currentScrollPos = treeViewControllerState?.scrollPos.y ?? 0;

            expandedIds = treeViewControllerState?.expandedIDs.ToInts() ?? new();



            var treeViewAnimator = treeViewController?.GetMemberValue("m_ExpansionAnimator");
            var treeViewAnimatorSetup = treeViewAnimator?.GetMemberValue("m_Setup");

            treeViewAnimatesScroll = treeViewController?.GetMemberValue<UnityEditor.AnimatedValues.AnimFloat>("m_FramingAnimFloat").isAnimating ?? false;

            treeViewAnimatesExpansion = treeViewAnimator?.GetMemberValue<bool>("isAnimating") ?? false;

        }

        object treeViewController;
        object treeViewControllerData;

        public float currentScrollPos;

        public List<int> expandedIds = new();

        public bool treeViewAnimatesScroll;
        public bool treeViewAnimatesExpansion;

        public int GetRowIndex(int instanceId)
        {
            return treeViewControllerData.InvokeMethod<int>("GetRow", instanceId.ToIdType());
        }
















        public void ToggleExpanded(int id)
        {
            SetExpanded_withAnimation(id, !expandedIds.Contains(id));

            window.Repaint();

        }

        public void CollapseAll()
        {
            var expandedRoots = new List<GameObject>();
            var expandedChildren = new List<GameObject>();

            foreach (var iid in expandedIds)
                if (_EditorUtility_InstanceIDToObject(iid) is GameObject expandedGo)
                    if (expandedGo.transform.parent)
                        expandedChildren.Add(expandedGo);
                    else
                        expandedRoots.Add(expandedGo);



            expandQueue_toCollapseAfterAnimation = expandedChildren.Select(r => r.GetInstanceID()).ToList();

            expandQueue_toAnimate = expandedRoots.Select(r => new ExpandQueueEntry { id = r.GetInstanceID(), expand = false })
                                                .OrderBy(r => GetRowIndex(r.id)).ToList();

            StartScrollAnimation(targetScrollPos: 0);


            window.Repaint();

        }

        public void Isolate(int targetId)
        {

            List<int> getParents(int id)
            {
                var parentIds = new List<int>();

                if (_EditorUtility_InstanceIDToObject(id) is not GameObject go) return parentIds;


                while (go.transform.parent)
                    parentIds.Add((go = go.transform.parent.gameObject).GetInstanceID());

                parentIds.Add(go.scene.handle);


                return parentIds;

            }

            var targetItemParents = getParents(targetId);



            var itemsToCollapse = expandedIds.ToList();

            itemsToCollapse.Remove(targetId);
            itemsToCollapse.RemoveAll(r => targetItemParents.Contains(r));
            itemsToCollapse.RemoveAll(r => itemsToCollapse.Intersect(getParents(r)).Any());

            if (_EditorUtility_InstanceIDToObject(targetId) is GameObject)
                itemsToCollapse.RemoveAll(r => _EditorUtility_InstanceIDToObject(r) is not GameObject); // won't collapse scenes




            expandQueue_toAnimate = itemsToCollapse.Select(id => new ExpandQueueEntry { id = id, expand = false })
                                                               .Append(new ExpandQueueEntry { id = targetId, expand = true })
                                                               .OrderBy(r => GetRowIndex(r.id)).ToList();


            window.Repaint();

        }





        public void StartExpandAnimation(List<int> targetExpandedIds)
        {

            var toExpand = targetExpandedIds.Except(expandedIds).ToHashSet();
            var toCollapse = expandedIds.Except(targetExpandedIds).ToHashSet();




            // hanlde destroyed objects

            var sceneIds = Enumerable.Range(0, EditorSceneManager.sceneCount).Select(i => EditorSceneManager.GetSceneAt(i).handle).ToHashSet();

            var toExpand_destroyed = toExpand.Where(id => !sceneIds.Contains(id) && _EditorUtility_InstanceIDToObject(id) as GameObject == null).ToHashSet();
            var toCollapse_destroyed = toCollapse.Where(id => !sceneIds.Contains(id) && _EditorUtility_InstanceIDToObject(id) as GameObject == null).ToHashSet();


            foreach (var id in toExpand_destroyed)
                expandedIds.Add(id);

            foreach (var id in toCollapse_destroyed)
                expandedIds.Remove(id);


            toExpand.ExceptWith(toExpand_destroyed);
            toCollapse.ExceptWith(toCollapse_destroyed);






            // hanlde non-animated expansions/collapses

            bool hasParentToCollapse(int id)
            {
                var go = _EditorUtility_InstanceIDToObject(id) as GameObject;

                if (!go) return false;
                if (!go.transform.parent) return false;


                var parentId = go.transform.parent.gameObject.GetInstanceID();

                return toCollapse.Contains(parentId)
                    || hasParentToCollapse(parentId);

            }
            bool areAllParentsExpanded(int id)
            {
                var go = _EditorUtility_InstanceIDToObject(id) as GameObject;

                if (!go) return true;
                if (!go.transform.parent) return true;


                var parentId = go.transform.parent.gameObject.GetInstanceID();

                return expandedIds.Contains(parentId)
                         && areAllParentsExpanded(parentId);

            }

            var toExpand_beforeAnimation = toExpand.Where(id => !areAllParentsExpanded(id)).ToHashSet();
            var toCollapse_afterAnimation = toCollapse.Where(id => hasParentToCollapse(id)).ToHashSet();


            foreach (var id in toExpand_beforeAnimation)
                SetExpanded_withoutAnimation(id, true);

            foreach (var id in toCollapse_afterAnimation)
                expandQueue_toCollapseAfterAnimation.Add(id);


            toExpand.ExceptWith(toExpand_beforeAnimation);
            toCollapse.ExceptWith(toCollapse_afterAnimation);






            // setup animation

            expandQueue_toAnimate = toCollapse.Select(id => new ExpandQueueEntry { id = id, expand = false })
                                .Concat(toExpand.Select(id => new ExpandQueueEntry { id = id, expand = true }))
                                .OrderBy(r => GetRowIndex(r.id)).ToList();

        }

        public void SetExpandedIds(List<int> targetExpandedIds)
        {
            treeViewControllerData.InvokeMethod("SetExpandedIDs", targetExpandedIds.ToArray()); // won't work on 6.3 but it's unused anyway
        }
        public void SetExpanded_withAnimation(int instanceId, bool expanded)
        {
            treeViewController.InvokeMethod("ChangeFoldingForSingleItem", instanceId.ToIdType(), expanded);
        }
        public void SetExpanded_withoutAnimation(int instanceId, bool expanded)
        {
            treeViewControllerData.InvokeMethod("SetExpanded", instanceId.ToIdType(), expanded);
        }



        public void StartScrollAnimation(float targetScrollPos)
        {
            if (targetScrollPos.DistanceTo(currentScrollPos) < .05f) return;

            this.targetScrollPos = targetScrollPos;

            animatingScroll = true;

        }

        public void SetScrollPos(float targetScrollPos)
        {
            window.GetMemberValue("m_SceneHierarchy").GetMemberValue<TreeViewState>("m_TreeViewState").scrollPos = Vector2.up * targetScrollPos;
        }



        public void RevealObject(GameObject go, bool expand, bool highlight, bool snapToTopMargin)
        {

            var idsToExpand = new List<int>();

            if (expand && go.transform.childCount > 0)
                idsToExpand.Add(go.GetInstanceID());

            var cur = go.transform;
            while (cur = cur.parent)
                idsToExpand.Add(cur.gameObject.GetInstanceID());

            idsToExpand.Add(go.scene.handle);

            idsToExpand.RemoveAll(r => expandedIds.Contains(r));




            foreach (var id in idsToExpand.SkipLast(1))
                SetExpanded_withoutAnimation(id, true);

            if (idsToExpand.Any())
                SetExpanded_withAnimation(idsToExpand.Last(), true);




            var rowCount = treeViewControllerData.GetMemberValue<ICollection>("m_Rows").Count;
            var maxScrollPos = rowCount * 16 - window.position.height + 26.9f;

            var rowIndex = treeViewControllerData.InvokeMethod<int>("GetRow", go.GetInstanceID().ToIdType());
            var rowPos = rowIndex * 16f + 8;

            var scrollAreaHeight = window.GetMemberValue<Rect>("treeViewRect").height;




            var margin = 48;

            var targetScrollPos = 0f;

            if (expand)
                targetScrollPos = (rowPos - margin).Min(maxScrollPos)
                                                   .Max(0);
            else
                targetScrollPos = currentScrollPos.Min(rowPos - margin)
                                                  .Max(rowPos - scrollAreaHeight + margin)
                                                  .Min(maxScrollPos)
                                                  .Max(0);
            if (targetScrollPos < 25)
                targetScrollPos = 0;



            StartScrollAnimation(targetScrollPos);




            if (!highlight) return;

            highlightAmount = 2.2f;

            animatingHighlight = true;

            objectToHighlight = go;


        }














        public VHierarchyController(EditorWindow window) => this.window = window;

        public EditorWindow window;

        public VHierarchyGUI gui => VHierarchy.guis_byWindow[window];

    }
}
#endif