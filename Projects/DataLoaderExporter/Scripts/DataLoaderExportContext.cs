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
        public DataObjectManager dataObjManager;
        public StructureManager structureManager;
    }
}
