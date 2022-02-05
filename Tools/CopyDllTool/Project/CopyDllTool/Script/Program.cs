using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CopyDllTool
{
    class Program
    {
        private const string m_dllFormat = "{0}/{1}.dll";
        private const string m_pdbFormat = "{0}/{1}.pdb";

        static void Main(string[] args)
        {
            Config.LoadConfig();
            Config config = Config.instance;
            string dllFolderPath = config.dllFolderPath;
            if (Directory.Exists(dllFolderPath))
            {
                foreach (var item in config.itemList)
                {
                    if (!item.enabled)
                    {
                        continue;
                    }
                    ExecuteCopy(m_dllFormat, item.dllName, config.dllFolderPath, item.destFolderPathList);
                    ExecuteCopy(m_pdbFormat, item.dllName, config.dllFolderPath, item.destFolderPathList);
                }
            }
        }

        private static void ExecuteCopy(string format, string fileName, string srcFolderPath, List<string> destFolderPathList)
        {
            string srcFilePath = GetFilePath(format, srcFolderPath, fileName);
            if (!File.Exists(srcFilePath))
            {
                return;
            }
            foreach (var destFolderPath in destFolderPathList)
            {
                if (!Directory.Exists(destFolderPath))
                {
                    continue;
                }
                string destFilePath = GetFilePath(format, destFolderPath, fileName);
                if (File.Exists(destFilePath))
                {
                    File.Delete(destFilePath);
                }
                File.Copy(srcFilePath, destFilePath);
            }
        }

        private static string GetFilePath(string fileFormat, string folderPath, string fileName)
        {
            folderPath = folderPath.Replace("\\", "/").TrimEnd('/');
            return string.Format(fileFormat, folderPath, fileName);
        }
    }
}
