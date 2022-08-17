using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using MonMoose.Core.Structure;

namespace MonMoose.Core.DataExporter
{
    public class DataStructureExporterProto : DataStructureExporter
    {
        private List<EnumStructureInfo> m_enumList = new List<EnumStructureInfo>();
        private List<ClassStructureInfo> m_classList = new List<ClassStructureInfo>();
        private List<ListStructureInfo> m_listList = new List<ListStructureInfo>();
        private List<PackStructureInfo> m_packList = new List<PackStructureInfo>();
        private List<DictionaryStructureInfo> m_dictionaryList = new List<DictionaryStructureInfo>();

        private Dictionary<string, DataStructureExportGroup> m_exportGroupMap = new Dictionary<string, DataStructureExportGroup>();
        private Dictionary<string, WriterGroup> m_ilWriterGroupMap = new Dictionary<string, WriterGroup>();
        private List<string> m_ilPathList = new List<string>();

        protected override void OnExport()
        {
            CollectStructureLists();
            //if (m_context.exportGroupList.Count == 0)
            //{
            //    DataStructureExportGroup exportGroup = new DataStructureExportGroup();
            //    exportGroup.name = m_context.singleFileExportGroupName;
            //    AddStructureNameToList(exportGroup.structureNameList, m_enumList);
            //    AddStructureNameToList(exportGroup.structureNameList, m_classList);
            //    AddStructureNameToList(exportGroup.structureNameList, m_packList);
            //    m_context.exportGroupList.Add(exportGroup);
            //}
            if (m_context.exportGroupList.Count == 0)
            {
                m_context.stopwatchCollector.Start("InitDefaultExportGroup");
                foreach (var structureInfo in m_enumList)
                {
                    DataStructureExportGroup exportGroup = new DataStructureExportGroup();
                    exportGroup.name = structureInfo.name;
                    exportGroup.structureNameList.Add(structureInfo.name);
                    m_context.exportGroupList.Add(exportGroup);
                }
                foreach (var structureInfo in m_classList)
                {
                    DataStructureExportGroup exportGroup = new DataStructureExportGroup();
                    exportGroup.name = structureInfo.name;
                    exportGroup.structureNameList.Add(structureInfo.name);
                    m_context.exportGroupList.Add(exportGroup);
                }
                foreach (var structureInfo in m_packList)
                {
                    DataStructureExportGroup exportGroup = new DataStructureExportGroup();
                    exportGroup.name = structureInfo.name;
                    exportGroup.structureNameList.Add(structureInfo.name);
                    m_context.exportGroupList.Add(exportGroup);
                }
                m_context.stopwatchCollector.Stop();
            }
            CollectExportGroupMap();
            m_context.stopwatchCollector.Start("ExportStructure");
            foreach (DataStructureExportGroup exportGroup in m_context.exportGroupList)
            {
                if (!HasAnyStructureNeedExport(exportGroup, m_context.ignoreStructureNameList))
                {
                    continue;
                }
                WriterGroup writerGroup = GetWriterGroup(exportGroup);
                ExportHead(writerGroup);
                ExportStructureList(exportGroup, m_enumList, writerGroup, ExportEnum);
                ExportStructureList(exportGroup, m_classList, writerGroup, ExportClass);
                ExportStructureList(exportGroup, m_packList, writerGroup, ExportPack);
            }
            m_context.stopwatchCollector.Stop();
            WriteIlFile();
            RunProtoc();
        }

        private void CollectExportGroupMap()
        {
            m_context.stopwatchCollector.Start("CollectExportGroupMap");
            for (int i = 0; i < m_context.exportGroupList.Count; ++i)
            {
                DataStructureExportGroup exportGroup = m_context.exportGroupList[i];
                for (int j = 0; j < exportGroup.structureNameList.Count; ++j)
                {
                    m_exportGroupMap.Add(exportGroup.structureNameList[j], exportGroup);
                }
            }
            m_context.stopwatchCollector.Stop();
        }

        private void CollectStructureLists()
        {
            foreach (var kv in m_context.structureManager.structureMap)
            {
                if (kv.Value.structureType == EStructureType.Enum)
                {
                    m_enumList.Add(kv.Value as EnumStructureInfo);
                }
                else if (kv.Value.structureType == EStructureType.Class)
                {
                    m_classList.Add(kv.Value as ClassStructureInfo);
                }
                if (kv.Value.structureType == EStructureType.List)
                {
                    m_listList.Add(kv.Value as ListStructureInfo);
                }
                if (kv.Value.structureType == EStructureType.Dictionary)
                {
                    m_dictionaryList.Add(kv.Value as DictionaryStructureInfo);
                }
                if (kv.Value.structureType == EStructureType.Pack)
                {
                    m_packList.Add(kv.Value as PackStructureInfo);
                }
                if (kv.Value.structureType == EStructureType.None)
                {
                    throw new Exception("");
                }
            }
        }

        private void ExportStructureList<T>(DataStructureExportGroup exportGroup, List<T> targetStructureList, WriterGroup writerGroup, Action<T, WriterGroup> actionOnExport) where T : StructureInfo
        {
            foreach (string structureName in exportGroup.structureNameList)
            {
                if (m_context.ignoreStructureNameList.Contains(structureName))
                {
                    continue;
                }
                T structureInfo = GetStructureInfo(structureName, targetStructureList);
                if (structureInfo != null)
                {
                    actionOnExport(structureInfo, writerGroup);
                }
            }
        }

        private WriterGroup GetWriterGroup(DataStructureExportGroup exportGroup)
        {
            WriterGroup group;
            if(!m_ilWriterGroupMap.TryGetValue(exportGroup.name, out group))
            {
                group = new WriterGroup(exportGroup);
                m_ilWriterGroupMap.Add(exportGroup.name, group);
            }
            return group;
        }

        private void ExportHead(WriterGroup writerGroup)
        {
            FileWriter ilWriter = writerGroup.writer;
            ilWriter.AppendLine("syntax = \"proto3\";");
            writerGroup.CreateAndInsertBlockItem();
            if (!string.IsNullOrEmpty(m_context.namespaceStr))
            {
                ilWriter.AppendLine(string.Format("package {0};", m_context.namespaceStr));
            }
        }

        private void ExportEnum(EnumStructureInfo structureInfo, WriterGroup writerGroup)
        {
            FileWriter ilWriter = writerGroup.writer;
            ilWriter.AppendLine();
            ilWriter.AppendLine("enum {0}", GetExportName(structureInfo));
            ilWriter.StartBlock();
            {
                foreach (EnumMemberInfo memberInfo in structureInfo.memberList)
                {
                    ilWriter.AppendLine("{0}_{1} = {2};", structureInfo.name, memberInfo.name, memberInfo.index);
                }
            }
            ilWriter.EndBlock();
        }

        private void ExportClass(ClassStructureInfo structureInfo, WriterGroup writerGroup)
        {
            FileWriter ilWriter = writerGroup.writer;
            ilWriter.AppendLine();
            ilWriter.AppendLine("message {0}", GetExportName(structureInfo));
            ilWriter.StartBlock();
            {
                for (int i = 0; i < structureInfo.memberList.Count; ++i)
                {
                    ClassMemberInfo memberInfo = structureInfo.memberList[i];
                    ilWriter.StartLine();
                    StructureInfo memberStructureInfo = memberInfo.structureInfo;
                    switch (memberStructureInfo.structureType)
                    {
                        case EStructureType.Basic:
                            ilWriter.Append(GetExportName(memberStructureInfo));
                            break;
                        case EStructureType.Enum:
                        case EStructureType.Class:
                            ilWriter.Append(GetExportName(memberStructureInfo));
                            writerGroup.AddImportGroup(GetExportGroup(memberStructureInfo.name));
                            break;
                        case EStructureType.List:
                            ListStructureInfo memberListInfo = memberStructureInfo as ListStructureInfo;
                            if (!memberListInfo.valueStructureInfo.isCollection)
                            {
                                ilWriter.Append("repeated {0}", GetExportName(memberListInfo.valueStructureInfo));
                            }
                            else
                            {
                                throw new Exception();
                            }
                            writerGroup.AddImportGroup(GetExportGroup(memberListInfo.valueStructureInfo.name));
                            break;
                        case EStructureType.Dictionary:
                            DictionaryStructureInfo memberDictionaryInfo = memberStructureInfo as DictionaryStructureInfo;
                            if (!memberDictionaryInfo.keyStructureInfo.isCollection && !memberDictionaryInfo.valueStructureInfo.isCollection)
                            {
                                ilWriter.Append("map<{0},{1}>", GetExportName(memberDictionaryInfo.keyStructureInfo), GetExportName(memberDictionaryInfo.valueStructureInfo));
                            }
                            else
                            {
                                throw new Exception();
                            }
                            writerGroup.AddImportGroup(GetExportGroup(memberDictionaryInfo.keyStructureInfo.name));
                            writerGroup.AddImportGroup(GetExportGroup(memberDictionaryInfo.valueStructureInfo.name));
                            break;
                        default:
                            throw new Exception("");
                    }
                    ilWriter.Append(" {0} = {1};", memberInfo.name, i + 1);
                    ilWriter.EndLine();
                }
            }
            ilWriter.EndBlock();
        }

        private void ExportPack(PackStructureInfo structureInfo, WriterGroup writerGroup)
        {
            FileWriter ilWriter = writerGroup.writer;
            ilWriter.AppendLine();
            ilWriter.AppendLine("message {0}", GetExportName(structureInfo));
            ilWriter.StartBlock();
            {
                for (int i = 0; i < structureInfo.memberList.Count; ++i)
                {
                    ClassMemberInfo memberInfo = structureInfo.memberList[i];
                    StructureInfo memberStructureInfo = memberInfo.structureInfo;
                    ListStructureInfo memberListInfo = memberStructureInfo as ListStructureInfo;
                    if (memberListInfo.valueStructureInfo.isCollection)
                    {
                        throw new Exception();
                    }
                    ilWriter.AppendLine("repeated {0} {1} = {2};", GetExportName(memberListInfo.valueStructureInfo), memberInfo.name, i + 1);
                    writerGroup.AddImportGroup(GetExportGroup(memberListInfo.valueStructureInfo.name));
                }
            }
            ilWriter.EndBlock();
        }

        private T GetStructureInfo<T>(string name, List<T> list) where T : StructureInfo
        {
            foreach (T structureInfo in list)
            {
                if (structureInfo.name == name)
                {
                    return structureInfo;
                }
            }
            return null;
        }

        private void AddStructureNameToList<T>(List<string> structureNameList, List<T> structureList) where T : StructureInfo
        {
            int count = structureList.Count;
            for (int i = 0; i < count; ++i)
            {
                string structureName = structureList[i].name;
                if (!structureNameList.Contains(structureName))
                {
                    structureNameList.Add(structureName);
                }
            }
        }

        private DataStructureExportGroup GetExportGroup(string structureName)
        {
            DataStructureExportGroup exportGroup;
            m_exportGroupMap.TryGetValue(structureName, out exportGroup);
            return exportGroup;
        }

        protected override string GetExportName(StructureInfo structureInfo)
        {
            switch (structureInfo.structureType)
            {
                case EStructureType.Basic:
                    BasicStructureInfo basicStructureInfo = structureInfo as BasicStructureInfo;
                    return GetExportName(basicStructureInfo.basicStructureType);
                case EStructureType.Class:
                    return m_context.prefixStr + structureInfo.name + m_context.postfixStr;
                case EStructureType.Pack:
                    return m_context.prefixStr + (structureInfo as PackStructureInfo).classStructureInfo.name + m_context.postfixStr + "List";
            }
            return structureInfo.name;
        }

        protected override string GetExportName(EBasicStructureType type)
        {
            switch (type)
            {
                case EBasicStructureType.Bool:
                    return "bool";
                case EBasicStructureType.Int8:
                case EBasicStructureType.Int16:
                case EBasicStructureType.Int32:
                    return "int32";
                case EBasicStructureType.UInt8:
                case EBasicStructureType.UInt16:
                case EBasicStructureType.UInt32:
                    return "uint32";
                case EBasicStructureType.Int64:
                    return "int64";
                case EBasicStructureType.UInt64:
                    return "uint64";
                case EBasicStructureType.Single:
                    return "float";
                case EBasicStructureType.Double:
                    return "double";
                case EBasicStructureType.String:
                    return "string";
            }
            throw new Exception("");
        }

        private void WriteIlFile()
        {
            m_context.stopwatchCollector.Start("WriteIlFile");
            foreach (var kv in m_ilWriterGroupMap)
            {
                string ilPath = FilePathUtility.GetPath(m_context.ilExportFolderPath, kv.Value.exportGroup.name + ".proto");
                m_context.codeFilePathList.Add(FilePathUtility.GetPath(m_context.structureExportFolderPath, kv.Value.exportGroup.name + ".cs"));
                kv.Value.WriteImportBlock();
                kv.Value.writer.WriteFile(ilPath);
                m_ilPathList.Add(ilPath);
            }
            m_context.stopwatchCollector.Stop();
        }

        //private void RunProtoc()
        //{
        //    m_context.stopwatchCollector.Start("RunProtoc");
        //    StringBuilder sb = new StringBuilder();
        //    int index = 0;
        //    for (int i = 0; i < m_ilPathList.Count; ++i)
        //    {
        //        index++;
        //        string ilPath = m_ilPathList[i];
        //        if (index != 0)
        //        {
        //            sb.Append(" ");
        //        }
        //        sb.Append(ilPath);
        //        if (sb.Length > (1<<15) - 1000 || i == m_ilPathList.Count -1)
        //        {
        //            string argStr = string.Format("--proto_path={2} --csharp_out={0} {1}", m_context.structureExportFolderPath, sb, m_context.ilExportFolderPath);
        //            string errorMsg;
        //            if (!RunExe(m_context.structureExporterPath, argStr, out errorMsg))
        //            {
        //                throw new Exception(errorMsg);
        //            }
        //            index = 0;
        //            sb = new StringBuilder();
        //        }
        //    }
        //    m_context.stopwatchCollector.Stop();
        //}

        private void RunProtoc()
        {
            m_context.stopwatchCollector.Start("RunProtoc");
            string cmdFilePath = FilePathUtility.GetPath(EFilePathSlashType.RightDown, m_context.tempFileFolderPath, "proto.txt");
            StringBuilder sb = new StringBuilder();
            m_ilPathList.Sort();
            foreach (var ilPath in m_ilPathList)
            {
                //sb.Append("./");
                //sb.Append(FilePathUtility.GetFileName(ilPath));
                //sb.Append(" ");
                sb.Append(ilPath);
                sb.Append("\r\n");
            }
            File.WriteAllText(cmdFilePath, sb.ToString());
            //string exePath = FilePathUtility.GetPath(m_context.ilExportFolderPath, "protoc.exe");
            //File.Copy(m_context.structureExportFolderPath, exePath);
            string argStr = string.Format("--proto_path={2} --csharp_out={0} @{1}", m_context.structureExportFolderPath, cmdFilePath, m_context.ilExportFolderPath);
            //string argStr = string.Format("@{0} --csharp_out={1}", cmdFilePath, m_context.structureExportFolderPath);
            string errorMsg;
            if (!RunExe(m_context.structureExporterPath, argStr, out errorMsg))
            {
                throw new Exception(errorMsg);
            }
            m_context.stopwatchCollector.Stop();
        }

        private bool RunExe(string fileName, string argument, out string errorMsg)
        {
            bool result = false;
            if (string.IsNullOrEmpty(fileName))
            {
                errorMsg = "File Name is Empty";
                return false;
            }
            Process process = new Process();
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = fileName;
            startInfo.Arguments = argument;
            startInfo.UseShellExecute = false;
            startInfo.RedirectStandardInput = false;
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = true;
            startInfo.CreateNoWindow = true;
            process.StartInfo = startInfo;
            errorMsg = string.Empty;
            try
            {
                if (process.Start())
                {
                    errorMsg = process.StandardError.ReadToEnd();
                    if (string.IsNullOrEmpty(errorMsg))
                    {
                        result = true;
                    }
                    process.WaitForExit();
                }
            }
            catch (Exception e)
            {
                errorMsg = e.Message;
            }
            finally
            {
                if (process != null)
                {
                    process.Close();
                }
            }
            return result;
        }

        private bool HasAnyStructureNeedExport(DataStructureExportGroup exportGroup, List<string> ignoreStructureNameList)
        {
            for (int i = 0; i < exportGroup.structureNameList.Count; ++i)
            {
                if (!ignoreStructureNameList.Contains(exportGroup.structureNameList[i]))
                {
                    return true;
                }
            }
            return false;
        }

        private class WriterGroup
        {
            public FileWriter writer = new FileWriter();
            public FileWriterBlockItemBlock importBlockWriter;
            public DataStructureExportGroup exportGroup;
            public List<DataStructureExportGroup> importExportGroupList = new List<DataStructureExportGroup>();

            public WriterGroup(DataStructureExportGroup exportGroup)
            {
                this.exportGroup = exportGroup;
            }

            public void CreateAndInsertBlockItem()
            {
                importBlockWriter = writer.CreateAndInsertBlockItem();
            }

            public void WriteImportBlock()
            {
                for (int i = 0; i < importExportGroupList.Count; ++i)
                {
                    importBlockWriter.AppendLine("import \"{0}.proto\";", importExportGroupList[i].name);
                }
            }

            public void AddImportGroup(DataStructureExportGroup group)
            {
                if (group == null)
                {
                    return;
                }
                if (group == exportGroup)
                {
                    return;
                }
                if (!importExportGroupList.Contains(group))
                {
                    importExportGroupList.Add(group);
                }
            }
        }
    }
}