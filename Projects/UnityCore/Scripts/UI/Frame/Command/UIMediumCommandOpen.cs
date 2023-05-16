using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MonMoose.Core
{
    public class UIMediumCommandOpen : UIMediumCommand
    {
        public UIMediumContext context;
        public bool isAsync;
        public bool immediately;
        public Action<UIMedium> actionOnOpenEnd;
        public Action<UIMediumContext, bool, bool, Action<UIMedium>> actionOnOpen;

        public override void OnRelease()
        {
            base.OnRelease();
            isAsync = default;
            immediately = default;
            context = null;
            actionOnOpen = null;
            actionOnOpenEnd = null;
        }

        protected override void OnExecute()
        {
            if (actionOnOpen != null)
            {
                actionOnOpen(context, isAsync, immediately, OnMediumOpenEnd);
            }
            else
            {
                OnMediumOpenEnd(null);
            }
        }

        private void OnMediumOpenEnd(UIMedium medium)
        {
            if (actionOnOpenEnd != null)
            {
                actionOnOpenEnd(medium);
            }
            End();
        }
    }
}
