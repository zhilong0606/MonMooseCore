using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MonMooseCore.Structure
{
    public static class StructureFactory
    {
        public static StructureInfo CreateSimpleStructureInfo(EStructureType structureType, string name)
        {
            return CreateStructureInfo(structureType, name, null, null, null);
        }

        public static ListStructureInfo CreateListStructureInfo(StructureInfo valueInfo)
        {
            return CreateStructureInfo(default(EStructureType), string.Empty, valueInfo, null, null) as ListStructureInfo;
        }

        public static DictionaryStructureInfo CreateDictionaryStructureInfo(StructureInfo keyInfo, StructureInfo valueInfo)
        {
            return CreateStructureInfo(default(EStructureType), string.Empty, valueInfo, keyInfo, null) as DictionaryStructureInfo;
        }

        public static PackStructureInfo CreatePackStructureInfo(ClassStructureInfo itemInfo)
        {
            return CreateStructureInfo(default(EStructureType), string.Empty, null, null, itemInfo) as PackStructureInfo;
        }

        public static StructureInfo CreateStructureInfo(EStructureType structureType, string name, StructureInfo valueStructureInfo, StructureInfo keyStructureInfo, ClassStructureInfo itemStructureInfo)
        {
            StructureInfo structureInfo = null;
            switch (structureType)
            {
                case EStructureType.Enum:
                    structureInfo = new EnumStructureInfo(name);
                    break;
                case EStructureType.Class:
                    structureInfo = new ClassStructureInfo(name);
                    break;
                case EStructureType.List:
                    structureInfo = new ListStructureInfo(valueStructureInfo);
                    break;
                case EStructureType.Dictionary:
                    structureInfo = new DictionaryStructureInfo(keyStructureInfo, valueStructureInfo);
                    break;
                case EStructureType.Pack:
                    structureInfo = new PackStructureInfo(itemStructureInfo);
                    break;
                default:
                    throw new Exception(string.Format("此结构类型{0}不能创建", structureType));
            }
            if (!structureInfo.isValid)
            {
                throw new Exception(string.Format("{0}创建类型时出错", structureInfo.name));
            }
            return structureInfo;
        }
    }
}
