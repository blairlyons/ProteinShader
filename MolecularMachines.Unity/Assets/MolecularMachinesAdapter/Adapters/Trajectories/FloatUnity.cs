using MolecularMachines.Framework.Model.Trajectories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.MolecularMachinesAdapter.Adapters.Trajectories
{
    class FloatUnity : TrajectorySectionUnity<Float>
    {
        public FloatUnity(TrajectoryUnity trajectoryUnity, TrajectorySection section) : base(trajectoryUnity, section)
        {
        }

        public override void Update()
        {
            Compound.Float();

            Next();
        }
    }
}
