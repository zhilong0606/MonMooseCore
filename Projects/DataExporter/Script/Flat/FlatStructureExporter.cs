using System;
using MonMooseCore.Structure;

namespace MonMooseCore.DataExporter
{
    public class FlatStructureExporter : StructureExporter
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