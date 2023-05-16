using UnityEngine;

namespace MonMoose.Core
{
    public static class UIUtility
    {
        public static string GetTimeString(int sec, bool seperateDay = true, int exactShowCount = 0)
        {
            if (sec < 0)
            {
                sec = 0;
            }
            int minute = sec / 60;
            int hour = minute / 60;
            minute = minute % 60;
            sec = sec % 60;
            if (seperateDay && hour > 24)
            {
                return string.Format("{0}天", hour / 24);
            }
            if (hour > 0 || exactShowCount >= 3)
            {
                return string.Format("{0:D2}:{1:D2}:{2:D2}", hour, minute, sec);
            }
            if (minute > 0 || exactShowCount >= 2)
            {

                return string.Format("{0:D2}:{1:D2}", minute, sec);
            }
            if (sec > 0 || exactShowCount >= 1)
            {
                return string.Format("{0:D2}", sec);
            }
            return string.Empty;
        }
    }
}
