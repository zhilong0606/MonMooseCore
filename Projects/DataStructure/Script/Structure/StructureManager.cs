using System.Collections.Generic;

namespace MonMooseCore.Structure
{
    public class StructureManager
    {
        public Dictionary<string, StructureInfo> structureMap = new Dictionary<string, StructureInfo>();
        private Dictionary<int, BasicStructureInfo> m_basicStructureInfoMap = new Dictionary<int, BasicStructureInfo>();

        public void Init(Dictionary<int, string> overrideBasicStructureNameMap = null)
        {
            Dictionary<int, string> basicNameMap = new Dictionary<int, string>()
            {
                {(int)EBasicStructureType.Bool, "Boolean" },
                {(int)EBasicStructureType.Int8, "SByte" },
                {(int)EBasicStructureType.UInt8, "Byte" },
                {(int)EBasicStructureType.Int16, "Int16" },
                {(int)EBasicStructureType.UInt16, "UInt16" },
                {(int)EBasicStructureType.Int32, "Int32" },
                {(int)EBasicStructureType.UInt32, "UInt32" },
                {(int)EBasicStructureType.Int64, "Int64" },
                {(int)EBasicStructureType.UInt64, "UInt64" },
                {(int)EBasicStructureType.Single, "Single" },
                {(int)EBasicStructureType.Double, "Double" },
                {(int)EBasicStructureType.String, "String" },
                {(int)EBasicStructureType.Bytes, "Buffer" },
            };
            foreach (var kv in basicNameMap)
            {
                EBasicStructureType basicType = (EBasicStructureType)kv.Key;
                string name = basicType.ToString();
                string overrideName;
                if (overrideBasicStructureNameMap != null && overrideBasicStructureNameMap.TryGetValue(kv.Key, out overrideName))
                {
                    name = overrideName;
                }
                m_basicStructureInfoMap.Add(kv.Key, new BasicStructureInfo(name, basicType, kv.Value));
            }
        }

        public void Clear()
        {
            structureMap.Clear();
            foreach (var kv in m_basicStructureInfoMap)
            {
                AddStructureInfo(kv.Value);
            }
        }

        public BasicStructureInfo GetBasicStructureInfo(EBasicStructureType structureType)
        {
            BasicStructureInfo info;
            m_basicStructureInfoMap.TryGetValue((int)structureType, out info);
            return info;
        }

        public void AddStructureInfo(StructureInfo structureInfo)
        {
            structureMap.Add(structureInfo.searchName, structureInfo);
        }

        public StructureInfo GetStructureInfo(string structureName)
        {
            StructureInfo structureInfo;
            if (structureMap.TryGetValue(structureName.ToLower(), out structureInfo))
            {
                return structureInfo;
            }
            return null;
        }

        public bool HasStructureInfo(string structureName)
        {
            StructureInfo structureInfo;
            return structureMap.TryGetValue(structureName.ToLower(), out structureInfo);
        }
    }
}
