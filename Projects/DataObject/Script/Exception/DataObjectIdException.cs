using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MonMooseCore.Data
{
    public class DataObjectIdException : Exception
    {
        public EErrorId errorId;
        public string structureName;
        public int dataObjectId;

        private const string m_msgFormat = "{0} {1} {2}";

        public DataObjectIdException(EErrorId errorId, string structureName, int dataObjectId)
            : base(string.Format(m_msgFormat, errorId, structureName, dataObjectId))
        {
            this.errorId = errorId;
            this.structureName = structureName;
            this.dataObjectId = dataObjectId;
        }

        public enum EErrorId
        {
            SameDataObjectId,
        }
    }
}
