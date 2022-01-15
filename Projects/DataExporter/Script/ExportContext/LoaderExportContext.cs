using MonMooseCore.Data;
using MonMooseCore.Structure;

namespace MonMooseCore.DataExporter
{
    public class LoaderExportContext : ExportContext
    {
        public string namespaceStr;
        public string prefixStr;
        public string postfixStr;
        public string loaderExportFolderPath;
        public string usingNamespaceStr;
        public StructureManager structureManager;
        public DataObjectManager dataObjManager;
    }
}
