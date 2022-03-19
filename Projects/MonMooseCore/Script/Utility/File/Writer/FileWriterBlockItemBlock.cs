using System.Collections.Generic;
using System.Text;

namespace MonMoose.Core
{
    public class FileWriterBlockItemBlock : FileWriterBlockItem
    {
        private List<FileWriterBlockItem> m_itemList = new List<FileWriterBlockItem>();
        private FileWriterBlockItem m_curItem;
        private int m_curTabIndex;

        public FileWriterBlockItemBlock(int tabIndex)
            : base(tabIndex)
        {
        }

        public override void GenerateResult(StringBuilder sb, string lineEndStr, string tabStr, int tabIndex)
        {
            if (m_curItem != null)
            {
                throw new System.Exception(string.Format("[{0}] CurLine is not Ended", this.GetType().Name));
            }
            if (m_curTabIndex != 0)
            {
                throw new System.Exception(string.Format("[{0}] Cannot End the Line Tab Index is Not Zero, Cur Tab Index is {1}", this.GetType().ToString(), m_curTabIndex));
            }
            int itemCount = m_itemList.Count;
            for (int i = 0; i < itemCount; ++i)
            {
                FileWriterBlockItem item = m_itemList[i];
                item.GenerateResult(sb, lineEndStr, tabStr, tabIndex);
            }
        }

        public override void Clear()
        {
            m_itemList.Clear();
            m_curItem = null;
            m_curTabIndex = 0;
            base.Clear();
        }

        public void StartTab()
        {
            m_curTabIndex++;
        }

        public void EndTab()
        {
            m_curTabIndex--;
        }

        public FileWriterBlockItemBlock StartLine()
        {
            if (m_curItem != null)
            {
                throw new System.Exception(string.Format("[{0}] CurLine is not Ended", this.GetType().Name));
            }
            m_curItem = new FileWriterBlockItemLineStringBuilder(m_curTabIndex);
            m_itemList.Add(m_curItem);
            return this;
        }

        public FileWriterBlockItemBlock EndLine()
        {
            if (m_curItem == null)
            {
                throw new System.Exception(string.Format("[{0}] CurLine is Empty, Need Start Line First", this.GetType().Name));
            }
            m_curItem = null;
            return this;
        }

        public FileWriterBlockItemBlock Append(string str, params object[] prms)
        {
            if (m_curItem == null)
            {
                throw new System.Exception(string.Format("[{0}] CurLine is Empty, Need Start Line First", this.GetType().Name));
            }
            FileWriterBlockItemLineStringBuilder lineItemStringBuilder = m_curItem as FileWriterBlockItemLineStringBuilder;
            if (lineItemStringBuilder == null)
            {
                throw new System.Exception(string.Format("[{0}] CurLine is Not StringBuilder, Cannot Append", this.GetType().Name));
            }
            if (!string.IsNullOrEmpty(str))
            {
                string lineStr = string.Empty;
                if (prms != null && prms.Length > 0)
                {
                    lineStr = string.Format(str, prms);
                }
                else
                {
                    lineStr = str;
                }
                lineItemStringBuilder.Append(lineStr);
            }
            return this;
        }

        public FileWriterBlockItemBlock AppendSpace()
        {
            Append(" ");
            return this;
        }

        public void AppendLine(string str = null, params object[] prms)
        {
            if (m_curItem != null)
            {
                throw new System.Exception(string.Format("[{0}] CurLine is not Ended", this.GetType().Name));
            }
            string lineStr = string.Empty;
            if (!string.IsNullOrEmpty(str))
            {
                if (prms != null && prms.Length > 0)
                {
                    lineStr = string.Format(str, prms);
                }
                else
                {
                    lineStr = str;
                }
            }
            m_itemList.Add(new FileWriterBlockItemLineString(lineStr, m_curTabIndex));
        }

        public FileWriterBlockItemBlock CreateAndInsertBlockItem()
        {
            if (m_curItem != null)
            {
                throw new System.Exception(string.Format("[{0}] CurLine is not Ended", this.GetType().Name));
            }
            FileWriterBlockItemBlock blockItem = new FileWriterBlockItemBlock(m_curTabIndex);
            m_itemList.Add(blockItem);
            return blockItem;
        }

        public void StartBlock()
        {
            AppendLine("{");
            StartTab();
        }

        public void EndBlock()
        {
            EndTab();
            AppendLine("}");
        }
    }
}
