using MonMooseCore.Data;
using MonMooseCore.Structure;

namespace MonMooseCore.DataExporter
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
        public bool useMultiTask;
        public StopwatchCollector stopwatchCollector;
        public StructureManager structureManager;
        public DataObjectManager dataObjManager;
    }
}
