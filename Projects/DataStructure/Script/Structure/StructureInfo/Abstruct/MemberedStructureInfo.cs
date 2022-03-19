using System;
using System.Collections.Generic;

namespace MonMoose.Core.Structure
{
    public abstract class MemberedStructureInfo<T> : MemberedStructureInfo
        where T : MemberInfo
    {
        protected List<T> m_memberList = new List<T>();

        public List<T> memberList
        {
            get { return m_memberList; }
        }
        
        protected MemberedStructureInfo(string name) : base(name)
        {
        }

        public virtual bool CanAddMember(T memberInfo)
        {
            return true;
        }

        public T GetMember(string memberName)
        {
            for (int i = 0; i < m_memberList.Count; ++i)
            {
                if (m_memberList[i].name == memberName)
                {
                    return m_memberList[i];
                }
            }
            return null;
        }

        public void AddMember(T memberInfo)
        {
            m_memberList.Add(memberInfo);
        }

        public override MemberInfo GetBaseMember(string memberName)
        {
            return GetMember(memberName);
        }

        public override void AddBaseMember(MemberInfo memberInfo)
        {
            T info = memberInfo as T;
            if (info != null && CanAddMember(info))
            {
                AddMember(info);
            }
            else
            {
                throw new DataStructureMemberException(DataStructureMemberException.EErrorId.CannotAddMember, name, memberInfo.name);
            }
        }
    }

    public abstract class MemberedStructureInfo : StructureInfo
    {
        public sealed override bool isCollection { get { return false; } }
        public override bool isValid { get { return !string.IsNullOrEmpty(name); } }

        protected MemberedStructureInfo(string name) : base(name)
        {
        }

        public bool HasMember(string memberName)
        {
            return GetBaseMember(memberName) != null;
        }

        public abstract MemberInfo GetBaseMember(string memberName);
        public abstract void AddBaseMember(MemberInfo memberInfo);
    }
}
