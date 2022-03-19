using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MonMoose.Core.Data
{
    public class StringDataValue : BasicDataValue
    {
        public string value;

        public override void Init(string str)
        {
            value = str;
            if (string.IsNullOrEmpty(value))
            {
                value = string.Empty;
            }
        }
    }
}
