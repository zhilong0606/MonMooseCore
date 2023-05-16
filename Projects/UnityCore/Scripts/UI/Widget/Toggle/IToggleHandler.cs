using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MonMoose.Core
{
    public interface IToggleHandler
    {
        UIToggleGroup toggleGroup { get; set; }
        void SetToggle(bool flag, bool silence);
    }
}
