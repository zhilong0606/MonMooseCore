using System;
using System.Collections;
using System.Collections.Generic;

namespace MonMoose.Core
{
    public class CodeNameUtility
    {
        public static string GetPrivateFiledName(string nameStr)
        {
            if (string.IsNullOrEmpty(nameStr))
            {
                return string.Empty;
            }
            if (nameStr.StartsWith("m_"))
            {
                return nameStr;
            }
            if (nameStr.Length > 0)
            {
                return "m_" + nameStr[0].ToString().ToLower() + nameStr.Substring(1);
            }
            return nameStr;
        }

        public static string GetPublicFiledName(string nameStr)
        {
            if (string.IsNullOrEmpty(nameStr))
            {
                return string.Empty;
            }
            if (nameStr.StartsWith("m_"))
            {
                nameStr = nameStr.Substring(2);
            }
            if (nameStr.Length > 0)
            {
                return nameStr[0].ToString().ToLower() + nameStr.Substring(1);
            }
            return nameStr;
        }
    }
}
