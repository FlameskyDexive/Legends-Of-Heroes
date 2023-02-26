using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

namespace Debugger
{
    public sealed partial class DebuggerComponent
    {
        [Serializable]
        private sealed class ConsoleWindow : IDebuggerWindow
        {
            private readonly Queue<LogNode> m_LogNodes = new Queue<LogNode>();

            private Vector2 m_LogScrollPosition = Vector2.zero;
            private Vector2 m_StackScrollPosition = Vector2.zero;
            private int m_InfoCount = 0;
            private int m_WarningCount = 0;
            private int m_ErrorCount = 0;
            private int m_FatalCount = 0;
            private LogNode m_SelectedNode = null;
            private bool m_LastLockScroll = true;
            private bool m_LastInfoFilter = true;
            private bool m_LastWarningFilter = true;
            private bool m_LastErrorFilter = true;
            private bool m_LastFatalFilter = true;

            [SerializeField]
            private bool m_LockScroll = false;

            [SerializeField]
            private int m_MaxLine = 100;

            [SerializeField]
            private bool m_InfoFilter = true;

            [SerializeField]
            private bool m_WarningFilter = true;

            [SerializeField]
            private bool m_ErrorFilter = true;

            [SerializeField]
            private bool m_FatalFilter = true;

            [SerializeField]
            private bool m_saveLog = false;

            [SerializeField]
            private Color32 m_InfoColor = Color.white;

            [SerializeField]
            private Color32 m_WarningColor = Color.yellow;

            [SerializeField]
            private Color32 m_ErrorColor = Color.red;

            [SerializeField]
            private Color32 m_FatalColor = new Color(0.7f, 0.2f, 0.2f);

            public bool LockScroll
            {
                get
                {
                    return m_LockScroll;
                }
                set
                {
                    m_LockScroll = value;
                }
            }

            public int MaxLine
            {
                get
                {
                    return m_MaxLine;
                }
                set
                {
                    m_MaxLine = value;
                }
            }

            public bool InfoFilter
            {
                get
                {
                    return m_InfoFilter;
                }
                set
                {
                    m_InfoFilter = value;
                }
            }

            public bool WarningFilter
            {
                get
                {
                    return m_WarningFilter;
                }
                set
                {
                    m_WarningFilter = value;
                }
            }

            public bool ErrorFilter
            {
                get
                {
                    return m_ErrorFilter;
                }
                set
                {
                    m_ErrorFilter = value;
                }
            }

            public bool FatalFilter
            {
                get
                {
                    return m_FatalFilter;
                }
                set
                {
                    m_FatalFilter = value;
                }
            }

            public int InfoCount
            {
                get
                {
                    return m_InfoCount;
                }
            }

            public int WarningCount
            {
                get
                {
                    return m_WarningCount;
                }
            }

            public int ErrorCount
            {
                get
                {
                    return m_ErrorCount;
                }
            }

            public int FatalCount
            {
                get
                {
                    return m_FatalCount;
                }
            }

            public Color32 InfoColor
            {
                get
                {
                    return m_InfoColor;
                }
                set
                {
                    m_InfoColor = value;
                }
            }

            public Color32 WarningColor
            {
                get
                {
                    return m_WarningColor;
                }
                set
                {
                    m_WarningColor = value;
                }
            }

            public Color32 ErrorColor
            {
                get
                {
                    return m_ErrorColor;
                }
                set
                {
                    m_ErrorColor = value;
                }
            }

            public Color32 FatalColor
            {
                get
                {
                    return m_FatalColor;
                }
                set
                {
                    m_FatalColor = value;
                }
            }
            private StreamWriter writer;


            public Texture2D searchImage;
            public Texture2D barImage;
            public Texture2D barActiveImage;
            public GUIContent searchContent;
            bool showSearchText = false;
            GUIStyle searchStyle;
            public Vector2 size = new Vector2(32, 32);
            GUIStyle barStyle;

            public void Initialize(params object[] args)
            {

                Application.logMessageReceived += OnLogMessageReceived;
                m_LockScroll = m_LastLockScroll = false;
                m_InfoFilter = m_LastInfoFilter = true;
                m_WarningFilter = m_LastWarningFilter = true;
                m_ErrorFilter = m_LastErrorFilter = true;
                m_FatalFilter = m_LastFatalFilter = true;
                m_saveLog = true;

                if(!Directory.Exists(LogFilePath))
                {
                    Directory.CreateDirectory(LogFilePath);
                }
                writer = new StreamWriter(LogFilePath + LogFileName);

                searchImage = Resources.Load<Texture2D>($"SearchIcon");
                searchContent = new GUIContent("", searchImage, "Search for logs");
                searchStyle = new GUIStyle();
                searchStyle.clipping = TextClipping.Clip;
                searchStyle.alignment = TextAnchor.LowerCenter;
                searchStyle.fontSize = (int)(size.y / 2);
                searchStyle.wordWrap = true;

                barImage = Resources.Load<Texture2D>($"SlicedBackground");
                barActiveImage = Resources.Load<Texture2D>($"SlicedBackground2");
                barStyle = new GUIStyle();
                barStyle.border = new RectOffset(1, 1, 1, 1);
                barStyle.normal.background = barImage;
                barStyle.active.background = barActiveImage;
                barStyle.alignment = TextAnchor.MiddleCenter;
                barStyle.margin = new RectOffset(1, 1, 1, 1);
                barStyle.clipping = TextClipping.Clip;
                barStyle.fontSize = (int)(size.y / 2);

            }

            public void Shutdown()
            {
                Application.logMessageReceived -= OnLogMessageReceived;
                Clear();
                writer.Close();
            }

            public void OnEnter()
            {
            }

            public void OnLeave()
            {
            }

            public void OnUpdate(float elapseSeconds, float realElapseSeconds)
            {
                if (m_LastLockScroll != m_LockScroll)
                {
                    m_LastLockScroll = m_LockScroll;
                }

                if (m_LastInfoFilter != m_InfoFilter)
                {
                    m_LastInfoFilter = m_InfoFilter;
                }

                if (m_LastWarningFilter != m_WarningFilter)
                {
                    m_LastWarningFilter = m_WarningFilter;
                }

                if (m_LastErrorFilter != m_ErrorFilter)
                {
                    m_LastErrorFilter = m_ErrorFilter;
                }

                if (m_LastFatalFilter != m_FatalFilter)
                {
                    m_LastFatalFilter = m_FatalFilter;
                }
            }

            Rect tempRect;
            string filterText = "";
            public void OnDraw()
            {
                RefreshCount();

                GUILayout.BeginHorizontal();
                {
                    if (GUILayout.Button("Clear", GUILayout.Width(50f)))
                    {
                        Clear();
                    }
                    if (GUILayout.Button(searchContent, searchStyle, GUILayout.Width(40), GUILayout.Height(size.y)))
                    {
                        showSearchText = !showSearchText;
                    }
                    if (showSearchText)
                    {
                        GUILayout.Box(searchContent, barStyle, GUILayout.Width(80), GUILayout.Height(30));
                        tempRect = GUILayoutUtility.GetLastRect();
                        string newFilterText = GUI.TextField(tempRect, filterText, barStyle);
                        if (newFilterText != filterText)
                        {
                            filterText = newFilterText;
                            
                        }
                    }
                    m_LockScroll = GUILayout.Toggle(m_LockScroll, "LockScroll", GUILayout.Width(80f));
                    GUILayout.Space(2);
                    m_InfoFilter = GUILayout.Toggle(m_InfoFilter, Utility.Text.Format("Info({0})", m_InfoCount), GUILayout.Width(80f));
                    m_WarningFilter = GUILayout.Toggle(m_WarningFilter, Utility.Text.Format("Warning({0})", m_WarningCount), GUILayout.Width(80f));
                    m_ErrorFilter = GUILayout.Toggle(m_ErrorFilter, Utility.Text.Format("Error({0})", m_ErrorCount), GUILayout.Width(80f));
                    m_FatalFilter = GUILayout.Toggle(m_FatalFilter, Utility.Text.Format("Fatal({0})", m_FatalCount), GUILayout.Width(80f));
                    m_saveLog = GUILayout.Toggle(m_saveLog, Utility.Text.Format("SaveLog", m_FatalCount), GUILayout.Width(80f));
                }
                GUILayout.EndHorizontal();

                GUILayout.BeginVertical("box");
                {
                    if (m_LockScroll)
                    {
                        m_LogScrollPosition.y = float.MaxValue;
                    }

                    m_LogScrollPosition = GUILayout.BeginScrollView(m_LogScrollPosition);
                    {
                        bool selected = false;
                        foreach (LogNode logNode in m_LogNodes)
                        {
                            if (showSearchText && !string.IsNullOrEmpty(filterText))
                            {
                                if(!logNode.LogMessage.ToLower().Contains(filterText.ToLower()))
                                {
                                    continue;
                                }
                            }
                            switch (logNode.LogType)
                            {
                                case LogType.Log:
                                    if (!m_InfoFilter)
                                    {
                                        continue;
                                    }
                                    break;

                                case LogType.Warning:
                                    if (!m_WarningFilter)
                                    {
                                        continue;
                                    }
                                    break;

                                case LogType.Error:
                                    if (!m_ErrorFilter)
                                    {
                                        continue;
                                    }
                                    break;

                                case LogType.Exception:
                                    if (!m_FatalFilter)
                                    {
                                        continue;
                                    }
                                    break;
                            }
                            if (GUILayout.Toggle(m_SelectedNode == logNode, GetLogString(logNode)))
                            {
                                selected = true;
                                if (m_SelectedNode != logNode)
                                {
                                    m_SelectedNode = logNode;
                                    m_StackScrollPosition = Vector2.zero;
                                }
                            }
                        }
                        if (!selected)
                        {
                            m_SelectedNode = null;
                        }
                    }
                    GUILayout.EndScrollView();
                }
                GUILayout.EndVertical();

                GUILayout.BeginVertical("box");
                {
                    m_StackScrollPosition = GUILayout.BeginScrollView(m_StackScrollPosition, GUILayout.Height(100f));
                    {
                        if (m_SelectedNode != null)
                        {
                            Color32 color = GetLogStringColor(m_SelectedNode.LogType);
                            if (GUILayout.Button(Utility.Text.Format("<color=#{0:x2}{1:x2}{2:x2}{3:x2}><b>{4}</b></color>{6}{6}{5}", color.r, color.g, color.b, color.a, m_SelectedNode.LogMessage, m_SelectedNode.StackTrack, Environment.NewLine), "label"))
                            {
                                CopyToClipboard(Utility.Text.Format("{0}{2}{2}{1}", m_SelectedNode.LogMessage, m_SelectedNode.StackTrack, Environment.NewLine));
                            }
                        }
                    }
                    GUILayout.EndScrollView();
                }
                GUILayout.EndVertical();
            }

            private void Clear()
            {
                m_LogNodes.Clear();
            }

            public void RefreshCount()
            {
                m_InfoCount = 0;
                m_WarningCount = 0;
                m_ErrorCount = 0;
                m_FatalCount = 0;
                foreach (LogNode logNode in m_LogNodes)
                {
                    switch (logNode.LogType)
                    {
                        case LogType.Log:
                            m_InfoCount++;
                            break;

                        case LogType.Warning:
                            m_WarningCount++;
                            break;

                        case LogType.Error:
                            m_ErrorCount++;
                            break;

                        case LogType.Exception:
                            m_FatalCount++;
                            break;
                    }
                }
            }

            public void GetRecentLogs(List<LogNode> results)
            {
                if (results == null)
                {
                    Debug.LogErrorFormat("Results is invalid.");
                    return;
                }

                results.Clear();
                foreach (LogNode logNode in m_LogNodes)
                {
                    results.Add(logNode);
                }
            }

            public void GetRecentLogs(List<LogNode> results, int count)
            {
                if (results == null)
                {
                    Debug.LogErrorFormat("Results is invalid.");
                    return;
                }

                if (count <= 0)
                {
                    UnityEngine.Debug.LogErrorFormat("Count is invalid.");
                    return;
                }

                int position = m_LogNodes.Count - count;
                if (position < 0)
                {
                    position = 0;
                }

                int index = 0;
                results.Clear();
                foreach (LogNode logNode in m_LogNodes)
                {
                    if (index++ < position)
                    {
                        continue;
                    }

                    results.Add(logNode);
                }
            }

            private void OnLogMessageReceived(string logMessage, string stackTrace, LogType logType)
            {
                if (logType == LogType.Assert)
                {
                    logType = LogType.Error;
                }

                m_LogNodes.Enqueue(LogNode.Create(logType, logMessage, stackTrace));
                if(m_saveLog)
                {
                    WriteLogToText(logMessage, logType);
                }
                while (m_LogNodes.Count > m_MaxLine)
                {
                    ReferencePool.Release(m_LogNodes.Dequeue());
                }
            }

            private string GetLogString(LogNode logNode)
            {
                Color32 color = GetLogStringColor(logNode.LogType);
                return Utility.Text.Format("<color=#{0:x2}{1:x2}{2:x2}{3:x2}>[{4:HH:mm:ss.fff}][{5}] {6}</color>", color.r, color.g, color.b, color.a, logNode.LogTime.ToLocalTime(), logNode.LogFrameCount, logNode.LogMessage);
            }

            internal Color32 GetLogStringColor(LogType logType)
            {
                Color32 color = Color.white;
                switch (logType)
                {
                    case LogType.Log:
                        color = m_InfoColor;
                        break;

                    case LogType.Warning:
                        color = m_WarningColor;
                        break;

                    case LogType.Error:
                        color = m_ErrorColor;
                        break;

                    case LogType.Exception:
                        color = m_FatalColor;
                        break;
                }

                return color;
            }

            public void WriteLogToText(string log, LogType logType = LogType.Error)
            {
                writer.WriteLine($"{System.DateTime.Now.ToString("G")}: {logType}: {log}");
                writer.Flush();
            }
            private string LogFilePath
            {
                get { return $"{Application.persistentDataPath}/Logs/"; }
            }

            private string LogFileName
            {
                get { return $"/Log_{System.DateTime.Now.ToString("yyyy-MM-dd")}.txt"; }
            }
        }
    }
}
