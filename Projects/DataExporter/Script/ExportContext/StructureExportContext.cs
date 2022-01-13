using System;
using System.Collections.Generic;
using System.Text;
using Structure;

namespace MonMooseCore.DataExporter
{
    public class StructureExportContext : ExportContext
    {
        public string name;
        public string namespaceStr;
        public string prefixStr;
        public string postfixStr;
        public string structureExportPath;
        public StructureManager structureManager;
    }
}
