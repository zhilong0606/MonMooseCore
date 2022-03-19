using System.Collections.Generic;

namespace MonMoose.Core
{
    public static partial class CSharpExtension
    {
        public static string[] SplitSafely(this string str, IList<char> separatorList, IList<char> openCharList, IList<char> closeCharList, bool skipEmpty, bool needTrim)
        {
            List<string> split = new List<string>();
            if (!string.IsNullOrEmpty(str))
            {
                List<int> stack = new List<int>();
                int startIndex = 0;
                for (int i = 0; i < str.Length; ++i)
                {
                    char ch = str[i];
                    int splitLength = -1;
                    if (separatorList.Contains(ch) && stack.Count == 0)
                    {
                        splitLength = i - startIndex;
                    }
                    else if (i == str.Length - 1)
                    {
                        splitLength = i - startIndex + 1;
                    }
                    if (splitLength >= 0)
                    {
                        string sp = string.Empty;
                        if (splitLength != 0)
                        {
                            sp = str.Substring(startIndex, splitLength);
                            if (needTrim)
                            {
                                sp = sp.TrimSafely();
                            }
                        }
                        if (!string.IsNullOrEmpty(sp) || !skipEmpty)
                        {
                            split.Add(sp);
                        }
                        startIndex = i + 1;
                        continue;
                    }
                    int openId = openCharList != null ? openCharList.IndexOf(ch) : -1;
                    if (openId >= 0)
                    {
                        stack.Add(openId);
                    }
                    int closeId = closeCharList != null ? closeCharList.IndexOf(ch) : -1;
                    if (closeId >= 0 && stack.Count > 0 && stack[stack.Count - 1] == closeId)
                    {
                        stack.RemoveAt(stack.Count - 1);
                    }
                }
            }
            return split.ToArray();
        }

        public static string ReplaceSafely(this string str, char ch1, char ch2)
        {
            if (!string.IsNullOrEmpty(str))
            {
                if (str.Contains(ch1))
                {
                    return str.Replace(ch1, ch2);
                }
                return str;
            }
            return string.Empty;
        }

        public static string ReplaceSafely(this string str, string str1, string str2)
        {
            if (!string.IsNullOrEmpty(str))
            {
                if (str.Contains(str1))
                {
                    return str.Replace(str1, str2);
                }
                return str;
            }
            return string.Empty;
        }

        public static string TrimSafely(this string str)
        {
            if (str != null)
            {
                return str.Trim();
            }
            return string.Empty;
        }

        public static string TrimSafely(this string str, char ch)
        {
            if (str != null)
            {
                return str.Trim(ch);
            }
            return string.Empty;
        }
    }
}