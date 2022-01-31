using System;

namespace MonMooseCore
{
    public abstract class AbstractProcessClip : IProcessClip
    {
        public virtual void Init()
        {
        }

        public virtual void UnInit()
        {
        }

        public virtual bool CanStart
        {
            get { return false; }
        }

        public Action<IProcessClip> OnEnd { get; set; }

        public void End()
        {
            if (OnEnd != null)
            {
                OnEnd(this);
            }
        }

        public virtual void OnStartClip()
        {
        }

        public virtual void OnSkipClip()
        {
        }

        public virtual void OnResetClip()
        {
        }
    }
}