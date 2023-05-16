using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MonMoose.Core
{
    public class UIStateController : MonoBehaviour
    {
        public List<UIState> m_stateList = new List<UIState>();
        public List<UIStateTransition> m_transitionList = new List<UIStateTransition>();

        public void ChangeState(string stateName, bool immediately, Action<UIState, UIStateTransition> actionOnEnd)
        {
            UIState state = GetState(stateName);

        }

        public UIState GetState(string stateName)
        {
            for (int i = 0; i < m_stateList.Count; ++i)
            {
                if (m_stateList[i].name == stateName)
                {
                    return m_stateList[i];
                }
            }
            return null;
        }
    }
}
