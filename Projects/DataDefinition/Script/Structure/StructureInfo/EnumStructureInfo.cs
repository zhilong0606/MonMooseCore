using System;
using System.Collections.Generic;

namespace Structure
{
    public class EnumStructureInfo : MemberedStructureInfo<EnumMemberInfo>
    {
        public sealed override EStructureType structureType { get { return EStructureType.Enum; } }

        public EnumStructureInfo(string name) : base(name)
        {
        }

        public bool CanAddMember(int memberIndex, string memberName)
        {
            EnumMemberInfo memberInfo = GetMember(memberName);
            if (memberInfo != null)
            {
                if (memberInfo.index != memberIndex)
                {
                    throw new Exception(string.Format("相同枚举名，类型名：{0}，枚举名：{1}", name, memberName));
                }
                return false;
            }
            memberInfo = GetMember(memberIndex);
            if (memberInfo != null)
            {
                if (memberInfo.name != memberName)
                {
                    throw new Exception(string.Format("相同枚举序号，类型名：{0}，序号：{1}", name, memberIndex));
                }
                return false;
            }
            return true;
        }

        public override bool CanAddMember(EnumMemberInfo memberInfo)
        {
            if (memberInfo != null)
            {
                return CanAddMember(memberInfo.index, memberInfo.name);
            }
            return false;
        }

        public EnumMemberInfo GetMember(int memberIndex)
        {
            foreach (EnumMemberInfo memberInfo in m_memberList)
            {
                if (memberInfo.index == memberIndex)
                {
                    return memberInfo;
                }
            }
            return null;
        }

        public EnumMemberInfo AddMember(int memberIndex, string memberName)
        {
            if (CanAddMember(memberIndex, memberName))
            {
                EnumMemberInfo memberInfo = new EnumMemberInfo(memberName, memberIndex);
                AddMember(memberInfo);
                return memberInfo;
            }
            return null;
        }

        public void SortMember()
        {
            m_memberList.Sort((x, y) => x.index.CompareTo(y.index));
        }
    }
}
