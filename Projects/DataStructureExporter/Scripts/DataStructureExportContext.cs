using System.Collections.Generic;
using MonMooseCore.Structure;

namespace MonMooseCore.DataExporter
{
    public class DataStructureExportContext
    {
        public string namespaceStr;
        public string prefixStr;
        public string postfixStr;
        public string structureExportFolderPath;
        public string ilExportFolderPath;
        public string structureExporterPath;
        public string singleFileExportGroupName = "Structure";
        public List<DataStructureExportGroup> exportGroupList = new List<DataStructureExportGroup>();
        public StopwatchCollector stopwatchCollector;
        public StructureManager structureManager;
        public List<MemberInfo> ignoreMemberInfoList = new List<MemberInfo>();
        public List<string> ignoreStructureNameList = new List<string>();
        public List<string> codeFilePathList = new List<string>();
    }
}
