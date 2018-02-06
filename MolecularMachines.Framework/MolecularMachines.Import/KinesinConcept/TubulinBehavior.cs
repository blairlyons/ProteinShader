using MolecularMachines.Framework.Model.Behaviors;
using MolecularMachines.Framework.Model.Markers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MolecularMachines.Import.KinesinConcept
{
    [EntityBehaviorId("kinesin.tubulin")]
    public class TubulinBehavior : EntityBehavior
    {
        [BindingSite("kinesinSite")]
        public BindingSite KinesinSite { get; set; }

        [State(InitialState = true)]
        public void Main()
        {

        }
    }
}
