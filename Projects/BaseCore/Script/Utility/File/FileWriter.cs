using System.IO;
using System.Text;

namespace MonMooseCore
{
    public class FileWriter
    {
        private StringBuilder m_stringBuilder = new StringBuilder();
        private int tabCount = 0;

        public void Clear()
        {
            tabCount = 0;
            m_stringBuilder.Length = 0;
        }

        public void StartTab()
        {
            tabCount++;
        }

        public void EndTab()
        {
            tabCount--;
        }

        public FileWriter StartLine(string str = null)
        {
            for (int i = 0; i < tabCount; ++i)
            {
                m_stringBuilder.Append("\t");
            }
            if (!string.IsNullOrEmpty(str))
            {
                m_stringBuilder.Append(str);
            }
            return this;
        }

        public FileWriter Append(string str)
        {
            m_stringBuilder.Append(str);
            return this;
        }

        public FileWriter AppendSpace()
        {
            m_stringBuilder.Append(" ");
            return this;
        }

        public FileWriter EndLine()
        {
            m_stringBuilder.Append("\r\n");
            return this;
        }

        public void AppendLine(string str = null)
        {
            if (!string.IsNullOrEmpty(str))
            {
                for (int i = 0; i < tabCount; ++i)
                {
                    m_stringBuilder.Append("\t");
                }
                m_stringBuilder.Append(str);
            }
            m_stringBuilder.Append("\r\n");
        }

        public void StartCodeBlock()
        {
            AppendLine("{");
            StartTab();
        }

        public void EndCodeBlock()
        {
            EndTab();
            AppendLine("}");
        }

        public override string ToString()
        {
            return m_stringBuilder.ToString();
        }

        public void WriteFile(DirectoryInfo dirInfo, string fileName)
        {
            using (StreamWriter file = new StreamWriter(dirInfo.FullName + "\\" + fileName, false, Encoding.GetEncoding("GB2312")))
            {
                file.Write(m_stringBuilder.ToString());
            }
        }

        public void WriteFile(string fileName)
        {
            using (StreamWriter file = new StreamWriter(fileName, false, Encoding.GetEncoding("GB2312")))
            {
                file.Write(m_stringBuilder.ToString());
            }
        }
    }
}
