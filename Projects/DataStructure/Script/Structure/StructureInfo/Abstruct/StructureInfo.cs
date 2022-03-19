namespace MonMoose.Core.Structure
{
    public abstract class StructureInfo
    {
        protected string m_name;
        protected string m_searchName;

        public string name { get { return m_name; } }
        public virtual string searchName { get { return m_searchName; } }

        public abstract bool isCollection { get; }
        public abstract EStructureType structureType { get; }
        public abstract bool isValid { get; }

        protected StructureInfo(string name)
        {
            m_name = name;
            m_searchName = name.ToLower();
        }

        public void Rename(string name)
        {
            m_name = name;
            m_searchName = name.ToLower();
        }
    }
}
