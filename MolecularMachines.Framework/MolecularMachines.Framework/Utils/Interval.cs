using MolecularMachines.Framework.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MolecularMachines.Framework.Utils
{
    public class Interval : IFrameUpdate
    {
        public Interval(TimeSpan interval, Action action)
        {
            this.nextAction = DateTime.Now.Add(interval);
            this.interval = interval;
            this.action = action;
        }

        private DateTime nextAction;
        private TimeSpan interval;
        private Action action;

        /// <summary>
        /// Must be called every frame.
        /// </summary>
        public void Update()
        {
            var now = DateTime.Now;
            if (now >= this.nextAction)
            {
                this.nextAction = now.Add(interval);

                action();
            }
        }
    }
}
