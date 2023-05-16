using System;
using System.Collections;
using System.Collections.Generic;
using MonMoose.Core;

namespace MonMoose.Core
{
    [Serializable]
    public class CustomizableValue<T> : CustomizableValue
    {
        public T specificValue;

        public CustomizableValue(T value)
        {
            specificValue = value;
        }

        public CustomizableValue() { }
    }

    [Serializable]
	public class CustomizableValue
    {
        public int customId;
        public bool needCustom;
    }
}
