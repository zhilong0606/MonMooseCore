using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Structure;

public abstract class StructureExporter : Exporter
{
    protected override void OnExport()
    {

    }

    protected abstract string GetExportName(BaseStructureInfo structureInfo);
    protected abstract string GetExportName(EBasicStructureType type);
}
