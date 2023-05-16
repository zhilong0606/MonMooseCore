using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MonMoose.Core
{
    public class UIMediumCommandClose : UIMediumCommand
    {
        public bool destroy;
        public bool immediately;
        public Action<UIMedium> actionOnCloseEnd;
        public Action<bool, bool, Action<UIMedium>> actionOnClose;

        public override void OnRelease()
        {
            base.OnRelease();
            destroy = default;
            immediately = default;
            actionOnClose = null;
            actionOnCloseEnd = null;
        }

        protected override void OnExecute()
        {
            if (actionOnClose != null)
            {
                actionOnClose(destroy, immediately, OnMediumCloseEnd);
            }
            else
            {
                OnMediumCloseEnd(null);
            }
        }

        private void OnMediumCloseEnd(UIMedium medium)
        {
            if (actionOnCloseEnd != null)
            {
                actionOnCloseEnd(medium);
            }
            End();
        }
    }
}
