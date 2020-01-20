using UnityEngine;

namespace Scripts.Utils
{
    public enum LogLevel {
        Debug,
        Warning,
        Error
    }

    public static class Record
    {
        public static void Log(string content, LogLevel level = LogLevel.Debug, bool use = false, bool unityLog = true)
        {
            if (unityLog) Debug.Log(content);
            if (!use) return;

        }
    }
}