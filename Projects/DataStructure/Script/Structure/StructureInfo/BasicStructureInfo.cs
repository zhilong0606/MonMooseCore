namespace MonMooseCore.Structure
{
    public class BasicStructureInfo : StructureInfo
    {
        protected EBasicStructureType m_basicStructureType;
        protected string m_exportName;

        public sealed override bool isCollection { get { return false; } }
        public sealed override bool isValid { get { return true; } }
        public sealed override EStructureType structureType { get { return EStructureType.Basic; } }
        public EBasicStructureType basicStructureType { get { return m_basicStructureType; } }
        public string exportName { get { return m_exportName; } }

        public BasicStructureInfo(string name, EBasicStructureType basicStructureType, string exportName) : base(name)
        {
            m_basicStructureType = basicStructureType;
            m_exportName = exportName;
        }
    }
}
