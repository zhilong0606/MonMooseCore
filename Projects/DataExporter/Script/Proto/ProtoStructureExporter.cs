using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using MonMooseCore.FileUtility;
using Structure;

namespace MonMooseCore.DataExporter
{
    public class ProtoStructureExporter : StructureExporter
    {
        private List<EnumStructureInfo> m_enumList = new List<EnumStructureInfo>();
        private List<ClassStructureInfo> m_classList = new List<ClassStructureInfo>();
        private List<ListStructureInfo> m_listList = new List<ListStructureInfo>();
        private List<DictionaryStructureInfo> m_dictionaryList = new List<DictionaryStructureInfo>();
        private List<PackStructureInfo> m_packList = new List<PackStructureInfo>();

        private FileWriter m_ilWriter = new FileWriter();

        private const string m_outputName = "Structure";
        private string m_protoOutputPath;
        private string m_csOutputPath;

        protected override void OnExport()
        {
            m_protoOutputPath = FolderManager.Instance.GetSubDirPath(m_context.name + "/" + m_context.exportMode.modeType.ToString(), EFolderType.IL);
            PathUtility.TryGetRelativePosition(m_protoOutputPath, out m_protoOutputPath);
            m_csOutputPath = FolderManager.Instance.GetSubDirPath(m_context.name + "/" + m_context.exportMode.modeType.ToString(), EFolderType.Code) + "\\";
            m_csOutputPath = m_csOutputPath.Replace("\\", "/");

            CollectStructureLists();
            ExportHead();
            ExportEnum();
            ExportClass();
            ExportPack();
            WriteIlFile();
            RunProtoc();
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

        private void ExportHead()
        {
            m_ilWriter.AppendLine("syntax = \"proto3\";");
            if (!string.IsNullOrEmpty(m_context.namespaceStr))
            {
                m_ilWriter.AppendLine(string.Format("package {0};", m_context.namespaceStr));
            }
        }

        private void ExportEnum()
        {
            for (int i = 0; i < m_enumList.Count; ++i)
            {
                EnumStructureInfo enumInfo = m_enumList[i];
                SendMsg((double) i / (m_enumList.Count + m_classList.Count), string.Format("正在导出枚举:{0} ({1}/{2})", enumInfo.name, i, m_enumList.Count));
                m_ilWriter.AppendLine();
                m_ilWriter.StartLine("enum").AppendSpace().Append(GetExportName(enumInfo)).EndLine();
                m_ilWriter.StartCodeBlock();
                {
                    foreach (EnumMemberInfo memberInfo in enumInfo.memberList)
                    {
                        m_ilWriter.StartLine(enumInfo.name).Append("_").Append(memberInfo.name).Append(" = ").Append(memberInfo.index.ToString()).Append(";").EndLine();
                    }
                }
                m_ilWriter.EndCodeBlock();
            }
        }

        private void ExportClass()
        {
            for (int i = 0; i < m_classList.Count; ++i)
            {
                ClassStructureInfo classInfo = m_classList[i];
                SendMsg((double) (i + m_enumList.Count) / (m_enumList.Count + m_classList.Count), string.Format("正在导出类型:{0} ({1}/{2})", classInfo.name, i, m_classList.Count));
                m_ilWriter.AppendLine();
                m_ilWriter.StartLine("message").AppendSpace().Append(GetExportName(classInfo)).EndLine();
                m_ilWriter.StartCodeBlock();
                {
                    for (int j = 0; j < classInfo.memberList.Count; ++j)
                    {
                        ClassMemberInfo memberInfo = classInfo.memberList[j];
                        m_ilWriter.StartLine();
                        StructureInfo memberStructureInfo = memberInfo.structureInfo;
                        switch (memberStructureInfo.structureType)
                        {
                            case EStructureType.Basic:
                            case EStructureType.Enum:
                            case EStructureType.Class:
                                m_ilWriter.Append(GetExportName(memberStructureInfo));
                                break;
                            case EStructureType.List:
                                ListStructureInfo memberListInfo = memberStructureInfo as ListStructureInfo;
                                if (!memberListInfo.valueStructureInfo.isCollection)
                                {
                                    m_ilWriter.Append("repeated").AppendSpace().Append(GetExportName(memberListInfo.valueStructureInfo));
                                }
                                else
                                {
                                    throw new Exception(string.Format("类型不能同时拥有多个组合，类型名：{0}，成员名：{1}, 成员类型名：{2}", classInfo.name, memberInfo.name, memberStructureInfo.name));
                                }
                                break;
                            case EStructureType.Dictionary:
                                DictionaryStructureInfo memberDictionaryInfo = memberStructureInfo as DictionaryStructureInfo;
                                if (!memberDictionaryInfo.keyStructureInfo.isCollection && !memberDictionaryInfo.valueStructureInfo.isCollection)
                                {
                                    m_ilWriter.Append("map").Append("<").Append(GetExportName(memberDictionaryInfo.keyStructureInfo)).Append(",").Append(GetExportName(memberDictionaryInfo.valueStructureInfo)).Append(">");
                                }
                                else
                                {
                                    throw new Exception(string.Format("类型不能同时拥有多个组合，类型名：{0}，成员名：{1}, 成员类型名：{2}", classInfo.name, memberInfo.name, memberStructureInfo.name));
                                }
                                break;
                            default:
                                throw new Exception("");
                        }
                        m_ilWriter.AppendSpace().Append(memberInfo.name).Append(" = ").Append((j + 1).ToString());
                        m_ilWriter.Append(";").EndLine();
                    }
                }
                m_ilWriter.EndCodeBlock();
            }
        }

        private void ExportPack()
        {
            for (int i = 0; i < m_packList.Count; ++i)
            {
                PackStructureInfo packInfo = m_packList[i];
                SendMsg((double) (i + m_enumList.Count) / (m_enumList.Count + m_classList.Count), string.Format("正在导出类型:{0} ({1}/{2})", packInfo.name, i, m_classList.Count));
                m_ilWriter.AppendLine();
                m_ilWriter.StartLine("message").AppendSpace().Append(GetExportName(packInfo)).EndLine();
                m_ilWriter.StartCodeBlock();
                {
                    for (int j = 0; j < packInfo.memberList.Count; ++j)
                    {
                        ClassMemberInfo memberInfo = packInfo.memberList[j];
                        m_ilWriter.StartLine();
                        StructureInfo memberStructureInfo = memberInfo.structureInfo;
                        ListStructureInfo memberListInfo = memberStructureInfo as ListStructureInfo;
                        if (!memberListInfo.valueStructureInfo.isCollection)
                        {
                            m_ilWriter.Append("repeated").AppendSpace().Append(GetExportName(memberListInfo.valueStructureInfo));
                        }
                        m_ilWriter.AppendSpace().Append(memberInfo.name).Append(" = ").Append((j + 1).ToString());
                        m_ilWriter.Append(";").EndLine();
                    }
                }
                m_ilWriter.EndCodeBlock();
            }
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
                case EBasicStructureType.Int64:
                    return "int64";
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
            m_ilWriter.WriteFile(m_protoOutputPath + m_outputName + ".proto");
        }

        private void RunProtoc()
        {
            string protoOutputFilePath = m_protoOutputPath + m_outputName + ".proto";
            string csOutputFilePath = m_csOutputPath + m_outputName + ".cs";
            string argStr = string.Format("--csharp_out={0} {1}", m_csOutputPath, protoOutputFilePath);
            string errorMsg;
            if (!Utility.RunExe("protoc", argStr, out errorMsg))
            {
                throw new Exception(errorMsg);
            }
            m_context.assembly = GetCompilerAssembly(new string[] {csOutputFilePath});
            if (!string.IsNullOrEmpty(m_context.structureExportPath) && Directory.Exists(m_context.structureExportPath))
            {
                File.Copy(csOutputFilePath, m_context.structureExportPath + m_outputName + ".cs", true);
            }
        }

        protected Assembly GetCompilerAssembly(string[] codeFiles)
        {
            CodeDomProvider complier = CodeDomProvider.CreateProvider("CSharp");
            CompilerParameters param = new CompilerParameters();
            param.GenerateExecutable = false;
            param.GenerateInMemory = true;
            param.TreatWarningsAsErrors = true;
            param.IncludeDebugInformation = false;
            param.ReferencedAssemblies.Add("./Protobuf.dll");
            param.ReferencedAssemblies.Add("System.dll");

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
    }
}