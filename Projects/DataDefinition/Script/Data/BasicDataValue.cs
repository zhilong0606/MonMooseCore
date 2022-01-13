using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Structure;

namespace Data
{
    public class BasicDataValue : DataValue
    {
        public string value;

        public BasicDataValue(string value)
        {
            this.value = value;
        }
    }
}
