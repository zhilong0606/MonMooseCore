using System;
using System.Collections;
using System.Collections.Generic;

namespace MonMoose.Core
{
	public class ClassPoolList : ClassPool
	{
        public override string poolName
        {
            get
            {
                Type innerType = m_classType.GetGenericArguments().GetValueSafely(0);
                return string.Format("List<{0}>", innerType != null ? innerType.Name : "Null");
            }
        }

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
