using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MonMooseCore.Data
{
    public class UInt32DataValue : BasicDataValue<uint>
    {
        protected override uint AnalyzeValue(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return 0;
            }
            uint value;
            if (uint.TryParse(str, out value))
            {
                return value;
            }
            ulong longValue;
            if (ulong.TryParse(str, out longValue))
            {
                return (uint)longValue;
            }
            float floatValue;
            if (float.TryParse(str, out floatValue))
            {
                return (uint)floatValue;
            }
            throw new Exception();
        }
    }
}
