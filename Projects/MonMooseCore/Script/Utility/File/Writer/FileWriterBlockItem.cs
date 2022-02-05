using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MonMooseCore
{
    public abstract class FileWriterBlockItem
    {
        private int m_tabIndex = 0;

        protected FileWriterBlockItem(int tabIndex)
        {
            m_tabIndex = tabIndex;
        }

        public virtual void Clear()
        {
            m_tabIndex = 0;
        }

        public abstract void GenerateResult(StringBuilder sb, string lineEndStr, string tabStr, int tabIndex);

        protected void AppendTab(StringBuilder sb, string tabStr, int tabIndex)
        {
            int tabCount = m_tabIndex + tabIndex;
            for (int i = 0; i < tabCount; ++i)
            {
                sb.Append(tabStr);
            }
        }
    }
}
