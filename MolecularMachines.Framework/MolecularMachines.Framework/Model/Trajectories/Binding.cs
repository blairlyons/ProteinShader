using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MolecularMachines.Framework.Model.Compounds;
using MolecularMachines.Framework.Model.Markers;

namespace MolecularMachines.Framework.Model.Trajectories
{
    public class Binding : TrajectorySection
    {
        public Binding(BindingSite activeSite, BindingSite passiveSite)
        {
            this.ActiveSite = activeSite;
            this.PassiveSite = passiveSite;
        }

        public BindingSite ActiveSite { get; private set; }
        public BindingSite PassiveSite { get; private set; }
    }
}
