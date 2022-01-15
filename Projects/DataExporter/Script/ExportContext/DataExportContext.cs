using MonMooseCore.Data;
using MonMooseCore.Structure;

namespace MonMooseCore.DataExporter
{
    public class DataExportContext : ExportContext
    {
        public string namespaceStr;
        public string prefixStr;
        public string postfixStr;
        public string dataExportFolderPath;
        public string dataExportExtensionStr;
        public StructureManager structureManager;
        public DataObjectManager dataObjManager;
    }
}
