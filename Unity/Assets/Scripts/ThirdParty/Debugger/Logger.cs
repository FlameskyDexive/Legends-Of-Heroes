
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEngine;
using Debug = UnityEngine.Debug;

public static class Logger
{
    public static bool writeLog { get; set; }

    private static string LogFilePath
    {
        get { return Application.persistentDataPath + "/Log_" + System.DateTime.Now.ToString("yyyy-MM-dd") + ".txt"; }
    }

    [Conditional("Debugger")]
    public static void Log(object obj)
    {
        Debug.Log(obj);
    }

    [Conditional("Debugger")]
    public static void Warning(object obj)
    {
        Debug.LogWarning(obj);
    }

    [Conditional("Debugger")]
    public static void Error(object obj)
    {
        Debug.LogError(obj);
    }

    public static void WriteLogToText(string log, LogType logType = LogType.Error)
    {
        var sw = File.AppendText(LogFilePath);
        sw.WriteLine($"{System.DateTime.Now.ToString("G")}: {logType}: {log}");
        sw.Close();
    }


}


