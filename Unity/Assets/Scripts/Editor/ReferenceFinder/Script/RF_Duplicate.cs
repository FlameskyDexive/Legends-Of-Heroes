//#define RF_DEBUG

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using CBParams = System.Collections.Generic.List<System.Collections.Generic.List<string>>;
using UnityObject = UnityEngine.Object;

namespace ReferenceFinder
{
    internal class RF_DuplicateTree2 : IRefDraw
    {
        private const float TimeDelayDelete = .5f;

        private static readonly RF_FileCompare fc = new RF_FileCompare();
        private readonly RF_TreeUI2.GroupDrawer groupDrawer;
        private CBParams cacheAssetList;
        public bool caseSensitive = false;
        private Dictionary<string, List<RF_Ref>> dicIndex; //index, list

        private bool dirty;
        private int excludeCount;
        private string guidPressDelete;
        
        private readonly Func<RF_RefDrawer.Sort> getSortMode;
        private readonly Func<RF_RefDrawer.Mode> getGroupMode;
        
        internal List<RF_Ref> list;
        internal Dictionary<string, RF_Ref> refs;
        public int scanExcludeByIgnoreCount;
        public int scanExcludeByTypeCount;
        private readonly string searchTerm = "";
        private float TimePressDelete;

        public RF_DuplicateTree2(IWindow window, Func<RF_RefDrawer.Sort> getSortMode, Func<RF_RefDrawer.Mode> getGroupMode)
        {
            this.window = window;
            this.getSortMode = getSortMode;
            this.getGroupMode = getGroupMode;
            groupDrawer = new RF_TreeUI2.GroupDrawer(DrawGroup, DrawAsset);
        }

        public IWindow window { get; set; }

        public bool Draw(Rect rect)
        {

            return false;
        }

        public bool DrawLayout()
        {
            if (dirty) RefreshView(cacheAssetList);

            if ((fc.nChunks2 > 0) && (fc.nScaned < fc.nChunks2))
            {
                Rect rect = GUILayoutUtility.GetRect(1, Screen.width, 18f, 18f);
                float p = fc.nScaned / (float)fc.nChunks2;

                EditorGUI.ProgressBar(rect, p, string.Format("Scanning {0} / {1}", fc.nScaned, fc.nChunks2));
                GUILayout.FlexibleSpace();
            } else
            {
                if (groupDrawer.hasValidTree) groupDrawer.tree.itemPaddingRight = 60f;
                groupDrawer.DrawLayout();    
            }
            
            DrawHeader();
            return false;
        }

        public int ElementCount()
        {
            return list?.Count ?? 0;
        }

        private void DrawAsset(Rect r, string guid)
        {
            if (!refs.TryGetValue(guid, out RF_Ref rf)) return;
            
            rf.asset.Draw(r, false,
                getGroupMode() != RF_RefDrawer.Mode.Folder,
                RF_Setting.ShowFileSize,
                RF_Setting.s.displayAssetBundleName,
                RF_Setting.s.displayAtlasName,
                RF_Setting.s.showUsedByClassed,
                window);

            Texture tex = AssetDatabase.GetCachedIcon(rf.asset.assetPath);
            if (tex == null) return;

            Rect drawR = r;
            drawR.x = drawR.x + drawR.width; // (groupDrawer.TreeNoScroll() ? 60f : 70f) ;
            drawR.width = 40f;
            drawR.y += 1;
            drawR.height -= 2;

            if (GUI.Button(drawR, "Use", EditorStyles.miniButton))
            {
                if (RF_Export.IsMergeProcessing)
                    Debug.LogWarning("Previous merge is processing");
                else
                {
                    //AssetDatabase.SaveAssets();
                    //EditorGUIUtility.systemCopyBuffer = rf.asset.guid;
                    //EditorGUIUtility.systemCopyBuffer = rf.asset.guid;
                    // Debug.Log("guid: " + rf.asset.guid + "  systemCopyBuffer " + EditorGUIUtility.systemCopyBuffer);
                    int index = rf.index;
                    Selection.objects = list.Where(x => x.index == index)
                        .Select(x => RF_Unity.LoadAssetAtPath<UnityObject>(x.asset.assetPath)).ToArray();
                    RF_Export.MergeDuplicate(rf.asset.guid);
                }
            }

            if (rf.asset.UsageCount() > 0) return;

            drawR.x -= 25;
            drawR.width = 20;
            if (wasPreDelete(guid))
            {
                Color col = GUI.color;
                GUI.color = Color.red;
                if (GUI.Button(drawR, "X", EditorStyles.miniButton))
                {
                    guidPressDelete = null;
                    AssetDatabase.DeleteAsset(rf.asset.assetPath);
                }

                GUI.color = col;
                window.WillRepaint = true;
            } else
            {
                if (GUI.Button(drawR, "X", EditorStyles.miniButton))
                {
                    guidPressDelete = guid;
                    TimePressDelete = Time.realtimeSinceStartup;
                    window.WillRepaint = true;
                }
            }
        }

        private bool wasPreDelete(string guid)
        {
            if (guidPressDelete == null || guid != guidPressDelete) return false;

            if (Time.realtimeSinceStartup - TimePressDelete < TimeDelayDelete) return true;

            guidPressDelete = null;
            return false;
        }

        private void DrawGroup(Rect r, string label, int childCount)
        {
            // GUI.Label(r, label + " (" + childCount + ")", EditorStyles.boldLabel);
            RF_Asset asset = dicIndex[label][0].asset;

            Texture tex = AssetDatabase.GetCachedIcon(asset.assetPath);
            Rect rect = r;

            if (tex != null)
            {
                rect.width = 16f;
                GUI.DrawTexture(rect, tex);
            }

            rect = r;
            rect.xMin += 16f;
            GUI.Label(rect, asset.assetName, EditorStyles.boldLabel);

            rect = r;
            rect.xMin += rect.width - 50f;
            GUI.Label(rect, RF_Helper.GetfileSizeString(asset.fileSize), EditorStyles.miniLabel);

            rect = r;
            rect.xMin += rect.width - 70f;
            GUI.Label(rect, childCount.ToString(), EditorStyles.miniLabel);

            rect = r;
            rect.xMin += rect.width - 70f;
        }


        // private List<RF_DuplicateFolder> duplicated;

        public void Reset(CBParams assetList)
        {
            fc.Reset(assetList, OnUpdateView, RefreshView);
        }

        private void OnUpdateView(CBParams assetList)
        { }

        public bool isExclueAnyItem()
        {
            return excludeCount > 0 || scanExcludeByTypeCount > 0;
        }

        public bool isExclueAnyItemByIgnoreFolder()
        {
            return scanExcludeByIgnoreCount > 0;
        }

        // void OnActive
        private void RefreshView(CBParams assetList)
        {
            cacheAssetList = assetList;
            dirty = false;
            list = new List<RF_Ref>();
            refs = new Dictionary<string, RF_Ref>();
            dicIndex = new Dictionary<string, List<RF_Ref>>();
            if (assetList == null) return;

            int minScore = searchTerm.Length;
            string term1 = searchTerm;
            if (!caseSensitive) term1 = term1.ToLower();

            string term2 = term1.Replace(" ", string.Empty);
            excludeCount = 0;

            for (var i = 0; i < assetList.Count; i++)
            {
                var lst = new List<RF_Ref>();
                for (var j = 0; j < assetList[i].Count; j++)
                {
                    string path = assetList[i][j];
                    if (!path.StartsWith("Assets/"))
                    {
                        Debug.LogWarning("Ignore asset: " + path);
                        continue;
                    }

                    string guid = AssetDatabase.AssetPathToGUID(path);
                    if (string.IsNullOrEmpty(guid)) continue;

                    if (refs.ContainsKey(guid)) continue;

                    RF_Asset asset = RF_Cache.Api.Get(guid);
                    if (asset == null) continue;
                    if (!asset.assetPath.StartsWith("Assets/")) continue; // ignore builtin, packages, ...

                    var fr2 = new RF_Ref(i, 0, asset, null);

                    if (RF_Setting.IsTypeExcluded(fr2.type))
                    {
                        excludeCount++;
                        continue; //skip this one
                    }

                    if (string.IsNullOrEmpty(searchTerm))
                    {
                        fr2.matchingScore = 0;
                        list.Add(fr2);
                        lst.Add(fr2);
                        refs.Add(guid, fr2);
                        continue;
                    }

                    //calculate matching score
                    string name1 = fr2.asset.assetName;
                    if (!caseSensitive) name1 = name1.ToLower();

                    string name2 = name1.Replace(" ", string.Empty);

                    int score1 = RF_Unity.StringMatch(term1, name1);
                    int score2 = RF_Unity.StringMatch(term2, name2);

                    fr2.matchingScore = Mathf.Max(score1, score2);
                    if (fr2.matchingScore > minScore)
                    {
                        list.Add(fr2);
                        lst.Add(fr2);
                        refs.Add(guid, fr2);
                    }
                }

                dicIndex.Add(i.ToString(), lst);
            }

            ResetGroup();
        }

        private void ResetGroup()
        {
            groupDrawer.Reset(list,
                rf => rf.asset.guid
                , GetGroup, SortGroup);
            if (window != null) window.Repaint();
        }

        private string GetGroup(RF_Ref rf)
        {
            return rf.index.ToString();
        }

        private void SortGroup(List<string> groups)
        {
            // groups.Sort( (item1, item2) =>
            // {
            // 	if (item1 == "Others" || item2 == "Selection") return 1;
            // 	if (item2 == "Others" || item1 == "Selection") return -1;
            // 	return item1.CompareTo(item2);
            // });
        }

        public void SetDirty()
        {
            dirty = true;
        }

        public void RefreshSort()
        { }

        private void DrawHeader()
        {
            string text = groupDrawer.hasValidTree ? "Rescan" : "Scan";

            if (GUILayout.Button(text))

                // if (RF_Cache)
            {
                OnCacheReady();
            }

            // RF_Cache.onReady -= OnCacheReady;
            // RF_Cache.onReady += OnCacheReady;
            // RF_Cache.Api.Check4Changes(false);
        }

        private void OnCacheReady()
        {
            scanExcludeByTypeCount = 0;
            Reset(RF_Cache.Api.ScanSimilar(IgnoreTypeWhenScan, IgnoreFolderWhenScan));
            RF_Cache.onReady -= OnCacheReady;
        }

        private void IgnoreTypeWhenScan()
        {
            scanExcludeByTypeCount++;
        }

        private void IgnoreFolderWhenScan()
        {
            scanExcludeByIgnoreCount++;
        }
    }

    internal class RF_FileCompare
    {
        public static HashSet<RF_Chunk> HashChunksNotComplete;
        internal static int streamClosedCount;
        private CBParams cacheList;
        public List<RF_Head> deads = new List<RF_Head>();
        public List<RF_Head> heads = new List<RF_Head>();

        public int nChunks;
        public int nChunks2;
        public int nScaned;
        public Action<CBParams> OnCompareComplete;
        public Action<CBParams> OnCompareUpdate;

        // private int streamCount;

        public void Reset(CBParams list, Action<CBParams> onUpdate, Action<CBParams> onComplete)
        {
            nChunks = 0;
            nScaned = 0;
            nChunks2 = 0;

            // streamCount = streamClosedCount = 0;
            HashChunksNotComplete = new HashSet<RF_Chunk>();

            if (heads.Count > 0)
                for (var i = 0; i < heads.Count; i++)
                {
                    heads[i].CloseChunk();
                }

            deads.Clear();
            heads.Clear();

            OnCompareUpdate = onUpdate;
            OnCompareComplete = onComplete;
            if (list.Count <= 0)
            {
                OnCompareComplete(new CBParams());
                return;
            }

            cacheList = list;
            // Debug.Log($"Found: {list.Count}");
            
            for (var i = 0; i < list.Count; i++)
            {
                var file = new FileInfo(list[i][0]);
                int nChunk = Mathf.CeilToInt(file.Length / (float)RF_Head.chunkSize);
                nChunks2 += nChunk;

                // string str = string.Join("\n", list[i]);
                // Debug.Log($"Check: {i}\n{str}");
            }

            // for(int i =0;i< list.Count;i++)
            // {
            //     AddHead(list[i]);
            // }
            AddHead(cacheList[cacheList.Count - 1]);
            cacheList.RemoveAt(cacheList.Count - 1);

            EditorApplication.update -= ReadChunkAsync;
            EditorApplication.update += ReadChunkAsync;
        }

        public RF_FileCompare AddHead(List<string> files)
        {
            if (files.Count < 2) Debug.LogWarning("Something wrong ! head should not contains < 2 elements");
            
            var chunkList = new List<RF_Chunk>();
            for (var i = 0; i < files.Count; i++)
            {
                // streamCount++;
                // try 
                // {
                // 	Debug.Log("new stream ");
                // 	stream = new FileStream(files[i], FileMode.Open, FileAccess.Read);
                // }
                // catch (Exception e)
                // {
                // 	Debug.LogWarning(e + "\nCan not open file: " + files[i]);
                // 	if (stream != null) stream.Close();
                // 	continue;
                // }

                chunkList.Add(new RF_Chunk
                {
                    file = files[i],
                    buffer = new byte[RF_Head.chunkSize]
                });
            }

            var file = new FileInfo(files[0]);
            int nChunk = Mathf.CeilToInt(file.Length / (float)RF_Head.chunkSize);

            heads.Add(new RF_Head
            {
                fileSize = file.Length,
                currentChunk = 0,
                nChunk = nChunk,
                chunkList = chunkList
            });
            
            // Debug.LogWarning($"Add New Head: \n {string.Join("\n", heads[^1].GetFiles().ToArray())}");
            nChunks += nChunk;

            return this;
        }

        // private bool checkCompleteAllCurFile()
        // {
        // 	return streamClosedCount + HashChunksNotComplete.Count >= streamCount; //-1 for safe
        // }

        private void ReadChunkAsync()
        {
            bool alive = ReadChunk();
            if (alive) return;
            
            var update = false;
            for (int i = heads.Count - 1; i >= 0; i--)
            {
                RF_Head h = heads[i];
                if (!h.isDead) continue;

                h.CloseChunk();
                heads.RemoveAt(i);
                if (h.chunkList.Count > 1)
                {
                    update = true;
                    deads.Add(h);

                    // string[] str = h.chunkList.Select(item => item.file).ToArray();
                    // Debug.Log($"AddResult: {string.Join("\n", str)}");
                }
            }
            
            if (update) Trigger(OnCompareUpdate);

            if (cacheList.Count == 0)
            {
                foreach (RF_Chunk item in HashChunksNotComplete)
                {
                    if ((item.stream == null) || !item.stream.CanRead) continue;
                    item.stream.Close();
                    item.stream = null;
                }

                HashChunksNotComplete.Clear();
                nScaned = nChunks;
                EditorApplication.update -= ReadChunkAsync;
                Trigger(OnCompareComplete);
            } 
            else
            {
                AddHead(cacheList[cacheList.Count-1]);
                cacheList.RemoveAt(cacheList.Count - 1);
            }
        }

        private void Trigger(Action<CBParams> cb)
        {
            if (cb == null) return;

            CBParams list = deads.Select(item => item.GetFiles()).ToList();

            //#if RF_DEBUG
            //        Debug.Log("Callback ! " + deads.Count + ":" + heads.Count);
            //#endif
            cb(list);
        }

        private bool ReadChunk()
        {
            var alive = false;

            for (var i = 0; i < heads.Count; i++)
            {
                RF_Head h = heads[i];
                if (h.isDead)
                {
                    //Debug.LogWarning("Should never be here : " + h.chunkList[0].file);
                    continue;
                }
                
                nScaned++;
                alive = true;
                h.ReadChunk();
                h.CompareChunk(heads);
                break;
            }
            
            return alive;
        }
    }

    internal class RF_Head
    {
        public const int chunkSize = 10240;

        public List<RF_Chunk> chunkList;
        public int currentChunk;

        public long fileSize;

        public int nChunk;
        public int size; //last stream read size

        public bool isDead => currentChunk == nChunk || chunkList.Count == 1;

        public List<string> GetFiles()
        {
            return chunkList.Select(item => item.file).ToList();
        }

        public void AddToDict(byte b, RF_Chunk chunk, Dictionary<byte, List<RF_Chunk>> dict)
        {
            List<RF_Chunk> list;
            if (!dict.TryGetValue(b, out list))
            {
                list = new List<RF_Chunk>();
                dict.Add(b, list);
            }

            list.Add(chunk);
        }

        public void CloseChunk()
        {
            for (var i = 0; i < chunkList.Count; i++)
            {
                RF_FileCompare.streamClosedCount++;

                if (chunkList[i].stream != null)
                {
					#if RF_DEBUG
					Debug.Log("stream close: " + chunkList[i].file);
					#endif

                    chunkList[i].stream.Close();
                    chunkList[i].stream = null;
                }
            }
        }

        public void ReadChunk()
        {
#if RF_DEBUG
        if (currentChunk == 0) Debug.LogWarning("Read <" + chunkList[0].file + "> " + currentChunk + ":" + nChunk);
#endif
            if (currentChunk == nChunk)
            {
                Debug.LogWarning("Something wrong, should dead <" + isDead + ">");
                return;
            }

            int from = currentChunk * chunkSize;
            size = (int)Mathf.Min(fileSize - from, chunkSize);

            for (var i = 0; i < chunkList.Count; i++)
            {
                RF_Chunk chunk = chunkList[i];
                if (chunk.streamError) continue;
                chunk.size = size;

                if (chunk.streamInited == false)
                {
                    chunk.streamInited = true;

                    try
                    {
						#if RF_DEBUG
						Debug.Log("New chunk: " + chunk.file);
						#endif
                        chunk.stream = new FileStream(chunk.file, FileMode.Open, FileAccess.Read);
                    }
					#if RF_DEBUG
                    catch (Exception e)
                    {
						
						Debug.LogWarning("Exception: " + e + "\n" + chunk.file + "\n" + chunk.stream);
					#else
                    catch
                    {
					#endif

                        chunk.streamError = true;
                        if (chunk.stream != null) // just to make sure we close the stream
                        {
                            chunk.stream.Close();
                            chunk.stream = null;
                        }
                    }

                    if (chunk.stream == null)
                    {
                        chunk.streamError = true;
                        continue;
                    }
                }

                try
                {
                    chunk.stream.Read(chunk.buffer, 0, size);
                } catch (Exception e)
                {
                    Debug.LogWarning(e + "\n" + chunk.file);

                    chunk.streamError = true;
                    chunk.stream.Close();
                }
            }

            // clean up dead chunks
            for (int i = chunkList.Count - 1; i >= 0; i--)
            {
                if (chunkList[i].streamError) chunkList.RemoveAt(i);
            }

            if (chunkList.Count == 1) Debug.LogWarning("No more chunk in list");

            currentChunk++;
        }

        public void CompareChunk(List<RF_Head> heads)
        {
            int idx = chunkList.Count;
            byte[] buffer = chunkList[idx - 1].buffer;

            while (--idx >= 0)
            {
                RF_Chunk chunk = chunkList[idx];
                int diff = FirstDifferentIndex(buffer, chunk.buffer, size);
                if (diff == -1) continue;
                
#if RF_DEBUG
            Debug.Log(string.Format(chunkList[idx].file + 
                " --> Different found at : idx={0} diff={1} size={2} chunk={3}",
            idx, diff, size, currentChunk));
#endif

                byte v = buffer[diff];
                
                var d = new Dictionary<byte, List<RF_Chunk>>(); //new heads
                chunkList.RemoveAt(idx);
                RF_FileCompare.HashChunksNotComplete.Add(chunk);

                AddToDict(chunk.buffer[diff], chunk, d);

                for (int j = idx - 1; j >= 0; j--)
                {
                    RF_Chunk tChunk = chunkList[j];
                    byte tValue = tChunk.buffer[diff];
                    if (tValue == v) continue;

                    idx--;
                    RF_FileCompare.HashChunksNotComplete.Add(tChunk);
                    chunkList.RemoveAt(j);
                    AddToDict(tChunk.buffer[diff], tChunk, d);
                }

                foreach (KeyValuePair<byte, List<RF_Chunk>> item in d)
                {
                    List<RF_Chunk> list = item.Value;
                    if (list.Count == 1)
                    {
#if RF_DEBUG
                    Debug.Log(" --> Dead head found for : " + list[0].file);
#endif
                        if (list[0].stream != null) list[0].stream.Close();
                    } else if (list.Count > 1) // 1 : dead head
                    {
#if RF_DEBUG
                    Debug.Log(" --> NEW HEAD : " + list[0].file);
#endif
                        heads.Add(new RF_Head
                        {
                            nChunk = nChunk,
                            fileSize = fileSize,
                            currentChunk = currentChunk - 1,
                            chunkList = list
                        });
                    }
                }
            }
        }

        internal static int FirstDifferentIndex(byte[] arr1, byte[] arr2, int maxIndex)
        {
            for (var i = 0; i < maxIndex; i++)
            {
                if (arr1[i] != arr2[i]) return i;
            }

            return -1;
        }
    }

    internal class RF_Chunk
    {
        public byte[] buffer;
        public string file;
        public long size;
        public FileStream stream;
        public bool streamError;

        public bool streamInited;
    }
}
