using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MonMoose.Core
{
    public abstract class LockableToggleUIController : ToggleUIController
    {
        protected abstract void OnViewChanged(bool isOn, bool isLocked);

        protected sealed override void OnToggleViewChanged(bool flag)
        {
            OnViewChanged(flag, m_isLocked);
        }

        protected override void OnClicked()
        {
            if (m_isLocked)
            {
                OnClickLocked();
                return;
            }
            base.OnClicked();
        }

        protected virtual void OnClickLocked()
        {
            
        }

        public bool isLocked
        {
            get { return m_isLocked; }
            set
            {
                m_isLocked = value;
                OnViewChanged(m_isOn, m_isLocked);
            }
        }

        private bool m_isLocked;
    }
}