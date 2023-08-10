using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace MonMoose.Core
{
    [CreateAssetMenu(fileName = "UIMediumConfig", menuName = "Custom Asset/UI/UIMediumConfig")]
    public class UIMediumConfig : ScriptableObject
    {
        public EnumString<UIMediumId> mediumId;
        [ListDrawerSettings(Expanded = true, AlwaysAddDefaultValue = true)]
        public List<EnumString<UIMediumTagId>> tagIdList = new List<EnumString<UIMediumTagId>>();
        public bool stackOptimizable = true;
        [ReadOnly]
        public string typeName;
        [OnValueChanged("OnBindScriptChanged")]
        public AssetWeakRef scriptWeakRef;
        [TableList(AlwaysExpanded = true)]
        public List<UIViewConfig> viewConfigList = new List<UIViewConfig>();

        [NonSerialized]
        public Type classType;

        public UIViewConfig GetViewConfigByName(string name)
        {
            for (int i = 0; i < viewConfigList.Count; ++i)
            {
                UIViewConfig viewConfig = viewConfigList[i];
                if (viewConfig != null && viewConfig.nameStr == name)
                {
                    return viewConfig;
                }
            }
            return null;
        }

        private void OnBindScriptChanged()
        {
            MonoScript monoScript = AssetWeakRefEditorUtility.GetAssetByWeakRef<MonoScript>(scriptWeakRef);
            if (monoScript != null)
            {
                typeName = monoScript.GetClass().Name;
            }
            else
            {
                typeName = string.Empty;
            }
        }

        [Button("添加并导出", DirtyOnClick = true)]
        private void GenerateBindCode()
        {
            UIMediumConfigInventory inventory = AssetDatabase.LoadAssetAtPath<UIMediumConfigInventory>(UIMediumConfigInventory.path);
            if (inventory.GetConfig(mediumId.value) == null)
            {
                inventory.configList.Add(this);
            }
            inventory.configList.Sort((x, y) => x.mediumId.value.CompareTo(y.mediumId.value));
            EditorUtility.SetDirty(inventory);
            AssetDatabase.SaveAssets();
            UIMediumConfigInventoryInspector.Export(inventory);
            UIMediumConfigEditorUtility.GenerateBindCode(this);
        }
    }
}
