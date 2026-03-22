using UnityEngine;

namespace NanBeiStudy.BaseDefense
{
    /// <summary>
    /// BaseDefense 专用分级日志系统 (BDLogger)
    /// 用于 AI 托管项目中的行为溯源与性能控制
    /// </summary>
    public static class BDLogger
    {
        public enum LogLevel
        {
            None = 0,
            Info = 1,     // 基础信息
            Warning = 2,  // 警告
            Error = 3,    // 错误
            Detail = 4    // 详细调试信息（开销最高）
        }

        // 全局日志开关
        public static LogLevel CurrentLevel = LogLevel.Info;

        public static void LogInfo(string message)
        {
            if (CurrentLevel >= LogLevel.Info)
                Debug.Log($"<color=cyan>[BD-INFO]</color> {message}");
        }

        public static void LogWarning(string message)
        {
            if (CurrentLevel >= LogLevel.Warning)
                Debug.LogWarning($"<color=yellow>[BD-WARN]</color> {message}");
        }

        public static void LogError(string message)
        {
            if (CurrentLevel >= LogLevel.Error)
                Debug.LogError($"<color=red>[BD-ERROR]</color> {message}");
        }

        public static void LogDetail(string message)
        {
            if (CurrentLevel >= LogLevel.Detail)
                Debug.Log($"<color=grey>[BD-DETAIL]</color> {message}");
        }
        
        // 用于 AI 验证的特殊标记日志
        public static void LogValidation(string tag, string value)
        {
             Debug.Log($"<color=green>[VALIDATION-RESULT]</color> Tag: {tag}, Value: {value}");
        }
    }
}
