using System;
using System.Collections.Generic;
using MonMooseCore.Structure;

namespace MonMooseCore.Data
{
    public class DataObjectManager
    {
        private Dictionary<ClassStructureInfo, Dictionary<int, DataObject>> m_structureMap = new Dictionary<ClassStructureInfo, Dictionary<int, DataObject>>();

        public Dictionary<ClassStructureInfo, Dictionary<int, DataObject>> structureMap
        {
            get { return m_structureMap; }
        }

        public Dictionary<int, DataObject> GetDataMap(ClassStructureInfo structureInfo)
        {
            Dictionary<int, DataObject> map;
            m_structureMap.TryGetValue(structureInfo, out map);
            return map;
        }

        public void AddDataObject(ClassStructureInfo structureInfo, int id, DataObject obj)
        {
            Dictionary<int, DataObject> map;
            if (!m_structureMap.TryGetValue(structureInfo, out map))
            {
                map = new Dictionary<int, DataObject>();
                m_structureMap.Add(structureInfo, map);
            }
            if (map.ContainsKey(id))
            {
                throw new DataObjectIdException(DataObjectIdException.EErrorId.SameDataObjectId, structureInfo.name, id);
            }
            map.Add(id, obj);
        }

        public void Clear()
        {
            m_structureMap.Clear();
        }
    }
}
