using System;
using System.Collections;
using System.Collections.Generic;

namespace MonMoose.Core
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
