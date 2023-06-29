using System;
using System.Collections;
using System.Collections.Generic;

namespace MonMoose.Core
{
    public class TreeNode : ClassPoolObj
    {
        private TreeNode m_parent;
        private List<TreeNode> m_childList = new List<TreeNode>();

        public TreeNode firstNode
        {
            get { return m_childList.GetValueSafely(0); }
        }

        public TreeNode parentNode
        {
            get { return m_parent; }
        }

        public override void OnRelease()
        {
            base.OnRelease();
            m_parent = null;
            m_childList.ReleaseAll();
        }

        public void AddChild(TreeNode node)
        {
            if (node == null)
            {
                return;
            }
            if (node.m_parent != null)
            {
                DebugUtility.LogError("TreeNode already has a parent");
                return;
            }
            if (m_childList.Contains(node))
            {
                DebugUtility.LogError("TreeNode already is child");
                return;
            }
            node.m_parent = this;
            m_childList.Add(node);
            OnAddChild(node);
        }

        public int GetIndexOfChildNode(TreeNode node)
        {
            return m_childList.IndexOf(node);
        }

        public TreeNode GetChildNode(int index)
        {
            return m_childList.GetValueSafely(index);
        }

        public int GetDepth()
        {
            TreeNode node = this;
            int depth = 0;
            while (node.m_parent != null)
            {
                depth++;
                node = node.m_parent;
            }
            return depth;
        }

        protected virtual void OnAddChild(TreeNode node) { }
    }
}
