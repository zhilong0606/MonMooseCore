using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Structure;

namespace MonMooseCore.DataExporter
{
    public abstract class StructureExporter : Exporter<StructureExportContext>
    {
        protected override void OnExport()
        {

        }

        protected abstract string GetExportName(StructureInfo structureInfo);
        protected abstract string GetExportName(EBasicStructureType type);
    }
}