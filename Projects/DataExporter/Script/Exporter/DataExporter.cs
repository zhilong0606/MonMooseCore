using System;
using MonMooseCore.Data;
using MonMooseCore.Structure;

namespace MonMooseCore.DataExporter
{
    public abstract class DataExporter : Exporter<DataExportContext, DataExportResult>
    {
        protected object AnalyzeValue(StructureInfo structureInfo, DataValue dataValue)
        {
            return AnalyzeValueInternal(structureInfo, dataValue);
        }

        protected object AnalyzeValueInternal(StructureInfo structureInfo, DataValue dataValue)
        {
            switch (structureInfo.structureType)
            {
                case EStructureType.Basic:
                    return AnalyzeBasicValue(structureInfo as BasicStructureInfo, dataValue as BasicDataValue);
                case EStructureType.Enum:
                    return AnalyzeEnumValue(structureInfo as EnumStructureInfo, dataValue as EnumDataValue);
                case EStructureType.Class:
                    return AnalyzeClassValue(structureInfo as ClassStructureInfo, dataValue as ClassDataValue);
                case EStructureType.List:
                    return AnalyzeListValue(structureInfo as ListStructureInfo, dataValue as ListDataValue);
                default:
                    throw new Exception();
            }
        }

        protected object AnalyzeBasicValue(BasicStructureInfo structureInfo, BasicDataValue dataValue)
        {
            string str = dataValue.value;
            switch (structureInfo.basicStructureType)
            {
                case EBasicStructureType.Bool:
                    return AnalyzeBoolValue(str);
                case EBasicStructureType.Int32:
                    return AnalyzeIntValue(str);
                case EBasicStructureType.UInt32:
                    return AnalyzeUIntValue(str);
                case EBasicStructureType.Int64:
                    return AnalyzeLongValue(str);
                case EBasicStructureType.Single:
                    return AnalyzeFloatValue(str);
                case EBasicStructureType.Double:
                    return AnalyzeDoubleValue(str);
                case EBasicStructureType.String:
                    return AnalyzeStringValue(str);
            }
            throw new Exception("");
        }

        protected virtual object AnalyzeStringValue(string str)
        {
            if (str == null)
            {
                str = string.Empty;
            }
            return str;
        }

        protected virtual bool AnalyzeBoolValue(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return false;
            }
            bool boolValue;
            if (bool.TryParse(str, out boolValue))
            {
                return boolValue;
            }
            int intValue;
            if (int.TryParse(str, out intValue))
            {
                if (intValue == 0)
                {
                    return false;
                }
                if (intValue == 1)
                {
                    return true;
                }
                return true;
            }
            throw new Exception();
        }

        protected virtual object AnalyzeLongValue(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return 0L;
            }
            long value;
            if (long.TryParse(str, out value))
            {
                return value;
            }
            throw new Exception();
        }

        protected virtual object AnalyzeIntValue(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return 0;
            }
            int value;
            if (int.TryParse(str, out value))
            {
                return value;
            }
            long longValue;
            if (long.TryParse(str, out longValue))
            {
                return (int)longValue;
            }
            float floatValue;
            if (float.TryParse(str, out floatValue))
            {
                return (int)longValue;
            }
            //return (int)0;
            throw new Exception();
        }

        protected virtual object AnalyzeUIntValue(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return 0;
            }
            uint value;
            if (uint.TryParse(str, out value))
            {
                return value;
            }
            ulong longValue;
            if (ulong.TryParse(str, out longValue))
            {
                return (uint)longValue;
            }
            float floatValue;
            if (float.TryParse(str, out floatValue))
            {
                return (uint)longValue;
            }
            //return (uint)0;
            throw new Exception();
        }

        protected virtual object AnalyzeDoubleValue(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return 0d;
            }
            double value;
            if (double.TryParse(str, out value))
            {
                return value;
            }
            throw new Exception();
        }

        protected virtual object AnalyzeFloatValue(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return 0f;
            }
            float value;
            if (float.TryParse(str, out value))
            {
                return value;
            }
            throw new Exception();
        }

        protected virtual object AnalyzeEnumValue(EnumStructureInfo structureInfo, EnumDataValue dataValue)
        {
            return dataValue.value;
        }

        protected virtual object AnalyzeClassValue(ClassStructureInfo structureInfo, ClassDataValue dataValue)
        {
            return AnalyzeDataObject(structureInfo, dataValue.value);
        }

        protected virtual object AnalyzeListValue(ListStructureInfo structureInfo, ListDataValue dataValue)
        {
            return null;
        }

        protected virtual object AnalyzeDataObject(ClassStructureInfo structureInfo, DataObject dataObj)
        {
            return null;
        }

        protected virtual object AnalyzeDataObject(int id, ClassStructureInfo structureInfo, DataObject dataObj)
        {
            return null;
        }
    }
}