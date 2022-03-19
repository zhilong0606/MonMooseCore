using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MonMoose.Core.Data
{
    public class Int32DataValue : BasicDataValue<int>
    {
        protected override int AnalyzeValue(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return 0;
            }
            int value;
            if (int.TryParse(str, out value))
            {
                return value;
            }
            long longValue;
            if (long.TryParse(str, out longValue))
            {
                return (int)longValue;
            }
            float floatValue;
            if (float.TryParse(str, out floatValue))
            {
                return (int)floatValue;
            }
            throw new Exception();
        }
    }
}
