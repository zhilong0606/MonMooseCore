using System;
using System.Collections;
using System.Collections.Generic;
using MonMoose.Core;

namespace MonMoose.Core
{
	[Serializable]
	public class CustomizableIntValue : CustomizableValue<int>
	{
        public CustomizableIntValue(int value) : base(value)
        {
        }
    }
}
