using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MolecularMachines.Framework.Model.Trajectories
{
    public class Movement : TrajectorySection
    {
        public Movement(ISpaceInherent destination, TimeSpan duration, bool collidersActive)
        {
            this.Destination = destination;
            this.Duration = duration;
            this.CollidersActive = collidersActive;
        }

        public ISpaceInherent Destination { get; private set; }
        public TimeSpan Duration { get; private set; }
        public bool CollidersActive { get; private set; }
    }
}
