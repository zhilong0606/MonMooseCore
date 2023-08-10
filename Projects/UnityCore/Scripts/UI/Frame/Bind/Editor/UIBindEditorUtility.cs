using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace MonMoose.Core
{
    public static class UIBindEditorUtility
    {
        public static void GenerateBindCode(UIBindGroup group)
        {
            MonoScript monoScript = GetMonoScriptBindGroup(group);
            if (monoScript == null)
            {
                return;
            }
            List<string> usingNamespaceList = new List<string>();
            usingNamespaceList.Add(typeof(UIBindGroup).Namespace);
            usingNamespaceList.AddNotContains(typeof(UIBindUtility).Namespace);
            foreach (var itemInfo in group.bindItemList)
            {
                if (itemInfo == null)
                {
                    continue;
                }
                string namespaceStr = UIBindUtility.GetBindTypeNamespace(itemInfo.bindType.value);
                usingNamespaceList.AddNotContains(namespaceStr);
            }
            usingNamespaceList.Sort();
            FileWriter fileWriter = new FileWriter();
            foreach(var namespaceStr in usingNamespaceList)
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
            fileWriter.AppendLine("public partial class {0}", monoScript.GetClass().Name);
            fileWriter.StartBlock();
            {
                foreach (var itemInfo in group.bindItemList)
                {
                    if(itemInfo == null)
                    {
                        continue;
                    }
                    string fieldName = CodeNameUtility.GetPrivateFiledName(itemInfo.nameStr);
                    string bindTypeName = GetBindTypeName(itemInfo);
                    fileWriter.AppendLine("private {0} {1};", bindTypeName, fieldName);
                }
                fileWriter.AppendLine();
                foreach (var itemInfo in group.bindItemList)
                {
                    if (itemInfo == null)
                    {
                        continue;
                    }
                    if (itemInfo.bindType.value != UIBindItemType.SubBindGroup)
                    {
                        continue;
                    }
                    string privateFieldName = CodeNameUtility.GetPrivateFiledName(itemInfo.nameStr);
                    string publicFieldName = CodeNameUtility.GetPublicFiledName(itemInfo.nameStr);
                    string bindTypeName = GetBindTypeName(itemInfo);
                    fileWriter.AppendLine("public {0} {1}", bindTypeName, publicFieldName);
                    fileWriter.StartBlock();
                    {
                        fileWriter.AppendLine("get {{ return {0}; }}", privateFieldName);
                    }
                    fileWriter.EndBlock();
                }
                fileWriter.AppendLine();
                fileWriter.AppendLine("protected sealed override void OnBindSubGroupsByBindGroup(UIBindGroup group)");
                fileWriter.StartBlock();
                {
                    fileWriter.AppendLine("base.OnBindSubGroupsByBindGroup(group);");
                    foreach (var itemInfo in group.bindItemList)
                    {
                        if (itemInfo == null)
                        {
                            continue;
                        }
                        if (itemInfo.bindType.value != UIBindItemType.SubBindGroup)
                        {
                            continue;
                        }
                        string bindTypeName = GetBindTypeName(itemInfo);
                        fileWriter.AppendLine("AddBindComponent(group.GetItemInfoByName(\"{0}\"), typeof({1}));", itemInfo.nameStr, bindTypeName);
                    }
                }
                fileWriter.EndBlock();
                fileWriter.AppendLine();
                fileWriter.AppendLine("protected sealed override void OnBindViewsByBindGroup(UIBindGroup group)");
                fileWriter.StartBlock();
                {
                    fileWriter.AppendLine("base.OnBindViewsByBindGroup(group);");
                    foreach (var itemInfo in group.bindItemList)
                    {
                        if (itemInfo == null)
                        {
                            continue;
                        }
                        string fieldName = CodeNameUtility.GetPrivateFiledName(itemInfo.nameStr);
                        string bindTypeName = GetBindTypeName(itemInfo);
                        if (itemInfo.bindType.value == UIBindItemType.GameObject)
                        {
                            fileWriter.AppendLine("{0} = UIBindUtility.GetBindObject(group.GetItemInfoByName(\"{1}\"));", fieldName, itemInfo.nameStr, bindTypeName);
                        }
                        else
                        {
                            fileWriter.AppendLine("{0} = UIBindUtility.GetBindComponent(group.GetItemInfoByName(\"{1}\"), typeof({2})) as {2};", fieldName, itemInfo.nameStr, bindTypeName);
                        }
                    }
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
            string filePath = FilePathUtility.GetPath(outputFolderPath, fileNameWithoutExtension + "_Bind.cs");
            fileWriter.WriteFile(filePath);
            Debug.Log("[UIBindEditorUtility] GenerateBindCode Finished: " + monoScript.GetClass().Name);
            List<UIBindGroup> subGroupList = new List<UIBindGroup>();
            foreach (var itemInfo in group.bindItemList)
            {
                if (itemInfo == null)
                {
                    continue;
                }
                if (itemInfo.bindType.value == UIBindItemType.SubBindGroup)
                {
                    subGroupList.Add(itemInfo.bindObj.GetComponent<UIBindGroup>());
                }
                else if (itemInfo.bindType.value == UIBindItemType.GameObjectPool)
                {
                    GameObjectPool pool = itemInfo.bindObj.GetComponentSafely<GameObjectPool>();
                    if (pool != null)
                    {
                        subGroupList.AddNotNull(pool.dynamicObj.GetComponentSafely<UIBindGroup>());
                    }
                }
            }
            for (int i = 0; i < subGroupList.Count; ++i)
            {
                GenerateBindCode(subGroupList[i]);
            }
        }

        public static bool CheckBindGroupValid(UIBindGroup group, List<string> appendErrorList, List<UIBindGroup> dupCheckList = null)
        {
            bool result = true;
            if (dupCheckList == null)
            {
                dupCheckList = new List<UIBindGroup>();
            }
            if (dupCheckList.Contains(group))
            {
                AppendErrorList(appendErrorList, string.Format("[BindGroup] CheckBindGroupValid: Duplicate: {0}", GetGameObjectChildPath(group.gameObject)));
                return false;
            }
            dupCheckList.Add(group);
            MonoScript monoScript = GetMonoScriptBindGroup(group);
            if (monoScript == null)
            {
                AppendErrorList(appendErrorList, string.Format("[BindGroup] CheckBindGroupValid: Script Cannot Find. GameObjectPath: {0}", GetGameObjectChildPath(group.gameObject)));
                result = false;
            }
            List<string> itemNameList = new List<string>();
            for (int i = 0; i < group.bindItemList.Count; ++i)
            {
                UIBindItemInfo itemInfo = group.bindItemList[i];
                if (itemNameList.Contains(itemInfo.nameStr))
                {
                    AppendErrorList(appendErrorList, string.Format("[BindGroup] CheckBindGroupValid: Same Output Name. GameObjectPath: {0}, Output Name: {1}", GetGameObjectChildPath(group.gameObject), itemInfo.nameStr));
                    result = false;
                }
                else
                {
                    itemNameList.Add(itemInfo.nameStr);
                }
                if (string.IsNullOrEmpty(itemInfo.nameStr))
                {
                    AppendErrorList(appendErrorList, string.Format("[BindGroup] CheckBindGroupValid: Empty Name. GameObjectPath: {0}, Index: {1}", GetGameObjectChildPath(group.gameObject), i));
                    result = false;
                }
                if (itemInfo.bindObj == null)
                {
                    AppendErrorList(appendErrorList, string.Format("[BindGroup] CheckBindGroupValid: Has Missing Bind Object. GameObjectPath: {0}, Index: {1}", GetGameObjectChildPath(group.gameObject), i));
                    result = false;
                }
                else if (itemInfo.bindType.value == UIBindItemType.SubBindGroup)
                {
                    UIBindGroup subBindGroup = itemInfo.bindObj.GetComponent<UIBindGroup>();
                    if (!CheckBindGroupValid(subBindGroup, appendErrorList, dupCheckList))
                    {
                        result = false;
                    }
                }
                else if (itemInfo.bindType.value == UIBindItemType.GameObjectPool)
                {
                    GameObjectPool pool = itemInfo.bindObj.GetComponentSafely<GameObjectPool>();
                    if (pool != null)
                    {
                        UIBindGroup subBindGroup = pool.dynamicObj.GetComponentSafely<UIBindGroup>();
                        if (subBindGroup != null && !CheckBindGroupValid(subBindGroup, appendErrorList, dupCheckList))
                        {
                            result = false;
                        }
                    }
                }
                if (!CheckBindItemInfoComponentExist(itemInfo))
                {
                    AppendErrorList(appendErrorList, string.Format("[BindGroup] CheckBindGroupValid: Cannot Find Component. GameObjectPath: {0}, Index: {1}", GetGameObjectChildPath(group.gameObject), i));
                    result = false;
                }
                
            }
            return result;
        }

        private static string GetBindTypeName(UIBindItemInfo itemInfo)
        {
            if (itemInfo == null)
            {
                return string.Empty;
            }
            UIBindItemType bindType = itemInfo.bindType.value;
            if (bindType == UIBindItemType.SubBindGroup)
            {
                UIBindGroup subBindGroup = null;
                if (itemInfo.bindObj != null)
                {
                    subBindGroup = itemInfo.bindObj.GetComponent<UIBindGroup>();
                }
                MonoScript subMonoScript = GetMonoScriptBindGroup(subBindGroup);
                if (subMonoScript != null)
                {
                    return subMonoScript.GetClass().Name;
                }
            }
            else
            {
                return UIBindUtility.GetBindTypeName(bindType);
            }
            return string.Empty;
        }

        private static void AppendErrorList(List<string> appendErrorList, string errorStr)
        {
            if (appendErrorList != null)
            {
                appendErrorList.Add(errorStr);
            }
        }

        public static MonoScript GetMonoScriptBindGroup(UIBindGroup group)
        {
            if (group == null)
            {
                return null;
            }
            return AssetWeakRefEditorUtility.GetAssetByWeakRef<MonoScript>(group.scriptWeakRef);
        }

        private static bool CheckBindItemInfoComponentExist(UIBindItemInfo itemInfo)
        {
            GameObject bindObj = itemInfo.bindObj;
            if (bindObj == null)
            {
                return false;
            }
            if (itemInfo.bindType.value == UIBindItemType.SubBindGroup)
            {
                return bindObj.GetComponent<UIBindGroup>() != null;
            }
            return UIBindUtility.GetBindComponent(itemInfo) != null;
        }

        private static string GetGameObjectChildPath(GameObject child)
        {
            GameObject go = child;
            if (go != null)
            {
                Transform trans = go.transform;
                string str = string.Empty;
                while (trans != null)
                {
                    str = "/" + trans.gameObject.name + str;
                    trans = trans.parent;
                }
                return str.Substring(1);
            }
            return string.Empty;
        }
    }
}
