using System;
using System.IO;
using System.Text;

namespace MonMooseCore
{
    public static class FilePathUtility
    {
        private const string m_doubleDotStr = "..";

        private const char m_dotChar = '.';
        private const string m_dotStr = ".";

        private const char m_unNormalizedSlashChar = '\\';
        private const string m_unNormalizedSlashStr = "\\";

        private const char m_normalizedSlashChar = '/';
        private const string m_normalizedSlashStr = "/";

        public static string GetFileNameWithoutExtensions(string name)
        {
            if (!string.IsNullOrEmpty(name))
            {
                name = GetFileName(name);
                int dotIndex = name.LastIndexOf(m_dotChar);
                if (dotIndex >= 0)
                {
                    return name.Substring(0, dotIndex);
                }
            }
            return name;
        }

        public static string GetFileName(string name)
        {
            if (!string.IsNullOrEmpty(name))
            {
                name = NormalizePath(name);
                int slashIndex = name.LastIndexOf(m_normalizedSlashChar);
                if (slashIndex >= 0)
                {
                    return name.Substring(slashIndex, name.Length - slashIndex);
                }
            }
            return name;
        }

        public static bool TryGetAbsolutePath(string relativePath, out string absolutePath)
        {
            absolutePath = string.Empty;
            if (string.IsNullOrEmpty(relativePath) || relativePath[0] != m_dotChar)
            {
                return false;
            }
            relativePath = NormalizePath(relativePath);
            string curFolderPath = NormalizeFolderPath(Directory.GetCurrentDirectory());
            if (string.IsNullOrEmpty(curFolderPath) || curFolderPath[0] == m_dotChar)
            {
                return false;
            }
            string[] curFolderNames = curFolderPath.Split(m_normalizedSlashChar);
            if (curFolderNames.Length == 0)
            {
                return false;
            }
            int startRelativeIndex = 0;
            string[] relativePathSplits = relativePath.Split(m_normalizedSlashChar);
            if (relativePathSplits.Length > 0 && relativePathSplits[0] == m_dotStr)
            {
                startRelativeIndex++;
            }
            int endRelativeIndex = -1;
            for (int i = startRelativeIndex; i < relativePathSplits.Length; i++)
            {
                if (relativePathSplits[i] != m_doubleDotStr)
                {
                    endRelativeIndex = i - 1;
                    break;
                }
            }
            if (endRelativeIndex < 0)
            {
                endRelativeIndex = relativePathSplits.Length - 1;
            }
            int needRelativeDepth = endRelativeIndex - startRelativeIndex + 1;
            if (curFolderNames.Length - needRelativeDepth <= 0)
            {
                return false;
            }
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < curFolderNames.Length - needRelativeDepth; i++)
            {
                sb.Append(curFolderNames[i]);
                sb.Append(m_normalizedSlashChar);
            }
            for (int i = endRelativeIndex + 1; i < relativePathSplits.Length; i++)
            {
                string folderName = relativePathSplits[i];
                if (!string.IsNullOrEmpty(folderName))
                {
                    sb.Append(relativePathSplits[i]);
                    if (i < relativePathSplits.Length - 1)
                    {
                        sb.Append(m_normalizedSlashChar);
                    }
                }
            }
            absolutePath = sb.ToString();
            return true;
        }

        public static bool TryGetRelativePath(string absolutePath, out string relativePath)
        {
            relativePath = string.Empty;
            if (string.IsNullOrEmpty(absolutePath) || absolutePath[0] == m_dotChar)
            {
                return false;
            }
            absolutePath = NormalizePath(absolutePath);
            string curFolderPath = NormalizeFolderPath(Directory.GetCurrentDirectory());
            if (string.IsNullOrEmpty(curFolderPath) || curFolderPath[0] == m_dotChar)
            {
                return false;
            }
            string[] curFolderNames = curFolderPath.Split(m_normalizedSlashChar);
            if (curFolderNames.Length == 0)
            {
                return false;
            }
            string[] absolutePathSplits = absolutePath.Split(m_normalizedSlashChar);
            if (absolutePathSplits.Length == 0)
            {
                return false;
            }
            int startIndex = -1;
            int count = Math.Min(absolutePathSplits.Length, curFolderNames.Length);
            for (int i = 0; i < count; i++)
            {
                if (absolutePathSplits[i] != curFolderNames[i])
                {
                    startIndex = i;
                    break;
                }
            }
            if (startIndex < 0)
            {
                startIndex = count + 1;
            }
            StringBuilder sb = new StringBuilder();
            sb.Append(m_dotChar);
            sb.Append(m_normalizedSlashChar);
            for (int i = startIndex; i < curFolderNames.Length; ++i)
            {
                sb.Append(m_doubleDotStr);
                if (i < curFolderNames.Length - 1)
                {
                    sb.Append(m_normalizedSlashChar);
                }
            }
            for (int i = startIndex; i < absolutePathSplits.Length; ++i)
            {
                sb.Append(m_normalizedSlashChar);
                sb.Append(absolutePathSplits[i]);
            }
            relativePath = sb.ToString();
            return true;
        }

        public static string NormalizePath(string path)
        {
            if (!string.IsNullOrEmpty(path))
            {
                if (path.Contains(m_unNormalizedSlashChar))
                {
                    return path.Replace(m_unNormalizedSlashChar, m_normalizedSlashChar);
                }
            }
            return path;
        }

        public static string NormalizeFolderPath(string path)
        {
            if (!string.IsNullOrEmpty(path))
            {
                path = NormalizePath(path);
                if (path.EndsWith(m_normalizedSlashStr))
                {
                    return path.TrimEnd(m_normalizedSlashChar);
                }
            }
            return path;
        }

        public static string GetPath(string folderPath, params string[] names)
        {
            StringBuilder sb = new StringBuilder();
            folderPath = NormalizeFolderPath(folderPath);
            sb.Append(folderPath);
            bool isFolderPathEmpty = string.IsNullOrEmpty(folderPath);
            int count = names.Length;
            for (int i = 0; i < count; ++i)
            {
                string name = NormalizeFolderPath(names[i]);
                if (!string.IsNullOrEmpty(name))
                {
                    if (!isFolderPathEmpty || i != 0)
                    {
                        sb.Append(m_normalizedSlashStr);
                    }
                    sb.Append(name);
                }
            }
            return sb.ToString();
        }
    }
}