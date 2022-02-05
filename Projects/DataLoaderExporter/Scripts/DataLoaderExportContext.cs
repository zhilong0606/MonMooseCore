using MonMooseCore;
using MonMooseCore.Data;
using MonMooseCore.Structure;

namespace MonMooseCore.DataExporter
{
    public class DataLoaderExportContext
    {
        public string namespaceStr;
        public string prefixStr;
        public string postfixStr;
        public string loaderExportFolderPath;
        public string usingNamespaceStr;
        public StopwatchCollector stopwatchCollector;
        public DataObjectManager dataObjManager;
        public StructureManager structureManager;
    }
}
