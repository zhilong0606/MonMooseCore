using System;

namespace MonMooseCore.Structure
{
    public class ClassStructureInfo : MemberedStructureInfo<ClassMemberInfo>
    {
        public sealed override EStructureType structureType { get { return EStructureType.Class; } }
        
        public ClassStructureInfo(string name) : base(name) { }
        
        public bool CanAddMember(StructureInfo structureInfo, string memberName)
        {
            ClassMemberInfo memberInfo = GetMember(memberName);
            if (memberInfo != null)
            {
                if (memberInfo.structureInfo != structureInfo)
                {
                    throw new Exception(string.Format("相同类成员名，类型名：{0}，成员名：{1}", name, memberName));
                }
                return false;
            }
            return true;
        }

        public override bool CanAddMember(ClassMemberInfo memberInfo)
        {
            if (memberInfo != null)
            {
                return CanAddMember(memberInfo.structureInfo, memberInfo.name);
            }
            return false;
        }

        public ClassMemberInfo AddMember(StructureInfo strcutureInfo, string memberName)
        {
            if (CanAddMember(strcutureInfo, memberName))
            {
                ClassMemberInfo memberInfo = new ClassMemberInfo(strcutureInfo, memberName);
                AddMember(memberInfo);
                return memberInfo;
            }
            return null;
        }
    }
}
