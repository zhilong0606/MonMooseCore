using System;
using System.Collections.Generic;

namespace Structure
{
    public abstract class CollectionStructureInfo : StructureInfo
    {
        public sealed override bool isCollection { get { return true; } }

        protected CollectionStructureInfo(string name) : base(name)
        {
        }
    }
}
