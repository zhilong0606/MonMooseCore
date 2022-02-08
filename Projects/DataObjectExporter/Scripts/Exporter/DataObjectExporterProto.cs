using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using Google.Protobuf;
using Google.Protobuf.Collections;
using MonMooseCore.Data;
using MonMooseCore.Structure;

namespace MonMooseCore.DataExporter
{
    public class DataObjectExporterProto : DataObjectExporter
    {
        private Dictionary<string, DataObjReflectHolder> m_dataObjRefMap = new Dictionary<string, DataObjReflectHolder>();
        private Dictionary<string, ListDataObjReflectHolder> m_listObjRefMap = new Dictionary<string, ListDataObjReflectHolder>();
        private Dictionary<string, FieldInfo> m_idFieldMap = new Dictionary<string, FieldInfo>();
        private Assembly m_assembly;
        private object m_dataObjRefMapLock = new object();
        private object m_listObjRefMapLock = new object();
        private object m_idFieldMapLock = new object();

        protected override void OnExport()
        {
            List<string> codeFilePathList = new List<string>();
            DirectoryInfo directoryInfo = new DirectoryInfo(m_context.structureExportPath);
            if (directoryInfo.Exists)
            {
                foreach (FileInfo fileInfo in directoryInfo.GetFiles("*.cs"))
                {
                    codeFilePathList.Add(fileInfo.FullName);
                }
            }
            m_assembly = GetCompilerAssembly(codeFilePathList.ToArray());
            if (m_assembly == null)
            {
                return;
            }
            if (m_context.useMultiTask)
            {
                ExportDataAsync();
            }
            else
            {
                ExportDataSync();
            }
        }

        private void ExportDataSync()
        {
            int index = 0;
            Dictionary<ClassStructureInfo, Dictionary<int, DataObject>> map = m_context.dataObjManager.structureMap;
            foreach (var kv in map)
            {
                SendMsg((double)index / map.Count, string.Format("正在导出数据:{0} ({1}/{2})", kv.Key.name, index, map.Count));
                ExportData(kv.Key, kv.Value);
                index++;
            }
        }

        private void ExportDataAsync()
        {
            List<System.Threading.Tasks.Task> taskList = new List<System.Threading.Tasks.Task>();
            Dictionary<ClassStructureInfo, Dictionary<int, DataObject>> map = m_context.dataObjManager.structureMap;
            foreach (var kv in map)
            {
                var task = System.Threading.Tasks.Task.Run(() => ExportData(kv.Key, kv.Value));
                taskList.Add(task);
            }
            System.Threading.Tasks.Task.WaitAll(taskList.ToArray());
        }

        private void ExportData(ClassStructureInfo structureInfo, Dictionary<int, DataObject> map)
        {
            Type structureType = GetType(structureInfo);
            PackStructureInfo packStructureInfo = m_context.structureManager.GetStructureInfo(structureInfo.name + "List") as PackStructureInfo;
            Type listStructureType = GetType(packStructureInfo);
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
            string outputPath = FilePathUtility.GetPath(m_context.dataExportFolderPath, structureInfo.name);
            if (!string.IsNullOrEmpty(m_context.dataExportExtensionStr))
            {
                outputPath = outputPath + "." + m_context.dataExportExtensionStr;
            }
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
        }

        protected override object AnalyzeListValue(ListStructureInfo structureInfo, ListDataValue dataValue)
        {
            ListDataObjReflectHolder refHolder;
            lock (m_listObjRefMapLock)
            {
                m_listObjRefMap.TryGetValue(structureInfo.name, out refHolder);
            }
            if (refHolder == null)
            {
                refHolder = new ListDataObjReflectHolder();
                refHolder.itemType = GetType(structureInfo.valueStructureInfo);
                refHolder.listType = typeof(RepeatedField<>).MakeGenericType(refHolder.itemType);
                refHolder.addMethodInfo = refHolder.listType.GetMethod("Add", new[] { refHolder.itemType }, new[] { new ParameterModifier(1) });
            }
            lock (m_listObjRefMapLock)
            {
                m_listObjRefMap[structureInfo.name] = refHolder;
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
            lock (m_dataObjRefMapLock)
            {
                m_dataObjRefMap.TryGetValue(structureInfo.name, out refHolder);
            }
            if (refHolder == null)
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
            }
            lock (m_dataObjRefMapLock)
            {
                m_dataObjRefMap[structureInfo.name] = refHolder;
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
            FieldInfo idField;
            lock (m_idFieldMapLock)
            {
                m_idFieldMap.TryGetValue(structureInfo.name, out idField);
            }
            if (idField == null)
            {
                Type valueType = GetType(structureInfo);
                idField = valueType.GetField("iD_", BindingFlags.NonPublic | BindingFlags.Instance);
            }
            lock (m_idFieldMapLock)
            {
                m_idFieldMap[structureInfo.name] = idField;
            }
            idField.SetValue(obj, id);
            return obj;
        }

        public string GetTypeName(StructureInfo structureInfo)
        {
            switch (structureInfo.structureType)
            {
                case EStructureType.Basic:
                    return "System." + (structureInfo as BasicStructureInfo).exportName;
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
            throw new Exception("");
            return null;
        }

        public Type GetType(StructureInfo structureInfo)
        {
            string typeName = GetTypeName(structureInfo);
            if (structureInfo.structureType == EStructureType.Basic)
            {
                return Type.GetType(typeName);
            }
            return m_assembly.GetType(typeName);
        }

        protected Assembly GetCompilerAssembly(string[] codeFiles)
        {
            CodeDomProvider complier = CodeDomProvider.CreateProvider("CSharp");
            CompilerParameters param = new CompilerParameters();
            param.GenerateExecutable = false;
            param.GenerateInMemory = true;
            param.TreatWarningsAsErrors = true;
            param.IncludeDebugInformation = false;
            param.ReferencedAssemblies.Add(m_context.dataExportDllPath);
            param.ReferencedAssemblies.Add("System.dll");
            param.ReferencedAssemblies.Add("System.Core.dll");

            string[] codes = new string[codeFiles.Length];
            for (int i = 0; i < codeFiles.Length; ++i)
            {
                codes[i] = File.ReadAllText(codeFiles[i], Encoding.UTF8);
            }

            CompilerResults result = complier.CompileAssemblyFromSource(param, codes);
            if (result.Errors.HasErrors)
            {
                StringBuilder sb = new StringBuilder(String.Empty);
                foreach (object err in result.Errors)
                {
                    sb.Append(err).Append("\r\n");
                }
                throw new Exception(sb.ToString());
            }
            return result.CompiledAssembly;
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