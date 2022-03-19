using System;

namespace MonMoose.Core
{
    public static class DebugUtility
    {
        public static Action<string> actionOnLog;
        public static Action<string> actionOnLogWarning;
        public static Action<string> actionOnLogError;

        public static void Log(string str)
        {
            if (actionOnLog != null)
            {
                actionOnLog(str);
            }
        }

        public static void LogWarning(string str)
        {
            if (actionOnLogWarning != null)
            {
                actionOnLogWarning(str);
            }
        }

        public static void LogError(string str)
        {
            if (actionOnLogError != null)
            {
                actionOnLogError(str);
            }
        }
    }
}
