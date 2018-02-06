using MolecularMachines.Framework.Model.Behaviors;
using MolecularMachines.Framework.Model.Markers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MolecularMachines.Import.Kinesin2
{
    [EntityBehaviorId("kinesin2.tubulinBeta")]
    public class TubulinBetaBehavior : EntityBehavior
    {
        [BindingSite("dock")]
        public BindingSite DockSite { get; set; }

        [State(InitialState = true)]
        public void Main()
        {

        }
    }
}
