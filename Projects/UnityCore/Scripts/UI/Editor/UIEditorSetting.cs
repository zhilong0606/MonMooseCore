using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace MonMoose.Core
{
    [CreateAssetMenu(fileName = "UIEditorSetting", menuName = "Custom Asset/UI/UIEditorSetting")]
    public class UIEditorSetting : ScriptableObject
    {
        public string mediumNamespace;
        public string controllerNamespace;

        public static UIEditorSetting instance
        {
            get { return ScriptableObjectEditorUtility.GetInstance<UIEditorSetting>(); }
        }
    }
}
