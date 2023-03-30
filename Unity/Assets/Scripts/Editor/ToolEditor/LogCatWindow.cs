#if PLATFORM_ANDROID
using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEditor.Android;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
#if UNITY_2017_3_OR_NEWER
using UnityEditor.Compilation;
#endif

public class LogCatWindow : EditorWindow
{
    // How many log entries to store in memory. Keep it low for better performance.
    private const int memoryLimit = 2000;
    
    // How many log entries to show in unity3D editor. Keep it low for better performance.
    private const int showLimit = 200;
    
    // Filters
    private bool prefilterOnlyUnity = true;
    private bool filterOnlyError = false;
    private bool filterOnlyWarning = false;
    private bool filterOnlyDebug = false;
    private bool filterOnlyInfo = false;
    private bool filterOnlyVerbose = false;
    private string filterByString = String.Empty;
    
    // Android adb logcat process
    private Process logCatProcess;
    
    // Log entries
    private List<LogCatLog> logsList = new List<LogCatLog>();
    private List<LogCatLog> filteredList = new List<LogCatLog>(memoryLimit);
    private const string LogcatPattern = @"([0-1][0-9]-[0-3][0-9] [0-2][0-9]:[0-5][0-9]:[0-5][0-9]\.[0-9]{3}) ([WIEDV])/(.*)";
    private static readonly Regex LogcatRegex = new Regex(LogcatPattern, RegexOptions.Compiled);

    // Filtered GUI list scroll position
    private Vector2 scrollPosition = new Vector2(0, 0);
    
    // Add menu item named "LogCat" to the Window menu
    [MenuItem("Window/LogCat - Android Logger")]
    public static void ShowWindow()
    {
        // Show existing window instance. If one doesn't exist, make one.
        EditorWindow.GetWindow(typeof(LogCatWindow), false, "Logcat");
    }
    
    void Update()
    {
        if (logsList.Count == 0)
            return;
        
        lock (logsList)
        {
            // Filter
            filteredList = logsList.Where(log => (filterByString.Length <= 2 || log.Message.ToLower().Contains(filterByString.ToLower())) &&
                                          ((!filterOnlyError && !filterOnlyWarning && !filterOnlyDebug && !filterOnlyInfo && !filterOnlyVerbose) 
             || filterOnlyError && log.Type == 'E' 
             || filterOnlyWarning && log.Type == 'W' 
             || filterOnlyDebug && log.Type == 'D' 
             || filterOnlyInfo && log.Type == 'I' 
             || filterOnlyVerbose && log.Type == 'V')).ToList();
        }

        if (logCatProcess != null)
        {
            Repaint();
        }
    }
    
    void OnGUI()
    {
#if UNITY_EDITOR == false
		Debug.LogError("OnGUI");
#endif

        GUILayout.BeginHorizontal();
        
        // Enable pre-filter if process is not started
        GUI.enabled = logCatProcess == null;
        prefilterOnlyUnity = GUILayout.Toggle(prefilterOnlyUnity, "Only Unity", "Button", GUILayout.Width(80));
        
        // Enable button if process is not started
        GUI.enabled = logCatProcess == null;
        if (GUILayout.Button("Start", GUILayout.Width(60)))
        {
            string adbPath = GetAdbPath();

            // Start `adb logcat -c` to clear the log buffer
            ProcessStartInfo clearProcessInfo = new ProcessStartInfo();
            clearProcessInfo.WindowStyle = ProcessWindowStyle.Hidden;
            clearProcessInfo.CreateNoWindow = true;
            clearProcessInfo.UseShellExecute = false;
            clearProcessInfo.FileName = adbPath;
            clearProcessInfo.Arguments = @"logcat -c";
            Process.Start(clearProcessInfo);  
            
            // Start `adb logcat` (with additional optional arguments) process for filtering
            ProcessStartInfo logProcessInfo = new ProcessStartInfo();
            logProcessInfo.CreateNoWindow = true;
            logProcessInfo.UseShellExecute = false;
            logProcessInfo.RedirectStandardOutput = true;
            logProcessInfo.RedirectStandardError = true;
            logProcessInfo.StandardOutputEncoding = Encoding.UTF8;
            logProcessInfo.FileName = adbPath;
            logProcessInfo.WindowStyle = ProcessWindowStyle.Hidden;
            
            // Add additional -s argument for filtering by Unity tag.
            logProcessInfo.Arguments = "logcat -v time"+(prefilterOnlyUnity ? " -s  \"Unity\"": "");
            
            logCatProcess = Process.Start(logProcessInfo);  
            
            logCatProcess.ErrorDataReceived += (sender, errorLine) => { 
                if (errorLine.Data != null && errorLine.Data.Length > 2)
                    AddLog(new LogCatLog(errorLine.Data)); 
            };
            logCatProcess.OutputDataReceived += (sender, outputLine) => { 
                if (outputLine.Data != null && outputLine.Data.Length > 2)
                    AddLog(new LogCatLog(outputLine.Data)); 
            };
            logCatProcess.BeginErrorReadLine();
            logCatProcess.BeginOutputReadLine();
        }
        
        // Disable button if process is already started
        GUI.enabled = logCatProcess != null;
        if (GUILayout.Button("Stop", GUILayout.Width(60)))
        {
            StopLogCatProcess();
        }
        
        GUI.enabled = true;
        if (GUILayout.Button("Clear", GUILayout.Width(60)))
        {
            lock (logsList)
            {
                logsList.Clear();
                filteredList.Clear();
            }
        }
        
        GUILayout.Label(filteredList.Count + " matching logs", GUILayout.Height(20));
        
        // Create filters
        filterByString = GUILayout.TextField(filterByString, GUILayout.Height(20));
        GUI.color = new Color(0.75f, 0.5f, 0.5f, 1f);
        filterOnlyError = GUILayout.Toggle(filterOnlyError, "Error", "Button", GUILayout.Width(80));
        GUI.color = new Color(0.95f, 0.95f, 0.3f, 1f);
        filterOnlyWarning = GUILayout.Toggle(filterOnlyWarning, "Warning", "Button", GUILayout.Width(80));
        GUI.color = new Color(0.5f, 0.5f, 0.75f, 1f);
        filterOnlyDebug = GUILayout.Toggle(filterOnlyDebug, "Debug", "Button", GUILayout.Width(80));
        GUI.color = new Color(0.5f, 0.75f, 0.5f, 1f);
        filterOnlyInfo = GUILayout.Toggle(filterOnlyInfo, "Info", "Button", GUILayout.Width(80));
        GUI.color = Color.white;
        filterOnlyVerbose = GUILayout.Toggle(filterOnlyVerbose, "Verbose", "Button", GUILayout.Width(80));
        
        GUILayout.EndHorizontal(); 
        
        GUIStyle lineStyle = new GUIStyle();
        lineStyle.normal.background = MakeTexture(600, 1, new Color(1.0f, 1.0f, 1.0f, 0.1f));
        
        scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Height(Screen.height - 45));
        
        // Show only top `showingLimit` log entries
        int fromIndex = filteredList.Count - showLimit;
        if (fromIndex < 0)
            fromIndex = 0;
        
        for (int i = fromIndex; i < filteredList.Count; i++)
        {
            LogCatLog log = filteredList[i];
            GUI.backgroundColor = log.GetBgColor();
            GUILayout.BeginHorizontal(lineStyle);
            GUILayout.Label(log.CreationDate + " | " + log.Message);
            GUILayout.EndHorizontal(); 
        }
        
        GUILayout.EndScrollView();
    }
    
    private Texture2D MakeTexture(int width, int height, Color col)
    {
        Color[] pix = new Color[width * height];
        
        for (int i = 0; i < pix.Length; i++)
            pix [i] = col;
        
        Texture2D result = new Texture2D(width, height);
        result.SetPixels(pix);
        result.Apply();
        
        return result;
    }
    
    private void AddLog(LogCatLog log)
    {
        lock (logsList)
        {
            if (logsList.Count > memoryLimit + 1)
                logsList.RemoveRange(0, logsList.Count - memoryLimit + 1);
            
            logsList.Add(log);
        }
    }

    void OnEnable()
    {
#if UNITY_2017_3_OR_NEWER
        CompilationPipeline.assemblyCompilationStarted += OnAssemblyCompilationStarted;
#endif
    }

    void OnDisable()
    {
#if UNITY_2017_3_OR_NEWER
        CompilationPipeline.assemblyCompilationStarted -= OnAssemblyCompilationStarted;
#endif
    }

    void OnDestroy()
    {
        StopLogCatProcess();
    }

    private void StopLogCatProcess()
    {
        if (logCatProcess == null)
        {
            return;
        }
        try
        {
            if (!logCatProcess.HasExited)
            {
                logCatProcess.Kill();
            }
        }
        catch(InvalidOperationException)
        {
            // Just ignore it.
        }
        finally
        {
            logCatProcess.Dispose();
            logCatProcess = null;
        }
    }

    private void OnAssemblyCompilationStarted(string _)
    {
        StopLogCatProcess();
    }

    private class LogCatLog
    {
        public LogCatLog(string data)
        {
            // First char indicates error type:
            // W - warning
            // E - error
            // D - debug
            // I - info
            // V - verbose
            Match match = LogcatRegex.Match(data);
            if (match.Success)
            {
                Type = match.Groups[2].Value[0];

                Message = match.Groups[3].Value;
                CreationDate = match.Groups[1].Value;
            }
            else
            {
                Type = 'V';

                Message = data;
                CreationDate = DateTime.Now.ToString("MM-dd HH:mm:ss.fff");
            }
        }
        
        public string CreationDate
        {
            get;
            set;
        }
        
        public char Type
        {
            get;
            set;
        }
        
        public string Message
        {
            get;
            set;
        }
        
        public Color GetBgColor()
        {
            switch (Type)
            {
                case 'W':
                    return Color.yellow;
                    
                case 'I':
                    return Color.green;
                    
                case 'E':
                    return Color.red;
                    
                case 'D':
                    return Color.blue;
                    
                case 'V':
                default:
                    return Color.grey;
            }
        }
    }

    private static string GetAdbPath()
    {
#if UNITY_2019_1_OR_NEWER
        ADB adb = ADB.GetInstance();
        return adb == null ? string.Empty : adb.GetADBPath();
#else
        string androidSdkRoot = EditorPrefs.GetString("AndroidSdkRoot");
        if (string.IsNullOrEmpty(androidSdkRoot))
        {
            return string.Empty;
        }
        return Path.Combine(androidSdkRoot, Path.Combine("platform-tools", "adb"));
#endif
    }
}
#endif
