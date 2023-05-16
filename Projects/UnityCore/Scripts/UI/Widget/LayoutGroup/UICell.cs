using UnityEngine;

namespace MonMoose.Core
{
    public delegate void CellSelectChangedDelegate(int index, bool isSelected);

    public delegate void CellEventHappenedDelegate(int eventId, int index, object obj);

    public abstract class UICell : MonoBehaviour
    {
        protected UIGrid m_ownerGrid;
        protected bool m_isRecycled = true;
        protected int m_index = 0;
        protected object m_info;

        public event CellEventHappenedDelegate eventOnCellEventHappened;

        public bool isRecycled
        {
            get { return m_isRecycled; }
        }

        public UIGrid ownerGrid
        {
            get { return m_ownerGrid; }
            set { m_ownerGrid = value; }
        }

        public int index
        {
            get { return m_index; }
            set { m_index = value; }
        }

        public object info
        {
            get { return m_info; }
        }

        protected virtual void OnUpdateCell()
        {
        }

        protected virtual void OnRenovate()
        {
        }

        protected virtual void OnRecycle()
        {
        }

        protected void Select()
        {
            m_ownerGrid.SelectIndex(m_index);
        }

        protected void SendEvent(int eventId)
        {
            SendEvent(eventId, null);
        }

        protected void SendEvent(int eventId, object obj)
        {
            if (eventOnCellEventHappened != null)
            {
                eventOnCellEventHappened(eventId, m_index, obj);
            }
        }

        public void UpdateCell(object info)
        {
            m_info = info;
            OnUpdateCell();
        }

        public void Renovate()
        {
            if (m_isRecycled)
            {
                OnRenovate();
                this.SetActiveSafely(true);
                m_isRecycled = false;
            }
        }

        public void Recycle()
        {
            if (!m_isRecycled)
            {
                OnRecycle();
                this.SetActiveSafely(false);
                m_isRecycled = true;
            }
        }
    }
}
