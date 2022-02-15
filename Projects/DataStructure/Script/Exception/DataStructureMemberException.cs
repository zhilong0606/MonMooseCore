using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MonMooseCore.Structure
{
    public class DataStructureMemberException : Exception
    {
        public EErrorId errorId;
        public string structureName;
        public string memberName;

        private const string m_msgFormat = "{0} {1} {2}";

        public DataStructureMemberException(EErrorId errorId, string structureName, string memberName)
            : base(string.Format(m_msgFormat, errorId, structureName, memberName))
        {
            this.errorId = errorId;
            this.structureName = structureName;
            this.memberName = memberName;
        }

        public enum EErrorId
        {
            SameMemberName,
            SameEnumMemberIndex,
            CannotAddMember,
        }
    }
}
