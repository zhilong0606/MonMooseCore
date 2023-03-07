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

        public Command topCommand
        {
            get { return m_curWrap != null ? m_curWrap.cmd : null; }
        }

        public void Init()
        {
            m_isBusy.actionOnValueChanged = OnBusyStateChanged;
        }

        public void PushCommand(Command cmd, Action<Command> actionOnEnd)
        {
            if(cmd == null)
            {
                return;
            }
            CommandWrap wrap = ClassPoolManager.instance.Fetch<CommandWrap>();
            wrap.cmd = cmd;
            wrap.actionOnCommandEnd = actionOnEnd;
            m_wrapList.Add(wrap);
            TryExecuteNextCommand();
        }

        public void ManualEndTopCommand()
        {
            if (m_curWrap != null)
            {
                m_curWrap.cmd.End();
            }
        }

        private void TryExecuteNextCommand()
        {
            if (m_curWrap == null && m_wrapList.Count > 0)
            {
                m_isBusy.curValue = true;
                ExecuteNextCommand();
            }
        }

        private void ExecuteNextCommand()
        {
            m_curWrap = m_wrapList[0];
            m_wrapList.RemoveAt(0);
            m_curWrap.cmd.Execute(OnCommandExecuteEnd);
        }

        private void OnCommandExecuteEnd(Command cmd)
        {
            if(m_curWrap.cmd != cmd)
            {
                DebugUtility.LogError("[CommandMachine] ExecuteNextCommand: Cannot End Command Not Cur");
                return;
            }
            CommandWrap wrap = m_curWrap;
            m_curWrap = null;
            if (wrap.actionOnCommandEnd != null)
            {
                wrap.actionOnCommandEnd(wrap.cmd);
            }
            wrap.cmd.Release();
            wrap.cmd = null;
            wrap.Release();
            if (m_wrapList.Count == 0)
            {
                m_isBusy.curValue = false;
            }
            else
            {
                ExecuteNextCommand();
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
            public Action<Command> actionOnCommandEnd;

            public override void OnRelease()
            {
                base.OnRelease();
                cmd = null;
                actionOnCommandEnd = null;
            }
        }
    }
}
