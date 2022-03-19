namespace MonMoose.Core.Structure
{
    public class ListStructureInfo : CollectionStructureInfo
    {
        protected StructureInfo m_valueStructureInfo;

        public override bool isValid
        {
            get { return m_valueStructureInfo != null; }
        }

        public StructureInfo valueStructureInfo { get { return m_valueStructureInfo; } }

        public sealed override EStructureType structureType { get { return EStructureType.List; } }

        public ListStructureInfo(StructureInfo valueStructureInfo) : base(StructureUtility.GetListStructureName(valueStructureInfo.name))
        {
            m_valueStructureInfo = valueStructureInfo;
        }
    }
}
