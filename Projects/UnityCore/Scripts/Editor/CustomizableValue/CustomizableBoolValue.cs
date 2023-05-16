using System;
using System.Collections;
using System.Collections.Generic;
using MonMoose.Core;

namespace MonMoose.Core
{
	[Serializable]
	public class CustomizableBoolValue : CustomizableValue<bool>
	{
        public CustomizableBoolValue(bool value) : base(value)
        {
        }
    }
}
