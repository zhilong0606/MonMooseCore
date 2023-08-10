using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace MonMoose.Core
{
    [Serializable]
    public class UIViewConfig
    {
        public string nameStr;
        public string typeName;
        [OnValueChanged("OnBindObjChanged")]
        public AssetWeakRef prefabWeakRef;

        [NonSerialized]
        public Type classType;

        private void OnBindObjChanged()
        {
            MonoScript viewMonoScript = UIMediumConfigEditorUtility.GetMonoScript(this);
            typeName = viewMonoScript.GetClass().Name;
        }
    }
}
