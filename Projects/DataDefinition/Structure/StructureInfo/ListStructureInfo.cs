using System;
using System.Collections.Generic;

namespace Structure
{
    public class ListStructureInfo : CollectionStructureInfo
    {
        protected StructureInfo m_valueStructureInfo;

        public StructureInfo valueStructureInfo { get { return m_valueStructureInfo; } }

        public sealed override EStructureType structureType { get { return EStructureType.List; } }

        public ListStructureInfo(StructureInfo valueStructureInfo) : base(string.Format("List<{0}>", valueStructureInfo.name))
        {
            m_valueStructureInfo = valueStructureInfo;
        }
    }
}
