using MolecularMachines.Framework.Model.Behaviors;
using MolecularMachines.Framework.Model.Markers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MolecularMachines.Import.AtpSynthase
{
    [EntityBehaviorId("atpSynthase.pr")]
    public class PrBehavior : EntityBehavior
    {
        [BindingSite("adpSite")]
        public BindingSite AdpSite { get; set; }

        [State(InitialState = true)]
        public void Idle()
        {

        }

        public bool IsFree
        {
            get
            {
                return AdpSite.IsFree;
            }
        }
    }
}
