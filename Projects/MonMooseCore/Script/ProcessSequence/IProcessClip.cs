using System;

namespace MonMoose.Core
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
