using System;
using System.Collections.Generic;

namespace MonMoose.Core
{
    public class PriorityInvoker<T>
    {
        private List<Node> m_nodeList = new List<Node>();

        public void Set(int priority, T obj)
        {
            int index = IndexOf(priority);
            Node newNode = new Node(priority, obj);
            if (index >= 0)
            {
                m_nodeList[index] = newNode;
                return;
            }
            for (int i = 0; i < m_nodeList.Count; ++i)
            {
                if (priority < m_nodeList[i].priority)
                {
                    m_nodeList.Insert(i, newNode);
                    return;
                }
            }
            m_nodeList.Add(newNode);
        }

        public void Invoke(Action<T> actionOnInvoke)
        {
            if (actionOnInvoke == null)
            {
                return;
            }
            foreach (Node node in m_nodeList)
            {
                try
                {
                    actionOnInvoke(node.obj);
                }
                catch (Exception e)
                {
                    DebugUtility.LogError(e.Message + "\r\n" + e.StackTrace);
                }
            }
        }

        private int IndexOf(int priority)
        {
            for (int i = 0; i < m_nodeList.Count; ++i)
            {
                if (m_nodeList[i].priority == priority)
                {
                    return i;
                }
            }
            return -1;
        }

        private struct Node
        {
            public int priority;
            public T obj;

            public Node(int priority, T obj)
            {
                this.priority = priority;
                this.obj = obj;
            }
        }
    }
}