using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MonMooseCore.Structure
{
    public static class StructureUtility
    {
        private const string m_listStructureNameFormat = "List<{0}>";
        private const string m_packStructureNameFormat = "{0}List";
        private const string m_enumIdStructureNameFormat = "E{0}Id";

        public static string GetListStructureName(string valueStructureName)
        {
            return string.Format(m_listStructureNameFormat, valueStructureName);
        }

        public static string GetPackStructureName(string itemStructureName)
        {
            return string.Format(m_packStructureNameFormat, itemStructureName);
        }

        public static string GetEnumIdStructureName(string itemStructureName)
        {
            return string.Format(m_enumIdStructureNameFormat, itemStructureName);
        }
    }
}
