namespace MonMooseCore.Structure
{
    public class ClassMemberInfo : MemberInfo
    {
        protected StructureInfo m_structureInfo;

        public StructureInfo structureInfo
        {
            get { return m_structureInfo; }
        }

        public ClassMemberInfo(StructureInfo structureInfo, string name) : base (name)
        {
            m_structureInfo = structureInfo;
        }
    }
}
