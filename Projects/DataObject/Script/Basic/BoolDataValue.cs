using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MonMoose.Core.Data
{
    public class BoolDataValue : BasicDataValue<bool>
    {
        protected override bool AnalyzeValue(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return false;
            }
            bool boolValue;
            if (bool.TryParse(str, out boolValue))
            {
                return boolValue;
            }
            int intValue;
            if (int.TryParse(str, out intValue))
            {
                if (intValue == 0)
                {
                    return false;
                }
                if (intValue == 1)
                {
                    return true;
                }
                return true;
            }
            throw new Exception();
        }
    }
}
