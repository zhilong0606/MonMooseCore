using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace MonMoose.Core
{
    [CustomEditor(typeof(UIMediumConfigInventory))]
    public class UIMediumConfigInventoryInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (GUILayout.Button("µ¼³ö"))
            {
                Export(target as UIMediumConfigInventory);
            }
        }

        public static void Export(UIMediumConfigInventory inventory)
        {
            string name = "UIMediumCustomTypeUtility";
            string folderPath = "Assets/Scripts/GameLogic/UI";
            string path = string.Format("{0}/{1}.cs", folderPath, name);
            FileWriter fileWriter = new FileWriter();
            fileWriter.AppendLine("using System;");
            fileWriter.AppendLine("using System.Collections.Generic;");
            fileWriter.AppendLine();
            fileWriter.AppendLine("namespace MonMoose.GameLogic.UI");
            fileWriter.StartBlock();
            {
                fileWriter.AppendLine("public static class {0}", name);
                fileWriter.StartBlock();
                {
                    fileWriter.AppendLine("private static Dictionary<string, Type> m_typeMap = new Dictionary<string, Type>();");
                    fileWriter.AppendLine();
                    fileWriter.AppendLine("static UIMediumCustomTypeUtility()");
                    fileWriter.StartBlock();
                    {
                        List<string> typeNameList = GetTypeNameList(inventory);
                        foreach (var typeName in typeNameList)
                        {
                            fileWriter.AppendLine("m_typeMap.Add(\"{0}\", typeof({0}));", typeName);
                        }
                    }
                    fileWriter.EndBlock();

                    fileWriter.AppendLine("public static Type GetType(string typeName)");
                    fileWriter.StartBlock();
                    {
                        fileWriter.AppendLine("Type type;");
                        fileWriter.AppendLine("m_typeMap.TryGetValue(typeName, out type);");
                        fileWriter.AppendLine("return type;");
                    }
                    fileWriter.EndBlock();
                }
                fileWriter.EndBlock();
            }
            fileWriter.EndBlock();

            fileWriter.WriteFile(path);
        }

        private static List<string> GetTypeNameList(UIMediumConfigInventory inventory)
        {
            List<string> list = new List<string>();
            foreach (var config in inventory.configList)
            {
                AppendTypeList(config, list);
            }
            return list;
        }

        private static void AppendTypeList(UIMediumConfig config, List<string> list)
        {
            if (config == null)
            {
                return;
            }
            string typeName = config.typeName;
            if (!list.Contains(typeName))
            {
                list.Add(typeName);
            }
            foreach (var viewConfig in config.viewConfigList)
            {
                AppendTypeList(viewConfig, list);
            }
        }

        private static void AppendTypeList(UIViewConfig config, List<string> list)
        {
            if (config == null)
            {
                return;
            }
            string typeName = config.typeName;
            if (!list.Contains(typeName))
            {
                list.Add(typeName);
            }
        }
    }
}
