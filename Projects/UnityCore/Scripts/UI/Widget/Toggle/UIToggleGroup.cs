using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MonMoose.Core
{
    public class UIToggleGroup
    {
        public bool canToggleSame;
        public bool canToggleClickOff;
        public bool autoRefresh = true;
        public Action<int> actionOnSelectionChanged;

        private List<IToggleHandler> m_toggleList = new List<IToggleHandler>();
        private int m_selectedIndex = -1;
        private int m_nextIndex = -1;

        public int selectedIndex
        {
            get { return m_selectedIndex; }
        }

        public IToggleHandler this[int index]
        {
            get { return m_toggleList.GetValueSafely(index); }
        }

        public void UpdateToggleList(IList list)
        {
            m_toggleList.Clear();
            for (int i = 0; i < list.Count; ++i)
            {
                IToggleHandler toggle = list[i] as IToggleHandler;
                if (toggle != null)
                {
                    AddToggle(toggle);
                }
            }
        }

        public void AddToggle(IToggleHandler toggle)
        {
            if (!m_toggleList.Contains(toggle))
            {
                toggle.SetToggle(false, true);
                toggle.toggleGroup = this;
                m_toggleList.Add(toggle);
            }
        }

        public void ClearToggle()
        {
            m_toggleList.Clear();
        }

        public void Select(IToggleHandler toggle, bool forceHandleSame = false)
        {
            int index = m_toggleList.IndexOf(toggle);
            SelectInterval(index, forceHandleSame, true);
        }

        public void Select(int index, bool forceHandleSame = false)
        {
            SelectInterval(index, forceHandleSame, true);
        }

        public void Refresh()
        {
            m_selectedIndex = m_nextIndex;
            for (int i = 0; i < m_toggleList.Count; ++i)
            {
                ChangeToggle(i, i == m_selectedIndex);
            }
        }

        public void OnToggleClicked(IToggleHandler toggle)
        {
            int index = m_toggleList.IndexOf(toggle);
            SelectInterval(index, false, autoRefresh);
        }

        public void SelectInterval(int index, bool forceHandleSame, bool handleRefresh)
        {
            if (index < 0)
            {
                index = 0;
            }
            if (index >= m_toggleList.Count)
            {
                index = m_toggleList.Count - 1;
            }
            if (index == m_selectedIndex)
            {
                if(!canToggleClickOff && !canToggleSame && !forceHandleSame)
                {
                    return;
                }
            }
            m_nextIndex = index;
            if (canToggleClickOff)
            {
                if(index == m_selectedIndex)
                {
                    m_nextIndex = -1;
                }
            }
            if (actionOnSelectionChanged != null)
            {
                actionOnSelectionChanged(m_nextIndex);
            }
            if (handleRefresh)
            {
                Refresh();
            }
        }

        private void ChangeToggle(int index, bool flag)
        {
            if (index >= 0 && index < m_toggleList.Count)
            {
                m_toggleList[index].SetToggle(flag, false);
            }
        }
    }
}
