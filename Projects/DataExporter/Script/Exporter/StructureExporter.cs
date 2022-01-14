using MonMooseCore.Structure;

namespace MonMooseCore.DataExporter
{
    public abstract class StructureExporter : Exporter<StructureExportContext, StructureExportResult>
    {
        protected override void OnExport()
        {

        }

        protected abstract string GetExportName(StructureInfo structureInfo);
        protected abstract string GetExportName(EBasicStructureType type);
    }
}