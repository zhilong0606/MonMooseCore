using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace MonMoose.Core
{
    public class UIBindGroup : MonoBehaviour
    {
        public AssetWeakRef scriptWeakRef;
        //[ListDrawerSettings(AlwaysAddDefaultValue = true)]
        [TableList(AlwaysExpanded = true)]
        public List<UIBindItemInfo> bindItemList = new List<UIBindItemInfo>();

        public UIBindItemInfo GetItemInfoByName(string nameStr)
        {
            for (int i = 0; i < bindItemList.Count; ++i)
            {
                if (bindItemList[i].nameStr == nameStr)
                {
                    return bindItemList[i];
                }
            }
            return null;
        }

        [Button(DirtyOnClick = true)]
        private void GenerateBindCode()
        {
            List<string> errorList = new List<string>();
            if (!UIBindEditorUtility.CheckBindGroupValid(this, errorList))
            {
                StringBuilder sb = new StringBuilder();
                foreach (string str in errorList)
                {
                    sb.AppendLine(str);
                }
                EditorUtility.DisplayDialog("¥ÌŒÛ", sb.ToString(), "∂Æ¡À");
            }
            else
            {
                UIBindEditorUtility.GenerateBindCode(this);
            }
        }

    }
}
