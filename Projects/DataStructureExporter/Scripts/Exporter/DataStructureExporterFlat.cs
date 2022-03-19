using System;
using MonMoose.Core.Structure;

namespace MonMoose.Core.DataExporter
{
    public class DataStructureExporterFlat : DataStructureExporter
    {
        protected override void OnExport()
        {
        }

        protected override string GetExportName(StructureInfo structureInfo)
        {
            throw new NotImplementedException();
        }

        protected override string GetExportName(EBasicStructureType type)
        {
            throw new NotImplementedException();
        }
    }
}