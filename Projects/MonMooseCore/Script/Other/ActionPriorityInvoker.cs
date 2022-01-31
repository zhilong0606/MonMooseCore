using System;

namespace MonMooseCore
{
    public class ActionPriorityInvoker : PriorityInvoker<Action>
    {
        public void Invoke()
        {
            Invoke(OnActionInvoke);
        }

        private void OnActionInvoke(Action action)
        {
            if (action != null)
            {
                action();
            }
        }
    }
}