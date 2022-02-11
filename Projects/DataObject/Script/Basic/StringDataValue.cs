﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MonMooseCore.Data
{
    public class StringDataValue : BasicDataValue
    {
        public string value;

        public override void Init(string str)
        {
            value = str;
        }
    }
}
