using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MonMoose.Core
{
    public class ResourceLoadTaskAlreadyLoad : ResourceLoadTask
    {
        protected override bool needLag
        {
            get { return false; }
        }

        protected override bool CheckLoadEnd()
        {
            return true;
        }
    }
}
