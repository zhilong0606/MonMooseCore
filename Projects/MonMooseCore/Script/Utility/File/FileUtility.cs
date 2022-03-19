using System.IO;

namespace MonMoose.Core
{
    public static class FileUtility
    {
        public static DirectoryInfo CreateDirectoryIfNotExist(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            return new DirectoryInfo(path);
        }
    }
}
