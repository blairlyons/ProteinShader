using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MolecularMachines.Framework.Model.Trajectories
{
    public class Attraction : TrajectorySection
    {
        public Attraction(ISpaceInherent applyAt, ISpaceInherent destination, float untilDistanceLessThan, TimeSpan timeout)
        {
            this.ApplyAt = applyAt;
            this.Destination = destination;
            this.UntilDistanceLessThan = untilDistanceLessThan;
            this.Timeout = timeout;
        }

        public ISpaceInherent ApplyAt { get; private set; }
        public ISpaceInherent Destination { get; private set; }
        public float UntilDistanceLessThan { get; private set; }
        public TimeSpan Timeout { get; private set; }
    }
}
