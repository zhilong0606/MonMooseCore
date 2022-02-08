using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace MonMooseCore
{
    public class StopwatchTreeNode
    {
        private Stopwatch m_stopwatch = new Stopwatch();
        private string m_name;
        private StopwatchTreeNode m_parent;
        private List<StopwatchTreeNode> m_childList = new List<StopwatchTreeNode>();

        public StopwatchTreeNode parent
        {
            get { return m_parent; }
        }

        public StopwatchTreeNode(string name)
        {
            m_name = name;
        }

        public void AddChild(StopwatchTreeNode child)
        {
            if (m_childList.Contains(child))
            {
                return;
            }
            child.m_parent = this;
            m_childList.Add(child);
        }

        public StopwatchTreeNode FindChild(string name)
        {
            foreach (StopwatchTreeNode child in m_childList)
            {
                if (child.m_name == name)
                {
                    return child;
                }
            }
            return null;
        }

        public void Start()
        {
            m_stopwatch.Start();
        }

        public void Stop()
        {
            m_stopwatch.Stop();
        }

        public void LogOut(StringBuilder sb, List<string> markList)
        {
            for (int i = 0; i < markList.Count - 1; ++i)
            {
                sb.Append(StopwatchMark.tabStr);
                sb.Append(markList[i]);
            }
            if (m_parent != null)
            {
                sb.Append(StopwatchMark.tabStr);
                if (m_parent.m_childList.IndexOf(this) == m_parent.m_childList.Count - 1)
                {
                    sb.Append(StopwatchMark.foldLastStr);
                }
                else
                {
                    sb.Append(StopwatchMark.foldStr);
                }
            }
            sb.Append(string.Format("({0:N2}) {1}\r\n", m_stopwatch.ElapsedMilliseconds / 1000f, m_name));
            for (int i = 0; i < m_childList.Count; ++i)
            {
                List<string> list = new List<string>(markList);
                StopwatchTreeNode child = m_childList[i];
                if (i == m_childList.Count - 1)
                {
                    list.Add(StopwatchMark.tabStr);
                }
                else
                {
                    list.Add(StopwatchMark.foldLinkStr);
                }
                child.LogOut(sb, list);
            }
        }
    }
}
