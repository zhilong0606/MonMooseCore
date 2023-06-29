using System;
using System.Collections;
using System.Collections.Generic;

namespace MonMoose.Core
{
    public class StateTree
    {
        private TreeNode m_rootNode;
        private TreeNode m_curNode;
        private List<TreeNode> m_tempList = new List<TreeNode>();

        public TreeNode rootTreeNode
        {
            get { return m_rootNode; }
        }

        public TreeNode curNode
        {
            get { return m_curNode; }
        }

        public void Init(TreeNode rootNode)
        {
            m_rootNode = rootNode;
        }

        public void Clear()
        {
            if (m_rootNode != null)
            {
                m_rootNode.Release();
                m_rootNode = null;
            }
            m_curNode = null;
        }

        public void AddChildToCurNode(TreeNode node)
        {
            if (m_curNode == null)
            {
                DebugUtility.LogError("AddChildToCurNode:m_curNode is null");
                return;
            }
            m_curNode.AddChild(node);
        }

        public bool MoveToNext()
        {
            if (m_curNode == null)
            {
                m_curNode = m_rootNode;
                return true;
            }
            if (m_curNode.firstNode != null)
            {
                m_curNode = m_curNode.firstNode;
                return true;
            }
            TreeNode node = m_curNode;
            while (true)
            {
                if (node.parentNode == null)
                {
                    node = null;
                    break;
                }
                int index = node.parentNode.GetIndexOfChildNode(node);
                TreeNode childNode = node.parentNode.GetChildNode(index + 1);
                if (childNode != null)
                {
                    node = childNode;
                    break;
                }
                node = node.parentNode;
                if (m_tempList.Contains(node))
                {
                    DebugUtility.LogError("Duplicated TreeNode");
                    node = null;
                    break;
                }
                m_tempList.Add(node);
            }
            m_tempList.Clear();
            m_curNode = node;
            return m_curNode != null;
        }


    }
}
