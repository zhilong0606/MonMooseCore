using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MonMooseCore.Structure
{
    public class ClassStructureMemberStructureException : Exception
    {
        public EErrorId errorId;
        public string structureName;
        public string memberName;
        public string memberStructureName;

        private const string m_msgFormat = "{0} {1} {2} {3}";

        public ClassStructureMemberStructureException(EErrorId errorId, string structureName, string memberName, string memberStructureName)
            : base(string.Format(m_msgFormat, errorId, structureName, memberName, memberStructureName))
        {
            this.errorId = errorId;
            this.structureName = structureName;
            this.memberName = memberName;
            this.memberStructureName = memberStructureName;
        }

        public enum EErrorId
        {
            NestCollections,
        }
    }
}