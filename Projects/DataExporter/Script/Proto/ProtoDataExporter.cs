using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Data;
using Google.Protobuf;
using Google.Protobuf.Collections;
using Structure;

namespace MonMooseCore.DataExporter
{
    public class ProtoDataExporter : DataExporter
    {
        private Dictionary<string, DataObjReflectHolder> m_dataObjRefMap = new Dictionary<string, DataObjReflectHolder>();
        private Dictionary<string, ListDataObjReflectHolder> m_listObjRefMap = new Dictionary<string, ListDataObjReflectHolder>();

        protected override void OnExport()
        {
            int index = 0;
            Dictionary<ClassStructureInfo, Dictionary<int, DataObject>> map = DataObjectManager.Instance.structureMap;
            foreach (var kv in map)
            {
                SendMsg(index, string.Format("正在导出数据:{0} ({1}/{2})", kv.Key.name, index, map.Count));
                ExportData(kv.Key, kv.Value);
                index++;
            }
        }

        private void ExportData(ClassStructureInfo structureInfo, Dictionary<int, DataObject> map)
        {
            Type structureType = GetType(structureInfo);
            PackStructureInfo packStructureInfo = StructureManager.Instance.GetStructureInfo(structureInfo.name + "List") as PackStructureInfo;
            Type listStructureType = m_context.assembly.GetType(GetTypeName(packStructureInfo));
            FieldInfo listFieldInfo = listStructureType.GetField("list_", BindingFlags.NonPublic | BindingFlags.Instance);

            MethodInfo listAddMethodInfo = listFieldInfo.FieldType.GetMethod("Add", new[] {structureType}, new[] {new ParameterModifier(1)});
            object listObj = Activator.CreateInstance(listStructureType);
            object listMemberObj = listFieldInfo.GetValue(listObj);
            MethodInfo writeMethod = listStructureType.GetMethod("WriteTo");
            foreach (var kv in map)
            {
                Object obj = AnalyzeDataObject(kv.Key, structureInfo, kv.Value);
                listAddMethodInfo.Invoke(listMemberObj, new[] {obj});
            }
            string outputPath = FolderManager.Instance.GetSubDirPath(m_context.name + "/" + m_context.exportMode.modeType.ToString(), EFolderType.Data) + "\\" + structureInfo.name;
            using (MemoryStream ms = new MemoryStream())
            {
                using (CodedOutputStream cos = new CodedOutputStream(ms))
                {
                    writeMethod.Invoke(listObj, new[] {cos});
                    cos.Flush();
                    using (FileStream fs = File.Create(outputPath))
                    {
                        ms.Position = 0;
                        ms.WriteTo(fs);
                        fs.Close();
                    }
                }
            }
            if (!string.IsNullOrEmpty(m_context.dataExportPath) && Directory.Exists(m_context.dataExportPath))
            {
                string destPath = m_context.dataExportPath + structureInfo.name;
                if (!string.IsNullOrEmpty(m_context.extensionStr))
                {
                    destPath = destPath + "." + m_context.extensionStr;
                }
                File.Copy(outputPath, destPath, true);
            }
        }

        protected override object AnalyzeListValue(ListStructureInfo structureInfo, ListDataValue dataValue)
        {
            ListDataObjReflectHolder refHolder;
            if (!m_listObjRefMap.TryGetValue(structureInfo.name, out refHolder))
            {
                refHolder = new ListDataObjReflectHolder();
                refHolder.itemType = GetType(structureInfo.valueStructureInfo);
                refHolder.listType = typeof(RepeatedField<>).MakeGenericType(refHolder.itemType);
                refHolder.addMethodInfo = refHolder.listType.GetMethod("Add", new[] {refHolder.itemType}, new[] {new ParameterModifier(1)});
                m_listObjRefMap.Add(structureInfo.name, refHolder);
            }
            Object obj = Activator.CreateInstance(refHolder.listType);
            foreach (DataValue value in dataValue.valueList)
            {
                object itemObj = AnalyzeValue(structureInfo.valueStructureInfo, value);
                refHolder.addMethodInfo.Invoke(obj, new[] {itemObj});
            }
            return obj;
        }

        protected override object AnalyzeDataObject(ClassStructureInfo structureInfo, DataObject dataObj)
        {
            DataObjReflectHolder refHolder;
            if (!m_dataObjRefMap.TryGetValue(structureInfo.name, out refHolder))
            {
                refHolder = new DataObjReflectHolder();
                refHolder.valueType = GetType(structureInfo);
                foreach (FieldInfo fieldInfo in refHolder.valueType.GetFields(BindingFlags.NonPublic | BindingFlags.Instance))
                {
                    if (fieldInfo.Name.EndsWith("_"))
                    {
                        string fieldName = fieldInfo.Name.TrimEnd('_').ToLower();
                        refHolder.fieldInfoMap.Add(fieldName, fieldInfo);
                    }
                }
                m_dataObjRefMap.Add(structureInfo.name, refHolder);
            }
            Object obj = Activator.CreateInstance(refHolder.valueType);
            foreach (DataField field in dataObj.fieldList)
            {
                string fieldLowerName = field.name.ToLower();
                FieldInfo fieldInfo;
                if (!refHolder.fieldInfoMap.TryGetValue(fieldLowerName, out fieldInfo))
                {
                    throw new Exception("");
                }
                object value = AnalyzeValue(field.structureInfo, field.value);
                fieldInfo.SetValue(obj, value);
            }
            return obj;
        }

        protected override object AnalyzeDataObject(int id, ClassStructureInfo structureInfo, DataObject dataObj)
        {
            object obj = AnalyzeDataObject(structureInfo, dataObj);
            Type valueType = GetType(structureInfo);
            FieldInfo fieldInfo = valueType.GetField("id_", BindingFlags.NonPublic | BindingFlags.Instance);
            fieldInfo.SetValue(obj, id);
            return obj;
        }

        public string GetTypeName(StructureInfo structureInfo)
        {
            switch (structureInfo.structureType)
            {
                case EStructureType.Basic:
                    return "System." + (structureInfo as BasicStructureInfo).name;
                case EStructureType.Class:
                {
                    string str = string.Empty;
                    if (!string.IsNullOrEmpty(m_context.namespaceStr))
                    {
                        str += m_context.namespaceStr + ".";
                    }
                    if (!string.IsNullOrEmpty(m_context.prefixStr))
                    {
                        str += m_context.prefixStr;
                    }
                    str += structureInfo.name;
                    if (!string.IsNullOrEmpty(m_context.postfixStr))
                    {
                        str += m_context.postfixStr;
                    }
                    return str;
                }
                case EStructureType.Pack:
                {
                    string str = string.Empty;
                    if (!string.IsNullOrEmpty(m_context.namespaceStr))
                    {
                        str += m_context.namespaceStr + ".";
                    }
                    if (!string.IsNullOrEmpty(m_context.prefixStr))
                    {
                        str += m_context.prefixStr;
                    }
                    str += (structureInfo as PackStructureInfo).classStructureInfo.name;
                    if (!string.IsNullOrEmpty(m_context.postfixStr))
                    {
                        str += m_context.postfixStr;
                    }
                    str += "List";
                    return str;
                }
                case EStructureType.Enum:
                    return structureInfo.name;
                case EStructureType.List:
                    return string.Format("List<{0}>", GetTypeName((structureInfo as ListStructureInfo).valueStructureInfo));
            }
            Debug.LogError("");
            return null;
        }

        public Type GetType(StructureInfo structureInfo)
        {
            string typeName = GetTypeName(structureInfo);
            if (structureInfo.structureType == EStructureType.Basic)
            {
                return Type.GetType(typeName);
            }
            return m_context.assembly.GetType(typeName);
        }

        public class DataObjReflectHolder
        {
            public Type valueType;
            public Dictionary<string, FieldInfo> fieldInfoMap = new Dictionary<string, FieldInfo>();
        }

        public class ListDataObjReflectHolder
        {
            public Type listType;
            public Type itemType;
            public MethodInfo addMethodInfo;
        }
    }
}