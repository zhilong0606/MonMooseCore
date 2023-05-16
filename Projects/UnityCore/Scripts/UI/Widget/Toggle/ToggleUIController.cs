using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MonMoose.Core
{
    public abstract class ToggleUIController : MonoBehaviour, IToggleHandler
    {
        public void SetToggle(bool flag, bool silence)
        {
            HandleToggleChanged(flag, silence);
        }

        protected virtual void OnClicked()
        {
            if (toggleGroup != null)
            {
                toggleGroup.OnToggleClicked(this);
            }
            else
            {
                HandleToggleChanged(!m_isOn);
            }
        }

        protected void HandleToggleChanged(bool flag, bool silence = false)
        {
            m_isOn = flag;
            OnToggleViewChanged(m_isOn);
            if (!silence)
            {
                if (actionOnToggleChanged != null)
                {
                    actionOnToggleChanged(m_isOn);
                }
            }
        }

        protected abstract void OnToggleViewChanged(bool flag);

        public bool isOn { get { return m_isOn; } }
        public UIToggleGroup toggleGroup { get; set; }
        public Action<bool> actionOnToggleChanged;

        protected bool m_isOn;
    }
}