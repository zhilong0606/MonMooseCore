using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace MonMooseCore
{
    public class StopwatchTreeNode
    {
        private Stopwatch m_stopwatch = new Stopwatch();
        private string m_name;
        private StopwatchTreeNode m_parent;
        private List<StopwatchTreeNode> m_childList = new List<StopwatchTreeNode>();

        private const string m_tabStr = "  ";
        private const string m_foldStr = "┣ ";
        private const string m_foldLastStr = "┗ ";
        private const string m_foldLinkStr = "┃ ";
        //ffff
        //  ┣ dddd
        //  ┃   ┣ eeee
        //  ┃   ┃   ┗ rrrrr
        //  ┃   ┗ mmmmm
        //  ┣ ewww
        //  ┗ dffff
        //       ┣ ddddd
        //       ┗ ggggg

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
            m_childList.Add(child);
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
            for (int i = 0; i < markList.Count; ++i)
            {
                sb.Append(m_tabStr);
                sb.Append(markList[i]);
            }
            if (m_parent != null)
            {
                sb.Append(m_tabStr);
                sb.Append(m_foldStr);
            }
            sb.Append("(").Append(m_stopwatch.ElapsedMilliseconds / 1000f).Append(")").Append(" ").Append(m_name).Append("\r\n");
            //sb.Append(string.Format("({0}) {1}\r\n", (m_stopwatch.ElapsedMilliseconds / 1000f),ToString(), m_name));
            for (int i = 0; i < m_childList.Count; ++i)
            {
                List<string> list = new List<string>(markList);
                StopwatchTreeNode child = m_childList[i];
                if (i == m_childList.Count - 1)
                {
                    list.Add(m_foldLastStr);
                }
                else
                {
                    list.Add(m_foldLinkStr);
                }
                child.LogOut(sb, list);
            }
        }
    }
}
