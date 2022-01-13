﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Data;
using Google.Protobuf;
using Structure;

public abstract class DataExporter : Exporter
{
    protected object AnalyzeValue(BaseStructureInfo structureInfo, DataValue dataValue)
    {
        return AnalyzeValueInternal(structureInfo, dataValue);
    }

    protected object AnalyzeValueInternal(BaseStructureInfo structureInfo, DataValue dataValue)
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
            case EBasicStructureType.Int64:
                return AnalyzeLongValue(str);
            case EBasicStructureType.Single:
                return AnalyzeFloatValue(str);
            case EBasicStructureType.Double:
                return AnalyzeDoubleValue(str);
            case EBasicStructureType.String:
                return AnalyzeStringValue(str);
        }
        Debug.LogError("");
        return null;
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
        bool value;
        if (bool.TryParse(str, out value))
        {
            return value;
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