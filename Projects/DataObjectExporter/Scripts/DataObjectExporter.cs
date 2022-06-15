using System;
using MonMoose.Core.Data;
using MonMoose.Core.Structure;

namespace MonMoose.Core.DataExporter
{
    public abstract class DataObjectExporter
    {
        protected DataObjectExportContext m_context;

        private Action<double, string> m_actionOnProcessMsgSend;

        public void Export(DataObjectExportContext context)
        {
            m_context = context;
            OnExport();
        }

        public void RegisterMsgReceiver(Action<double, string> actionOnReceiveMsg)
        {
            m_actionOnProcessMsgSend += actionOnReceiveMsg;
        }

        public void UnRegisterMsgReceiver(Action<double, string> actionOnReceiveMsg)
        {
            m_actionOnProcessMsgSend -= actionOnReceiveMsg;
        }
        protected abstract void OnExport();

        protected void SendMsg(double processValue, string content)
        {
            if (m_actionOnProcessMsgSend != null)
            {
                m_actionOnProcessMsgSend(processValue, content);
            }
        }


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
            switch (structureInfo.basicStructureType)
            {
                case EBasicStructureType.Bool:
                    return (dataValue as BoolDataValue).value;
                case EBasicStructureType.Int8:
                    return (dataValue as Int8DataValue).value;
                case EBasicStructureType.UInt8:
                    return (dataValue as UInt8DataValue).value;
                case EBasicStructureType.Int16:
                    return (dataValue as Int16DataValue).value;
                case EBasicStructureType.UInt16:
                    return (dataValue as UInt16DataValue).value;
                case EBasicStructureType.Int32:
                    return (dataValue as Int32DataValue).value;
                case EBasicStructureType.UInt32:
                    return (dataValue as UInt32DataValue).value;
                case EBasicStructureType.Int64:
                    return (dataValue as Int64DataValue).value;
                case EBasicStructureType.UInt64:
                    return (dataValue as UInt64DataValue).value;
                case EBasicStructureType.Single:
                    return (dataValue as SingleDataValue).value;
                case EBasicStructureType.Double:
                    return (dataValue as DoubleDataValue).value;
                case EBasicStructureType.String:
                    return (dataValue as StringDataValue).value;
            }
            throw new Exception("");
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