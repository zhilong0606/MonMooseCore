using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace MonMooseCore
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
