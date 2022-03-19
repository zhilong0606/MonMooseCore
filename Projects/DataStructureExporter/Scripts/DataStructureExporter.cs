using System;
using MonMoose.Core.Structure;

namespace MonMoose.Core.DataExporter
{
    public abstract class DataStructureExporter
    {
        protected DataStructureExportContext m_context;

        private Action<double, string> m_actionOnProcessMsgSend;

        public void Export(DataStructureExportContext context)
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
        protected abstract string GetExportName(StructureInfo structureInfo);
        protected abstract string GetExportName(EBasicStructureType type);

        protected void SendMsg(double processValue, string content)
        {
            if (m_actionOnProcessMsgSend != null)
            {
                m_actionOnProcessMsgSend(processValue, content);
            }
        }
    }
}