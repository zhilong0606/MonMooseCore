using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Structure;

public abstract class Exporter
{
    protected UserContext m_context;
    public Action<double, string> actionOnProcessMsgSend;

    public void Export(UserContext context)
    {
        m_context = context;
        OnExport();
    }

    protected abstract void OnExport();

    protected void SendMsg(double processValue, string content)
    {
        if (actionOnProcessMsgSend != null)
        {
            actionOnProcessMsgSend(processValue, content);
        }
    }
}
