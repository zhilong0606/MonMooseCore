using System;
using System.Collections;
using System.Collections.Generic;

namespace MonMoose.Core
{
    public enum ProcessStateId
    {
        None,
        Inited,//None
        Started,//Pause,Init
        Paused,//Start
        Ended,//Init,Start
        UnInited,//Init,Start,Pause,End
    }
}
