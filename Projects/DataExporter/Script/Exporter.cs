using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Structure;

namespace MonMooseCore.DataExporter
{
    public abstract class Exporter<T> : Exporter
    {
        protected T m_context;

        public void Export(T context)
        {
            m_context = context;
            OnExport();
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
