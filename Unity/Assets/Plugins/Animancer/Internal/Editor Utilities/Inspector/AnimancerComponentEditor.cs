// Animancer // Copyright 2020 Kybernetik //

#if UNITY_EDITOR

using System.Text;
using UnityEditor;
using UnityEngine;

namespace Animancer.Editor
{
    /// <summary>[Editor-Only]
    /// A custom Inspector for <see cref="AnimancerComponent"/>s.
    /// </summary>
    [CustomEditor(typeof(AnimancerComponent), true), CanEditMultipleObjects]
    public class AnimancerComponentEditor : BaseAnimancerComponentEditor
    {
        /************************************************************************************************************************/

        private bool _ShowResetOnDisableWarning;

        /// <summary>[Editor-Only]
        /// Draws any custom GUI for the `property`.
        /// The return value indicates whether the GUI should replace the regular call to
        /// <see cref="EditorGUILayout.PropertyField(SerializedProperty, GUIContent, bool, GUILayoutOption[])"/> or
        /// not. True = GUI was drawn, so don't draw the regular GUI. False = Draw the regular GUI.
        /// </summary>
        protected override bool DoOverridePropertyGUI(string path, SerializedProperty property, GUIContent label)
        {
            if (path == "_ActionOnDisable")
            {
                EditorGUILayout.PropertyField(property, label, true);
                if (property.enumValueIndex == (int)AnimancerComponent.DisableAction.Reset)
                {
                    // Since getting all the components creates garbage, only do it during layout events.
                    if (Event.current.type == EventType.Layout)
                    {
                        _ShowResetOnDisableWarning = !AreAllResettingTargetsAboveTheirAnimator();
                    }

                    if (_ShowResetOnDisableWarning)
                    {
                        EditorGUILayout.HelpBox("Reset only works if this component is above the Animator" +
                            " so OnDisable can perform the Reset before the Animator actually gets disabled." +
                            " Click here to fix." +
                            "\n\nOtherwise you can use Stop and call Animator.Rebind before disabling this GameObject.",
                            MessageType.Error);

                        if (AnimancerGUI.TryUseClickEventInLastRect())
                            MoveResettingTargetsAboveTheirAnimator();
                    }
                }
                return true;
            }

            return base.DoOverridePropertyGUI(path, property, label);
        }

        /************************************************************************************************************************/

        private bool AreAllResettingTargetsAboveTheirAnimator()
        {
            for (int i = 0; i < Targets.Length; i++)
            {
                var target = Targets[i];
                if (!target.ResetOnDisable)
                    continue;

                var animator = target.Animator;
                if (animator == null ||
                    target.gameObject != animator.gameObject)
                    continue;

                var targetObject = (Object)target;
                var components = target.gameObject.GetComponents<Component>();
                for (int j = 0; j < components.Length; j++)
                {
                    var component = components[j];
                    if (component == targetObject)
                        break;
                    else if (component == animator)
                        return false;
                }
            }

            return true;
        }

        /************************************************************************************************************************/

        private void MoveResettingTargetsAboveTheirAnimator()
        {
            for (int i = 0; i < Targets.Length; i++)
            {
                var target = Targets[i];
                if (!target.ResetOnDisable)
                    continue;

                var animator = target.Animator;
                if (animator == null ||
                    target.gameObject != animator.gameObject)
                    continue;

                int animatorIndex = -1;

                var targetObject = (Object)target;
                var components = target.gameObject.GetComponents<Component>();
                for (int j = 0; j < components.Length; j++)
                {
                    var component = components[j];
                    if (component == targetObject)
                    {
                        if (animatorIndex >= 0)
                        {
                            var count = j - animatorIndex;
                            while (count-- > 0)
                                UnityEditorInternal.ComponentUtility.MoveComponentUp((Component)target);
                        }
                        break;
                    }
                    else if (component == animator)
                    {
                        animatorIndex = j;
                    }
                }
            }
        }

        /************************************************************************************************************************/

        /// <summary>The priority of all context menu items added by this class.</summary>
        public const int MenuItemPriority = 2000;

        /// <summary>The start of all <see cref="AnimancerComponent"/> context menu items.</summary>
        public const string MenuItemPrefix = "CONTEXT/AnimancerComponent/";

        /************************************************************************************************************************/

        /// <summary>Returns <see cref="AnimancerPlayable.IsGraphPlaying"/>.</summary>
        [MenuItem(MenuItemPrefix + "Pause Graph", validate = true)]
        private static bool ValidatePauseGraph(MenuCommand command)
        {
            var animancer = command.context as AnimancerComponent;
            return
                animancer.IsPlayableInitialised &&
                animancer.Playable.IsGraphPlaying;
        }

        /// <summary>Calls <see cref="AnimancerPlayable.PauseGraph()"/>.</summary>
        [MenuItem(MenuItemPrefix + "Pause Graph", priority = MenuItemPriority)]
        private static void PauseGraph(MenuCommand command)
        {
            var animancer = command.context as AnimancerComponent;
            animancer.Playable.PauseGraph();
        }

        /************************************************************************************************************************/

        /// <summary>Returns !<see cref="AnimancerPlayable.IsGraphPlaying"/>.</summary>
        [MenuItem(MenuItemPrefix + "Unpause Graph", validate = true)]
        private static bool ValidatePlayGraph(MenuCommand command)
        {
            var animancer = command.context as AnimancerComponent;
            return
                animancer.IsPlayableInitialised &&
                !animancer.Playable.IsGraphPlaying;
        }

        /// <summary>Calls <see cref="AnimancerPlayable.UnpauseGraph()"/>.</summary>
        [MenuItem(MenuItemPrefix + "Unpause Graph", priority = MenuItemPriority)]
        private static void PlayGraph(MenuCommand command)
        {
            var animancer = command.context as AnimancerComponent;
            animancer.Playable.UnpauseGraph();
            animancer.Evaluate();
        }

        /************************************************************************************************************************/

        /// <summary>
        /// Returns <see cref="AnimancerComponent.IsPlayableInitialised"/>.
        /// </summary>
        [MenuItem(MenuItemPrefix + "Stop All Animations", validate = true)]
        private static bool ValidateStop(MenuCommand command)
        {
            var animancer = command.context as AnimancerComponent;
            return animancer.IsPlayableInitialised;
        }

        /// <summary>Calls <see cref="AnimancerComponent.Stop()"/>.</summary>
        [MenuItem(MenuItemPrefix + "Stop All Animations", priority = MenuItemPriority)]
        private static void Stop(MenuCommand command)
        {
            var animancer = command.context as AnimancerComponent;
            animancer.Stop();
            animancer.Evaluate();
        }

        /************************************************************************************************************************/

        /// <summary>Logs a description of all states currently in the <see cref="AnimancerComponent.Playable"/>.</summary>
        [MenuItem(MenuItemPrefix + "Log Description of States", priority = MenuItemPriority)]
        private static void LogDescriptionOfStates(MenuCommand command)
        {
            var animancer = command.context as AnimancerComponent;
            var message = new StringBuilder();
            message.Append(animancer.ToString());
            if (animancer.IsPlayableInitialised)
            {
                message.Append(":\n");
                animancer.Playable.AppendDescription(message);
            }
            else
            {
                message.Append(": Playable is not initialised.");
            }

            AnimancerEditorUtilities.AppendNonCriticalIssues(message);

            Debug.Log(message, animancer);
        }

        /************************************************************************************************************************/
    }
}

#endif

