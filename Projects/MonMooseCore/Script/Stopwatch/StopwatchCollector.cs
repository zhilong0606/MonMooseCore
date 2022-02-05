﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace MonMooseCore
{
    public class StopwatchCollector
    {
        private List<StopwatchTreeNode> m_nodeList = new List<StopwatchTreeNode>();
        private StopwatchTreeNode m_curNode;

        public void Start(string name)
        {
            StopwatchTreeNode node = null;
            if (m_curNode != null)
            {
                node = m_curNode.FindChild(name);
            }
            if (node == null)
            {
                node = new StopwatchTreeNode(name);
                if (m_curNode != null)
                {
                    m_curNode.AddChild(node);
                }
            }
            m_curNode = node;
            m_curNode.Start();
        }

        public void Stop()
        {
            if (m_curNode != null)
            {
                m_curNode.Stop();
                if (m_curNode.parent == null)
                {
                    m_nodeList.Add(m_curNode);
                }
                m_curNode = m_curNode.parent;
            }

        }

        public void LogOut(string path)
        {
            StringBuilder sb = new StringBuilder();
            foreach (StopwatchTreeNode node in m_nodeList)
            {
                node.LogOut(sb, new List<string>());
            }
            using (StreamWriter file = new StreamWriter(path, false, Encoding.UTF8))
            {
                file.Write(sb.ToString());
            }
        }
    }
}