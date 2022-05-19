using System;
using System.Collections;
using System.Collections.Generic;

namespace MonMoose.Core
{
	public static class ProcessUtility
    {
        public static bool InitAndStartAndCheckRunning(this ProcessBase process)
        {
            if (process != null)
            {
                process.Init();
                if (process.canStart)
                {
                    process.Start();
                    return process.isStarted;
                }
                process.Skip();
            }
            return false;
        }
    }
}
