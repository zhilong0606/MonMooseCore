using System;
using System.Collections;
using System.Collections.Generic;
using MonMoose.Core;

namespace MonMoose.Core
{
	[Serializable]
	public class CustomizableFloatValue : CustomizableValue<float>
	{
        public CustomizableFloatValue(float value) : base(value)
        {
        }
    }
}
