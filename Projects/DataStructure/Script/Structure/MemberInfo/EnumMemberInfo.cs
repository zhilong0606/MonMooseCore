namespace MonMooseCore.Structure
{
    public class EnumMemberInfo : MemberInfo
    {
        protected int m_index;

        public int index
        {
            get { return m_index; }
        }

        public EnumMemberInfo(string name, int index) : base (name)
        {
            m_index = index;
        }
    }
}
