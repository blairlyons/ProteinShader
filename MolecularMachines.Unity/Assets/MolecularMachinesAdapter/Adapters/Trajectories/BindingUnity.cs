using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MolecularMachines.Framework.Model.Trajectories;
using Assets.MolecularMachinesAdapter.Adapters.Trajectories;

namespace Assets.MolecularMachinesAdapter.Adapters.Trajectories
{
    class BindingUnity : TrajectorySectionUnity<Binding>
    {
        public BindingUnity(TrajectoryUnity trajectoryUnity, TrajectorySection section) : base(trajectoryUnity, section)
        { }

        public override void Update()
        {
            if (info.ActiveSite.Owner.IsDisposed || info.PassiveSite.Owner.IsDisposed)
            {
                Next();
            }
            else
            {
                info.ActiveSite.InstantBind(info.PassiveSite);

                Next();
            }
        }
    }
}
