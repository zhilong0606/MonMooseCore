using System;

namespace MonMooseCore
{
    public interface IProcessClip
    {
        void Init();
        void UnInit();
        bool CanStart { get; }
        Action<IProcessClip> OnEnd { set; }
        void OnSkipClip();
        void OnStartClip();
        void OnResetClip();
    }
}
