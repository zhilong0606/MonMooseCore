using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MonMooseCore
{
    public class FileWriterBlockItemLineString : FileWriterBlockItem
    {
        private string m_str;

        public FileWriterBlockItemLineString(string lineStr, int tabIndex)
            : base(tabIndex)
        {
            m_str = lineStr;
        }

        public override void Clear()
        {
            m_str = null;
            base.Clear();
        }

        public override void GenerateResult(StringBuilder sb, string lineEndStr, string tabStr, int tabIndex)
        {
            AppendTab(sb, tabStr, tabIndex);
            if (!string.IsNullOrEmpty(m_str))
            {
                sb.Append(m_str);
            }
            sb.Append(lineEndStr);
        }
    }
}
