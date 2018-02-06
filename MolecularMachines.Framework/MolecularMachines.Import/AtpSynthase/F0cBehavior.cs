using MolecularMachines.Framework.Model.Behaviors;
using MolecularMachines.Framework.Model.Markers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MolecularMachines.Import.AtpSynthase
{
    [EntityBehaviorId("atpSynthase.f0c")]
    public class F0cBehavior : EntityBehavior
    {
        [BindingSite("protonSite")]
        public BindingSite ProtonSite { get; set; }

        [State(InitialState = true, Conformation = "tensed")]
        public void Tense()
        {
            // Change to Relax-State as soon a proton is bound
            if (ProtonSite.IsBound)
            {
                SetState(Relax);
            }
        }

        [State(Conformation = "relaxed")]
        public void Relax()
        {
            // Change to Tense-State as soon the proton leaves
            if (!ProtonSite.IsBound)
            {
                SetState(Tense);
            }
        }
    }
}
