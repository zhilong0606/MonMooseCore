namespace MonMooseCore.Structure
{
    public class DictionaryStructureInfo : CollectionStructureInfo
    {
        protected StructureInfo m_keyStructureInfo;
        protected StructureInfo m_valueStructureInfo;

        public StructureInfo keyStructureInfo { get { return m_keyStructureInfo; } }
        public StructureInfo valueStructureInfo { get { return m_valueStructureInfo; } }

        public sealed override EStructureType structureType { get { return EStructureType.Dictionary; } }

        public DictionaryStructureInfo(StructureInfo keyStructureInfo, StructureInfo valueStructureInfo) : base(string.Format("Dictionary<{0},{1}>", keyStructureInfo.name, valueStructureInfo.name))
        {
            m_keyStructureInfo = keyStructureInfo;
            m_valueStructureInfo = valueStructureInfo;
        }
    }
}
