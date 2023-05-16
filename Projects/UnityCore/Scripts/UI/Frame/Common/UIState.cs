using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MonMoose.Core
{
    [Serializable]
    public class UIState
    {
        public string name;
        public List<GameObject> activeObjList = new List<GameObject>();
    }
}
