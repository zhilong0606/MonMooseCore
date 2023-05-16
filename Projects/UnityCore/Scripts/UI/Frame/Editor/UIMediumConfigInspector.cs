using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace MonMoose.Core
{
    [CustomEditor(typeof(UIMediumConfig))]
    public class UIMediumConfigInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            UIMediumConfig config = target as UIMediumConfig;
            if (config == null)
            {
                return;
            }
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("mediumId"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("tagIdList"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("stackOptimizable"));
            bool needUpdateMonoScript;
            MonoScript monoScript = AssetWeakRefEditorUtility.GetAssetByWeakRef<MonoScript>(config.scriptWeakRef, out needUpdateMonoScript);
            MonoScript newMonoScript = EditorGUILayout.ObjectField("Medium脚本", monoScript, typeof(MonoScript), false) as MonoScript;
            bool isDirty = false;
            if (newMonoScript != monoScript || needUpdateMonoScript)
            {
                monoScript = newMonoScript;
                config.scriptWeakRef = AssetWeakRefEditorUtility.GetAssetWeakRef(monoScript);
                isDirty = true;
            }
            string typeName = monoScript != null ? monoScript.name : string.Empty;
            if (config.typeName != typeName)
            {
                config.typeName = typeName;
                isDirty = true;
            }
            EditorGUILayout.PropertyField(serializedObject.FindProperty("viewConfigList"));
            if (GUILayout.Button("添加并导出"))
            {
                UIMediumConfigInventory inventory = AssetDatabase.LoadAssetAtPath<UIMediumConfigInventory>(UIMediumConfigInventory.path);
                if (inventory.GetConfig(config.mediumId.value) == null)
                {
                    inventory.configList.Add(config);
                }
                inventory.configList.Sort((x, y) => x.mediumId.value.CompareTo(y.mediumId.value));
                EditorUtility.SetDirty(inventory);
                AssetDatabase.SaveAssets();
                UIMediumConfigInventoryInspector.Export(inventory);
                GenerateBindCode(config);
            }
            if (EditorGUI.EndChangeCheck() || isDirty)
            {
                serializedObject.ApplyModifiedProperties();
                EditorUtility.SetDirty(target);
            }
        }


        public static void GenerateBindCode(UIMediumConfig config)
        {
            bool needUpdateMonoScript;
            MonoScript monoScript = AssetWeakRefEditorUtility.GetAssetByWeakRef<MonoScript>(config.scriptWeakRef, out needUpdateMonoScript);
            if (monoScript == null)
            {
                return;
            }
            List<string> usingNamespaceList = new List<string>();
            usingNamespaceList.Add(typeof(UIController).Namespace);
            foreach (var viewConfig in config.viewConfigList)
            {
                if (viewConfig == null)
                {
                    continue;
                }
                MonoScript viewMonoScript = UIMediumConfigEditorUtility.GetMonoScript(viewConfig);
                usingNamespaceList.AddNotContains(viewMonoScript.GetClass().Namespace);
            }
            usingNamespaceList.Sort();
            FileWriter fileWriter = new FileWriter();
            foreach (var namespaceStr in usingNamespaceList)
            {
                if (string.IsNullOrEmpty(namespaceStr))
                {
                    continue;
                }
                if (namespaceStr == monoScript.GetClass().Namespace)
                {
                    continue;
                }
                fileWriter.AppendLine("using {0};", namespaceStr);
            }
            fileWriter.AppendLine();
            if (!string.IsNullOrEmpty(monoScript.GetClass().Namespace))
            {
                fileWriter.AppendLine("namespace {0}", monoScript.GetClass().Namespace);
                fileWriter.StartBlock();
            }
            fileWriter.AppendLine("public partial class {0}", monoScript.name);
            fileWriter.StartBlock();
            {
                foreach (var viewConfig in config.viewConfigList)
                {
                    if (viewConfig == null)
                    {
                        continue;
                    }
                    MonoScript viewMonoScript = UIMediumConfigEditorUtility.GetMonoScript(viewConfig);
                    string fieldName = CodeNameUtility.GetPrivateFiledName(viewConfig.nameStr);
                    fileWriter.AppendLine("private {0} {1};", viewMonoScript.name, fieldName);
                }
                fileWriter.AppendLine();
                foreach (var viewConfig in config.viewConfigList)
                {
                    MonoScript viewMonoScript = UIMediumConfigEditorUtility.GetMonoScript(viewConfig);
                    string privateFieldName = CodeNameUtility.GetPrivateFiledName(viewConfig.nameStr);
                    string publicFieldName = CodeNameUtility.GetPublicFiledName(viewConfig.nameStr);
                    fileWriter.AppendLine("public {0} {1}", viewMonoScript.name, publicFieldName);
                    fileWriter.StartBlock();
                    {
                        fileWriter.AppendLine("get");
                        fileWriter.StartBlock();
                        {
                            fileWriter.AppendLine("if ({0} == null)", privateFieldName);
                            fileWriter.StartBlock();
                            {
                                fileWriter.AppendLine("{0} = CreateController(\"{1}\") as {2};", privateFieldName, viewConfig.nameStr, viewMonoScript.name);
                            }
                            fileWriter.EndBlock();
                            fileWriter.AppendLine("return {0};", privateFieldName);
                        }
                        fileWriter.EndBlock();
                    }
                    fileWriter.EndBlock();
                    fileWriter.AppendLine();
                }
                fileWriter.AppendLine("protected override void OnAddController(string name, UIController ctrl)");
                fileWriter.StartBlock();
                {
                    fileWriter.AppendLine("base.OnAddController(name, ctrl);");
                    fileWriter.AppendLine("switch (name)");
                    fileWriter.StartBlock();
                    {
                        foreach (var viewConfig in config.viewConfigList)
                        {
                            if (viewConfig == null)
                            {
                                continue;
                            }
                            string privateFieldName = CodeNameUtility.GetPrivateFiledName(viewConfig.nameStr);
                            MonoScript viewMonoScript = UIMediumConfigEditorUtility.GetMonoScript(viewConfig);
                            fileWriter.AppendLine("case \"{0}\":", viewConfig.nameStr);
                            fileWriter.StartTab();
                            {
                                fileWriter.AppendLine("{0} = ctrl as {1};", privateFieldName, viewMonoScript.name);
                                fileWriter.AppendLine("break;");
                            }
                            fileWriter.EndTab();
                        }
                    }
                    fileWriter.EndBlock();
                }
                fileWriter.EndBlock();
            }
            fileWriter.EndBlock();
            if (!string.IsNullOrEmpty(monoScript.GetClass().Namespace))
            {
                fileWriter.EndBlock();
            }

            string scriptRefPath = AssetDatabase.GetAssetPath(monoScript);
            string outputFolderPath = FilePathUtility.GetFileFolderPath(scriptRefPath);
            string fileNameWithoutExtension = FilePathUtility.GetFileNameWithoutExtension(scriptRefPath);
            string filePath = FilePathUtility.GetPath(outputFolderPath, fileNameWithoutExtension + "_AutoGen.cs");
            fileWriter.WriteFile(filePath);
            Debug.Log("[UIMediumConfigInspector] GenerateCode Finished: " + monoScript.name);
        }
    }
}
