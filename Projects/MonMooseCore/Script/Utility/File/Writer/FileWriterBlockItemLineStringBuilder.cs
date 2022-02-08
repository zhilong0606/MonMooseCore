using System.Text;

namespace MonMooseCore
{
    public class FileWriterBlockItemLineStringBuilder : FileWriterBlockItem
    {
        private StringBuilder m_lineBuilder = new StringBuilder();

        public FileWriterBlockItemLineStringBuilder(int tabIndex)
            : base(tabIndex)
        {
        }

        public override void Clear()
        {
            m_lineBuilder.Length = 0;
            base.Clear();
        }

        public void Append(string str)
        {
            m_lineBuilder.Append(str);
        }

        public void AppendSpace()
        {
            m_lineBuilder.Append(" ");
        }

        public override void GenerateResult(StringBuilder sb, string lineEndStr, string tabStr, int tabIndex)
        {
            AppendTab(sb, tabStr, tabIndex);
            if (m_lineBuilder.Length > 0)
            {
                sb.Append(m_lineBuilder.ToString());
            }
            sb.Append(lineEndStr);
        }
    }
}
