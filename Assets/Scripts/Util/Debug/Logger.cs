// ==================================================
// Logger.cs
// ==================================================
// 이 소스 코드의 권리를 명시하는 주석을 제거하지 마시오.
// 소스 코드에 대한 모든 권리는 (주)크로노웨어즈에 있습니다.
//
// Copyright 2020 (c) ChronoWares All Rights Reserved.
// ==================================================

using UnityEngine;

public static class Logger
{
    public static bool isDebugBuild = false;
    public static Constant.LogLevel logLevel = Constant.LogLevel.All;

    private static bool IsEnable(Constant.LogLevel level)
    {
        if (logLevel <= level)
            return true;
        return false;
    }

    #region Log
    public static void Log(object message)
    {
        if (IsEnable(Constant.LogLevel.All))
            Debug.Log(message);
    }

    public static void Log(object message, Object context)
    {
        if (IsEnable(Constant.LogLevel.All))
            Debug.Log(message, context);
    }

    public static void LogFormat(string format, params object[] args)
    {
        if (IsEnable(Constant.LogLevel.All))
            Debug.LogFormat(format, args);
    }

    public static void LogFormat(Object context, string format, params object[] args)
    {
        if (IsEnable(Constant.LogLevel.All))
            Debug.LogFormat(context, format, args);
    }
    #endregion

    #region LogWarning
    public static void LogWarning(object message)
    {
        if (IsEnable(Constant.LogLevel.All))
            Debug.LogWarning(message);
    }

    public static void LogWarning(object message, Object context)
    {
        if (IsEnable(Constant.LogLevel.All))
            Debug.LogWarning(message, context);
    }

    public static void LogWarningFormat(string format, params object[] args)
    {
        if (IsEnable(Constant.LogLevel.All))
            Debug.LogWarningFormat(format, args);
    }

    public static void LogWarningFormat(Object context, string format, params object[] args)
    {
        if (IsEnable(Constant.LogLevel.All))
            Debug.LogWarningFormat(context, format, args);
    }
    #endregion

    #region LogError
    public static void LogError(object message)
    {
        if (IsEnable(Constant.LogLevel.All))
            Debug.LogError(message);
    }

    public static void LogError(object message, Object context)
    {
        if (IsEnable(Constant.LogLevel.All))
            Debug.LogError(message, context);
    }

    public static void LogErrorFormat(string format, params object[] args)
    {
        if (IsEnable(Constant.LogLevel.All))
            Debug.LogErrorFormat(format, args);
    }

    public static void LogErrorFormat(Object context, string format, params object[] args)
    {
        if (IsEnable(Constant.LogLevel.All))
            Debug.LogErrorFormat(context, format, args);
    }
    #endregion
}
