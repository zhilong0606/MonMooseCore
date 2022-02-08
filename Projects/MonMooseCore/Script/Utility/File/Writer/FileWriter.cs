using System.IO;
using System.Text;

namespace MonMooseCore
{
    public class FileWriter : FileWriterBlockItemBlock
    {
        private string m_lineEndStr = "\r\n";
        private string m_tabStr = "\t";

        public FileWriter()
            : base(0)
        {
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            GenerateResult(sb, m_lineEndStr, m_tabStr, 0);
            return sb.ToString();
        }

        public void WriteFile(DirectoryInfo dirInfo, string fileName)
        {
            using (StreamWriter file = new StreamWriter(FilePathUtility.GetPath(dirInfo.FullName, fileName), false, Encoding.GetEncoding("GB2312")))
            {
                file.Write(ToString());
            }
        }

        public void WriteFile(string fileName)
        {
            using (StreamWriter file = new StreamWriter(fileName, false, Encoding.GetEncoding("GB2312")))
            {
                file.Write(ToString());
            }
        }
    }
}
