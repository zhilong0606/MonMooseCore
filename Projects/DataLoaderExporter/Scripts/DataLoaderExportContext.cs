using MonMoose.Core;
using MonMoose.Core.Data;
using MonMoose.Core.Structure;

namespace MonMoose.Core.DataExporter
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
