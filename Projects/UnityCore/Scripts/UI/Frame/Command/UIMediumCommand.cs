using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MonMoose.Core
{
    public abstract class UIMediumCommand : Command
    {
        protected UIMedium m_medium;

        public void Init(UIMedium medium)
        {
            m_medium = medium;
        }

        public override void OnRelease()
        {
            m_medium = null;
            base.OnRelease();
        }
    }
}
