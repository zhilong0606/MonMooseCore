using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace MonMoose.Core
{
	public class ReorderableListHandler<T> where T : class
    {
        public delegate void DelegateOnDrawItem(Rect rect, int index, bool active, bool focused);
        public delegate void DelegateOnDrawItemDetail(int index);
        public delegate T DelegateOnCreateInfo(object data);

        private ReorderableList m_reorderableList;
        private List<T> m_infoList;
        private DelegateOnCreateInfo m_funcOnCreateInfo;
        private DelegateOnDrawItemDetail m_actionOnDrawItemDetail;
        private Func<List<Tuple<string, object>>> m_funcOnGetAddItemMenuList;
        private DelegateOnDrawItem m_actionOnDrawItem;
        private Action m_actionOnUpdateInfo;

        public void DoLayoutList(List<T> infoList,
            bool draggable,
            DelegateOnDrawItem actionOnDrawItem,
            DelegateOnDrawItemDetail actionOnDrawItemDetail,
            DelegateOnCreateInfo funcOnCreateInfo,
            Func<List<Tuple<string, object>>> funcOnGetAddItemMenuList,
            Action actionOnUpdateInfo)
        {
            m_infoList = infoList;
            m_actionOnDrawItem = actionOnDrawItem;
            m_actionOnDrawItemDetail = actionOnDrawItemDetail;
            m_funcOnCreateInfo = funcOnCreateInfo;
            m_funcOnGetAddItemMenuList = funcOnGetAddItemMenuList;
            m_actionOnUpdateInfo = actionOnUpdateInfo;
            if (m_reorderableList == null)
            {
                m_reorderableList = new ReorderableList(m_infoList, typeof(T), draggable, false, true, true);
                m_reorderableList.drawElementCallback = OnItemDraw;
                m_reorderableList.onReorderCallback = OnReorderItem;
                m_reorderableList.onAddCallback = OnAddItem;
                m_reorderableList.onRemoveCallback = OnRemoveItem;
                m_reorderableList.onSelectCallback = OnSelectItem;
            }
            m_reorderableList.DoLayoutList();
            if (m_reorderableList.index >= 0 && m_reorderableList.index < m_infoList.Count)
            {
                DrawInfo(m_reorderableList.index);
            }
        }

        private void OnItemDraw(Rect rect, int index, bool isActive, bool isFocused)
        {
            if (m_actionOnDrawItem != null)
            {
                m_actionOnDrawItem(rect, index, isActive, isFocused);
            }
        }

        private void OnReorderItem(ReorderableList list)
        {
            UpdateInfo();
        }

        private void OnAddItem(ReorderableList list)
        {
            if (m_funcOnGetAddItemMenuList != null)
            {
                var menuItemList = m_funcOnGetAddItemMenuList();
                GenericMenu addMenu = new GenericMenu();
                foreach (var menuItem in menuItemList)
                {
                    addMenu.AddItem(new GUIContent(menuItem.Item1), false, AddItemByMenu, menuItem.Item2);
                }
                addMenu.ShowAsContext();
            }
            else
            {
                AddInfo(null);
            }
        }

        private void OnRemoveItem(ReorderableList list)
        {
            if (list.index >= 0 && list.index < m_infoList.Count)
            {
                m_infoList.RemoveAt(list.index);
                UpdateInfo();
            }
        }

        private void OnSelectItem(ReorderableList list)
        {
            int index = Mathf.Clamp(list.index, 0, m_infoList.Count);
            if(index != list.index)
            {
                list.index = index;
            }
        }

        private void DrawInfo(int index)
        {
            if (m_actionOnDrawItemDetail != null)
            {
                m_actionOnDrawItemDetail(index);
            }
        }

        private void UpdateInfo()
        {
            if (m_actionOnUpdateInfo != null)
            {
                m_actionOnUpdateInfo();
            }
        }

        private void AddItemByMenu(object userdata)
        {
            AddInfo(userdata);
        }

        private void AddInfo(object data)
        {
            if (m_funcOnCreateInfo != null)
            {
                T item = m_funcOnCreateInfo(data);
                if (item != null)
                {
                    m_infoList.Add(item);
                    UpdateInfo();
                    m_reorderableList.index++;
                }
            }
        }
    }
}
