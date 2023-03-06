using System;
using System.Collections;
using System.Collections.Generic;

namespace MonMoose.Core
{
    public class CommandMachine
    {
        private CommandWrap m_curWrap;
        private List<CommandWrap> m_wrapList = new List<CommandWrap>();
        private ListenedValue<bool> m_isBusy = new ListenedValue<bool>();

        public Action<bool> actionOnBusyStateChanged;

        public void Init()
        {
            m_isBusy.actionOnValueChanged = OnBusyStateChanged;
        }

        public void PushCmd(Command cmd, Action<Command> actionOnEnd)
        {
            CommandWrap wrap = ClassPoolManager.instance.Fetch<CommandWrap>();
            wrap.cmd = cmd;
            wrap.actionOnCmdEnd = actionOnEnd;
            m_wrapList.Add(wrap);
            TryExecuteNextCmd();
        }

        private void TryExecuteNextCmd()
        {
            if (m_curWrap == null && m_wrapList.Count > 0)
            {
                m_isBusy.curValue = true;
                ExecuteNextCmd();
            }
        }

        private void ExecuteNextCmd()
        {
            m_curWrap = m_wrapList[0];
            m_wrapList.RemoveAt(0);
            m_curWrap.cmd.Execute(OnCmdExecuteEnd);
        }

        private void OnCmdExecuteEnd(Command cmd)
        {
            if(m_curWrap.cmd != cmd)
            {
                DebugUtility.LogError("[CommandMachine] ExecuteNextCmd: Cannot End Cmd Not Cur");
                return;
            }
            CommandWrap wrap = m_curWrap;
            m_curWrap = null;
            if(wrap.actionOnCmdEnd != null)
            {
                wrap.actionOnCmdEnd(cmd);
            }
            wrap.cmd.Release();
            wrap.cmd = null;
            wrap.Release();
            if(m_wrapList.Count == 0)
            {
                m_isBusy.curValue = false;
            }
            else
            {
                ExecuteNextCmd();
            }
        }

        private void OnBusyStateChanged(bool pre, bool cur)
        {
            NotifyBusyStateChanged(cur);
        }

        private void NotifyBusyStateChanged(bool isBusy)
        {
            if (actionOnBusyStateChanged != null)
            {
                actionOnBusyStateChanged(isBusy);
            }
        }

        private class CommandWrap : ClassPoolObj
        {
            public Command cmd;
            public Action<Command> actionOnCmdEnd;

            public override void OnRelease()
            {
                base.OnRelease();
                cmd = null;
                actionOnCmdEnd = null;
            }
        }
    }
}
