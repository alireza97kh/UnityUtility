using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DobeilLogger : MonoSingleton<DobeilLogger>
{
    public bool isActive = true;
    public static void Log(string msg)
    {
        if (Instance.isActive)
            Debug.Log(msg);
    }
    public static void LogError(string msg)
    {
        if (Instance.isActive)
            Debug.LogError(msg);
    }
    public static void LogException(Exception msg)
    {
        if (Instance.isActive)
            Debug.LogException(msg);
    }
    public static void LogWarning(string msg)
    {
        if (Instance.isActive)
            Debug.LogWarning(msg);
    }
    public static void Log(string msg, string color)
    {
        if (Instance.isActive)
            Debug.Log("<color=" + color + ">" + msg + "</color>");
    }
    public static void Log(string msg, Color color)
    {
        if (Instance.isActive)
            Debug.Log("<color=" + color + ">" + msg + "</color>");
    }
    public static void LogFormat(LogType logType, UnityEngine.Object context, string format, params object[] args)
    {
        if (Instance.isActive)
            Debug.unityLogger.logHandler.LogFormat(logType, context, format, args);
    }

    public static void LogException(Exception exception, UnityEngine.Object context)
    {
        if (Instance.isActive)
            Debug.unityLogger.LogException(exception, context);
    }
}
