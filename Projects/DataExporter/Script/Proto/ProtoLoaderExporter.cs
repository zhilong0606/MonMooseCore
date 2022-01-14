namespace MonMooseCore.DataExporter
{
    public class ProtoLoaderExporter : LoaderExporter
    {
        private FileWriter m_loaderWriter = new FileWriter();

        private const string m_outputName = "StaticDataManager_Proto";
        private const string m_initLoaderFormat = "m_loaderMap.Add(\"{0}\", new ProtoDataLoader<{1}{0}{2}, {1}{0}{2}List>(m_{3}List, {1}{0}{2}List.Parser.ParseFrom, (fromList, toList) => {{ toList.Clear(); toList.AddRange(fromList.List); }}));";
        private const string m_annotationFormat = "//{0}";
        private const string m_listDefineFormat = "private List<{1}{0}{2}> m_{3}List = new List<{1}{0}{2}>();";
        private const string m_getItemFuncFormat = "public {1}{0}{2} Get{0}(int id) {{ foreach (var info in m_{3}List) if (info.Id == id) return info; return null; }}";
        private const string m_getEnumIdItemFuncFormat = "public {1}{0}{2} Get{0}(E{0}Id id) {{ foreach (var info in m_{3}List) if (info.Id == (int)id) return info; return null; }}";
        private const string m_getEnumeratorFuncFormat = "public IEnumerable<{1}{0}{2}> {3}List {{ get {{ return m_{3}List; }} }}";
        private const string m_getItemCountFuncFormat = "public int {0}Count {{ get {{ return m_{0}List.Count; }} }}";

        protected override void OnExport()
        {
            m_loaderWriter.AppendLine("using System.Collections.Generic;");
            m_loaderWriter.AppendLine(string.Format("using {0};", m_context.namespaceStr));
            m_loaderWriter.AppendLine("");
            m_loaderWriter.AppendLine(string.Format("namespace {0}", m_context.usingNamespaceStr));
            m_loaderWriter.AppendLine("{");
            m_loaderWriter.StartTab();
            {
                m_loaderWriter.AppendLine("public partial class StaticDataManager");
                m_loaderWriter.AppendLine("{");
                m_loaderWriter.StartTab();
                {
                    m_loaderWriter.AppendLine("partial void OnInitLoaders()");
                    m_loaderWriter.AppendLine("{");
                    m_loaderWriter.StartTab();
                    {
                        foreach (var kv in m_context.dataObjManager.structureMap)
                        {
                            m_loaderWriter.AppendLine(string.Format(m_initLoaderFormat, kv.Key.name, m_context.prefixStr, m_context.postfixStr, ChangeFirstToLower(kv.Key.name)));
                        }
                    }
                    m_loaderWriter.EndTab();
                    m_loaderWriter.AppendLine("}");
                    foreach (var kv in m_context.dataObjManager.structureMap)
                    {
                        string structureName = kv.Key.name;
                        m_loaderWriter.AppendLine("");
                        m_loaderWriter.AppendLine(string.Format(m_annotationFormat, structureName));
                        m_loaderWriter.AppendLine(string.Format(m_listDefineFormat, structureName, m_context.prefixStr, m_context.postfixStr, ChangeFirstToLower(structureName)));
                        m_loaderWriter.AppendLine(string.Format(kv.Key.isEnumId ? m_getEnumIdItemFuncFormat : m_getItemFuncFormat, structureName, m_context.prefixStr, m_context.postfixStr, ChangeFirstToLower(structureName)));
                        m_loaderWriter.AppendLine(string.Format(m_getEnumeratorFuncFormat, structureName, m_context.prefixStr, m_context.postfixStr, ChangeFirstToLower(structureName)));
                        m_loaderWriter.AppendLine(string.Format(m_getItemCountFuncFormat, ChangeFirstToLower(structureName)));
                    }
                }
                m_loaderWriter.EndTab();
                m_loaderWriter.AppendLine("}");
            }
            m_loaderWriter.EndTab();
            m_loaderWriter.AppendLine("}");

            m_loaderWriter.WriteFile(m_context.loaderExportFolderPath + m_outputName + ".cs");
        }

        private static string ChangeFirstToLower(string str)
        {
            return str.Substring(0, 1).ToLower() + str.Substring(1);
        }

        private static string ChangeFirstToUpper(string str)
        {
            return str.Substring(0, 1).ToUpper() + str.Substring(1);
        }

    }
}