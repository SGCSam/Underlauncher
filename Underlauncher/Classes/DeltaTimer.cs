using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Underlauncher
{
    public class DeltaTimer
    {
        private Stopwatch watch;
        private double _TimeWaited = 0.0;
        private Stopwatch waitWatch = new Stopwatch();

        public DeltaTimer()
        {
            watch = new Stopwatch();
        }

        public bool Wait(double timeToWait)
        {
            if (!waitWatch.IsRunning)
            {
                waitWatch.Start();
            }

            if (timeToWait <= _TimeWaited)
            {
                _TimeWaited = 0;
                waitWatch.Stop();
                return true;
            }

            else
            {
                double deltaTime = waitWatch.ElapsedMilliseconds;
                _TimeWaited += deltaTime;
                waitWatch.Restart();
                return false;
            }
        }
    }
}
