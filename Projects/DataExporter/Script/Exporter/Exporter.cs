using System;

namespace MonMooseCore.DataExporter
{
    public abstract class Exporter<TContext, TResult> : Exporter
        where TContext : ExportContext
        where TResult : ExportResult, new()
    {
        protected TContext m_context;
        protected TResult m_result = new TResult();

        public TResult Export(TContext context)
        {
            m_context = context;
            OnExport();
            return m_result;
        }
    }

    public abstract class Exporter
    {
        public Action<double, string> actionOnProcessMsgSend;

        protected abstract void OnExport();

        protected void SendMsg(double processValue, string content)
        {
            if (actionOnProcessMsgSend != null)
            {
                actionOnProcessMsgSend(processValue, content);
            }
        }
    }
}
