using System.Collections.Generic;
using MonMoose.Core.Data;
using MonMoose.Core.Structure;

namespace MonMoose.Core.DataExporter
{
    public class DataObjectExportContext
    {
        public string namespaceStr;
        public string prefixStr;
        public string postfixStr;
        public string structureExportPath;
        public string dataExportFolderPath;
        public string dataExportExtensionStr;
        public string dataExportDllPath;
        public string idName;
        public List<string> codeFilePathList = new List<string>();
        public bool useMultiTask;
        public StopwatchCollector stopwatchCollector;
        public StructureManager structureManager;
        public DataObjectManager dataObjManager;
    }
}
