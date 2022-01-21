using System;
using System.IO;
using System.Text;

namespace MonMooseCore
{
    public static class FilePathUtility
    {
        private static readonly String[] POSITION_SPLIT = {@"/", @"\"};
        private const String POSITION_DELIMITER_WIN = @"\";
        private const string POSITION_DELIMITER = "/";
        private const String STRING_COLON = @":";
        private const string STRING_POINT = ".";
        private const string STRING_TWO_POINTS = "..";

        public static string GetFileNameExceptExtensions(string name)
        {
            if (!string.IsNullOrEmpty(name))
            {
                int dotIndex = name.LastIndexOf(STRING_POINT, StringComparison.Ordinal);
                int slashIndex = name.LastIndexOf("/", StringComparison.Ordinal);
                if (slashIndex < 0)
                {
                    slashIndex = name.LastIndexOf("\\", StringComparison.Ordinal);
                }
                if (dotIndex >= 0)
                {
                    if (dotIndex > slashIndex)
                    {
                        return name.Substring(0, dotIndex);
                    }
                }
                else if (slashIndex >= 0)
                {
                    return name.Substring(slashIndex, name.Length - slashIndex);
                }
            }
            return name;
        }

        public static Boolean TryGetAbsolutePosition(String relative, out String absolute)
        {
            string current = Directory.GetCurrentDirectory();
            absolute = String.Empty;
            if (String.IsNullOrEmpty(current))
                return false;
            if (String.IsNullOrEmpty(relative))
                return false;
            String subString = current.Substring(0, 1);
            if (subString == STRING_POINT)
                return false;
            String[] currents = current.Split(POSITION_SPLIT, StringSplitOptions.RemoveEmptyEntries);
            if (currents.Length < 2)
                return false;
            subString = relative.Substring(0, 1);
            if (subString != STRING_POINT)
            {
                String second = relative.Substring(1, 1);
                if (second == STRING_COLON)
                {
                    absolute = relative;
                    return true;
                }
                else
                {
                    absolute = relative;
                    return true;
                }
            }
            String[] relatives = relative.Split(POSITION_SPLIT, StringSplitOptions.RemoveEmptyEntries);
            if (relatives.Length < 2)
                return false;
            Int32 relativeCount = 0;
            for (Int32 i = 0; i < relatives.Length; i++)
            {
                subString = relatives[i].Substring(0, 1);
                if (subString != STRING_POINT)
                {
                    relativeCount = i;
                    break;
                }
            }
            if (relativeCount > currents.Length)
                return false;
            for (Int32 i = 0; i < currents.Length - relativeCount + 1; i++)
                absolute += String.Format("{0}{1}", currents[i], POSITION_DELIMITER_WIN);
            for (Int32 i = relativeCount; i < relatives.Length; i++)
                absolute += String.Format("{0}{1}", relatives[i], POSITION_DELIMITER_WIN);
            return true;
        }

        public static Boolean TryGetRelativePosition(string absolute, out string relative)
        {
            string current = Directory.GetCurrentDirectory();
            relative = string.Empty;
            if (string.IsNullOrEmpty(current))
                return false;
            if (string.IsNullOrEmpty(absolute))
                return false;
            string subString = current.Substring(0, 1);
            if (subString == STRING_POINT)
                return false;
            string[] currents = current.Split(POSITION_SPLIT, StringSplitOptions.RemoveEmptyEntries);
            if (currents.Length < 2)
                return false;
            subString = absolute.Substring(0, 1);
            if (subString == STRING_POINT)
            {
                relative = absolute;
                return false;
            }
            string[] absolutes = absolute.Split(POSITION_SPLIT, StringSplitOptions.RemoveEmptyEntries);
            if (absolutes.Length < 2)
                return false;
            if (currents.Length <= absolutes.Length)
            {
                relative = GetRelativePosition(currents, absolutes, false);
            }
            else
            {
                relative = GetRelativePosition(absolutes, currents, true);
            }
            return true;
        }

        public static string GetRelativePosition(string[] positions1, string[] positions2, Boolean targetIs1)
        {
            string relative = string.Empty;
            Int32 i = 0;
            for (; i < positions1.Length; i++)
            {
                if (positions1[i].ToUpper() != positions2[i].ToUpper())
                {
                    break;
                }
            }
            if (targetIs1)
            {
                relative = SplicePosition(positions2.Length - i, i, positions1);
            }
            else
            {
                relative = SplicePosition(positions1.Length - i, i, positions2);
            }
            return relative;
        }

        public static string SplicePosition(Int32 count, Int32 start, string[] positions)
        {
            string position = string.Empty;
            for (Int32 i = 0; i < count; i++)
            {
                if (i == 0)
                {
                    position = string.Format("{0}{1}", STRING_POINT, POSITION_DELIMITER);
                }
                position += string.Format("{0}{1}", STRING_TWO_POINTS, POSITION_DELIMITER);
            }
            if (string.IsNullOrEmpty(position))
                position = @"./";
            for (Int32 i = start; i < positions.Length; i++)
            {
                position += string.Format("{0}{1}", positions[i], POSITION_DELIMITER);
            }
            return position;
        }

        public static string NormalizePath(string path)
        {
            if (path.Contains('\\'))
            {
                return path.Replace('\\', '/');
            }
            return path;
        }

        public static string NormalizeFolderPath(string path)
        {
            path = NormalizePath(path);
            if (path.EndsWith("/"))
            {
                return path.TrimEnd('/');
            }
            return path;
        }

        public static string GetPath(string folderPath, params string[] names)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(folderPath);
            folderPath = NormalizeFolderPath(folderPath);
            bool isFolderPathEmpty = string.IsNullOrEmpty(folderPath);
            int count = names.Length;
            for(int i = 0; i < count; ++i)
            {
                string name = NormalizeFolderPath(names[i]);
                if (!string.IsNullOrEmpty(name))
                {
                    if (!isFolderPathEmpty || i != 0)
                    {
                        sb.Append("/");
                    }
                    sb.Append(name);
                }
            }
            return sb.ToString();
        }
    }
}