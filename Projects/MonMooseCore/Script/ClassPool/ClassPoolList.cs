using System;
using System.Collections;
using System.Collections.Generic;

namespace MonMoose.Core
{
	public class ClassPoolList : ClassPool
	{
        protected override void OnRelease(object obj)
        {
            base.OnRelease(obj);
            IList list = obj as IList;
            if (list != null)
            {
                list.Clear();
            }
        }
    }
}
