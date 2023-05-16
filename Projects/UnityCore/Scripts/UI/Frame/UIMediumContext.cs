using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MonMoose.Core
{
    public class UIMediumContext
    {
        public UIMediumId mediumId;
        public UIMediumContext preContext;

        public UIMediumContext(UIMediumId mediumId)
        {
            this.mediumId = mediumId;
        }

        public virtual void OverrideFrom(UIMediumContext context)
        {
            mediumId = context.mediumId;
            preContext = context.preContext;
        }
    }
}
