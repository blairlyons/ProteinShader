using MolecularMachines.Framework.Model;
using MolecularMachines.Framework.Model.Trajectories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.MolecularMachinesAdapter.Adapters.Trajectories
{
    class MovementUnity : TrajectorySectionUnity<Movement>
    {
        public MovementUnity(TrajectoryUnity trajectoryUnity, TrajectorySection section) : base(trajectoryUnity, section)
        {
        }

        private LocRotStatic startLocRot;

        public override void Start()
        { 
            this.startLocRot = new LocRotStatic(this.Compound.SpatialLink.LocRot);
            this.TrajectoryUnity.SetCollidersActive(info.CollidersActive);
        }

        public override void Update()
        {
            if (info.Destination.IsAlive)
            {
                var destination = info.Destination.LocRot;

                var t = this.CurrentSectionTime;
                var p = (float)(t.TotalSeconds / info.Duration.TotalSeconds);

                this.LocRot.SetLerp(this.startLocRot, destination, p);

                if (t >= info.Duration)
                {
                    Next();
                }
            }
            else
            {
                Next();
            }
        }
    }
}
