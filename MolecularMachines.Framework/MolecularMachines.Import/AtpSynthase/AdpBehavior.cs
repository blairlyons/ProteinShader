using MolecularMachines.Framework.Model.Behaviors;
using MolecularMachines.Framework.Model.Markers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MolecularMachines.Import.AtpSynthase
{
    [EntityBehaviorId("atpSynthase.adp")]
    public class AdpBehavior : EntityBehavior
    {
        [BindingSite("prSite")]
        public BindingSite PrSite { get; set; }

        [BindingSite("mainSite")]
        public BindingSite MainSite { get; set; }

        [State(InitialState = true)]
        public void Main()
        {

        }

        public bool IsAdp
        {
            get
            {
                return PrSite.IsFree;
            }
        }

        public bool IsAtp
        {
            get
            {
                return PrSite.IsBound;
            }
        }

        public bool IsFree
        {
            get
            {
                return MainSite.IsFree;
            }
        }
    }
}
