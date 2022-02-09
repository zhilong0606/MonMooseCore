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

        private const char m_rightDownSlashChar = '\\';
        private const string m_rightDownSlashStr = "\\";

        private const char m_rightUpSlashChar = '/';
        private const string m_rightUpSlashStr = "/";

        public static string GetFileNameWithoutExtension(string name)
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
                int slashIndex = Math.Max(name.LastIndexOf(m_rightUpSlashChar), name.LastIndexOf(m_rightDownSlashChar));
                if (slashIndex >= 0)
                {
                    return name.Substring(slashIndex, name.Length - slashIndex);
                }
            }
            return name;
        }

        public static bool TryGetAbsolutePath(string relativePath, out string absolutePath)
        {
            return TryGetAbsolutePath(EFilePathSlashType.RightUp, relativePath, out absolutePath);
        }

        public static bool TryGetAbsolutePath(EFilePathSlashType slashType, string relativePath, out string absolutePath)
        {
            absolutePath = string.Empty;
            if (string.IsNullOrEmpty(relativePath) || relativePath[0] != m_dotChar)
            {
                return false;
            }
            relativePath = NormalizeFolderPath(slashType, relativePath);
            string curFolderPath = NormalizeFolderPath(slashType, Directory.GetCurrentDirectory());
            if (string.IsNullOrEmpty(curFolderPath) || curFolderPath[0] == m_dotChar)
            {
                return false;
            }
            char slashChar = GetFilePathSlashChar(slashType);
            string[] curFolderNames = curFolderPath.Split(slashChar);
            if (curFolderNames.Length == 0)
            {
                return false;
            }
            int startRelativeIndex = 0;
            string[] relativePathSplits = relativePath.Split(slashChar);
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
                sb.Append(slashChar);
            }
            for (int i = endRelativeIndex + 1; i < relativePathSplits.Length; i++)
            {
                string folderName = relativePathSplits[i];
                if (!string.IsNullOrEmpty(folderName))
                {
                    sb.Append(relativePathSplits[i]);
                    if (i < relativePathSplits.Length - 1)
                    {
                        sb.Append(slashChar);
                    }
                }
            }
            absolutePath = sb.ToString();
            return true;
        }

        public static bool TryGetRelativePath(string absolutePath, out string relativePath)
        {
            return TryGetRelativePath(EFilePathSlashType.RightUp, absolutePath, out relativePath);
        }

        public static bool TryGetRelativePath(EFilePathSlashType slashType, string absolutePath, out string relativePath)
        {
            relativePath = string.Empty;
            if (string.IsNullOrEmpty(absolutePath) || absolutePath[0] == m_dotChar)
            {
                return false;
            }
            absolutePath = NormalizeFolderPath(slashType, absolutePath);
            string curFolderPath = NormalizeFolderPath(slashType, Directory.GetCurrentDirectory());
            if (string.IsNullOrEmpty(curFolderPath) || curFolderPath[0] == m_dotChar)
            {
                return false;
            }
            char slashChar = GetFilePathSlashChar(slashType);
            string[] curFolderNames = curFolderPath.Split(slashChar);
            if (curFolderNames.Length == 0)
            {
                return false;
            }
            string[] absolutePathSplits = absolutePath.Split(slashChar);
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
            sb.Append(slashChar);
            for (int i = startIndex; i < curFolderNames.Length; ++i)
            {
                sb.Append(m_doubleDotStr);
                if (i < curFolderNames.Length - 1)
                {
                    sb.Append(slashChar);
                }
            }
            for (int i = startIndex; i < absolutePathSplits.Length; ++i)
            {
                sb.Append(slashChar);
                sb.Append(absolutePathSplits[i]);
            }
            relativePath = sb.ToString();
            return true;
        }

        public static string GetPath(string folderPath, params string[] names)
        {
            return GetPath(EFilePathSlashType.RightUp, folderPath, names);
        }

        public static string GetPath(EFilePathSlashType slashType, string folderPath, params string[] names)
        {
            StringBuilder sb = new StringBuilder();
            folderPath = NormalizeFolderPath(slashType, folderPath);
            sb.Append(folderPath);
            bool isFolderPathEmpty = string.IsNullOrEmpty(folderPath);
            int count = names.Length;
            char slashChar = GetFilePathSlashChar(slashType);
            for (int i = 0; i < count; ++i)
            {
                string name = NormalizeFolderPath(slashType, names[i]);
                if (!string.IsNullOrEmpty(name))
                {
                    if (!isFolderPathEmpty || i != 0)
                    {
                        sb.Append(slashChar);
                    }
                    sb.Append(name);
                }
            }
            return sb.ToString();
        }

        private static string NormalizePath(EFilePathSlashType slashType, string path)
        {
            if (!string.IsNullOrEmpty(path))
            {
                char otherSlashChar = GetFilePathSlashChar(GetOtherSlashType(slashType));
                if (path.Contains(otherSlashChar))
                {
                    return path.Replace(otherSlashChar, GetFilePathSlashChar(slashType));
                }
            }
            return path;
        }

        private static string NormalizeFolderPath(EFilePathSlashType slashType, string path)
        {
            if (!string.IsNullOrEmpty(path))
            {
                path = NormalizePath(slashType, path);

                if (path.EndsWith(GetFilePathSlashStr(slashType)))
                {
                    return path.TrimEnd(GetFilePathSlashChar(slashType));
                }
            }
            return path;
        }

        private static string GetFilePathSlashStr(EFilePathSlashType slashType)
        {
            switch (slashType)
            {
                case EFilePathSlashType.RightDown:
                    return m_rightDownSlashStr;
                case EFilePathSlashType.RightUp:
                    return m_rightUpSlashStr;
            }
            return string.Empty;
        }

        private static char GetFilePathSlashChar(EFilePathSlashType slashType)
        {
            switch (slashType)
            {
                case EFilePathSlashType.RightDown:
                    return m_rightDownSlashChar;
                case EFilePathSlashType.RightUp:
                    return m_rightUpSlashChar;
            }
            return '\0';
        }

        private static EFilePathSlashType GetOtherSlashType(EFilePathSlashType slashType)
        {
            switch (slashType)
            {
                case EFilePathSlashType.RightDown:
                    return EFilePathSlashType.RightUp;
                case EFilePathSlashType.RightUp:
                    return EFilePathSlashType.RightDown;
            }
            throw new Exception();
        }
    }

    public enum EFilePathSlashType
    {
        RightUp,
        RightDown,
    }
}