using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MolecularMachines.Framework.Utils
{
    public class Timer
    {
        public Timer(TimeSpan duration)
        {
            this.duration = (float)duration.TotalSeconds;
            Reset();
        }

        private float duration;
        private DateTime started;

        public void Reset()
        {
            this.started = DateTime.Now;
        }

        public float Time
        {
            get
            {
                return (float)(DateTime.Now - started).TotalSeconds;
            }
        }

        public bool IsDone()
        {
            float dummy;
            return IsDone(out dummy);
        }

        public bool IsDone(out float progress)
        {
            progress = this.Time / this.duration;
            if (progress >= 1f)
            {
                progress = 1f;
                return true;
            }
            else
            { return false; }
        }
    }
}
