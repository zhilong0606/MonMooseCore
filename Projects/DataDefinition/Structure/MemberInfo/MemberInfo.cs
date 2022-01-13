using System;
using System.Collections.Generic;

namespace Structure
{
    public abstract class MemberInfo
    {
        protected string m_name;

        public string name
        {
            get { return m_name; }
        }

        protected MemberInfo(string name)
        {
            m_name = name;
        }

        public void Rename(string name)
        {
            m_name = name;
        }
    }
}
