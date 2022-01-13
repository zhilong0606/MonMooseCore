using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Structure;

public class FlatStructureExporter : StructureExporter
{
    protected override void OnExport()
    {
    }

    protected override string GetExportName(BaseStructureInfo structureInfo)
    {
        throw new NotImplementedException();
    }

    protected override string GetExportName(EBasicStructureType type)
    {
        throw new NotImplementedException();
    }
}
