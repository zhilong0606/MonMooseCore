using System;

namespace MonMooseCore
{
    public abstract class FrameCommand : ClassPoolObj
    {
        private Action<FrameCommand> m_actionOnExecute;

        public abstract int typeId { get; }

        protected void Excute()
        {
            if (m_actionOnExecute != null)
            {
                m_actionOnExecute(this);
            }
        }
    }
}
