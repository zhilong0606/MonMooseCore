using MonMooseCore.Structure;

namespace MonMooseCore.DataExporter
{
    public class StructureExportContext : ExportContext
    {
        public string namespaceStr;
        public string prefixStr;
        public string postfixStr;
        public string structureExportFolderPath;
        public string ilExportFolderPath;
        public string structureExporterPath;
        public StructureManager structureManager;
    }
}
