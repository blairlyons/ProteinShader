using MolecularMachines.Framework.Model;
using MolecularMachines.Framework.Model.SpatialLinks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.MolecularMachinesAdapter.Adapters.SpatialLinks
{
    public class FixedUnity : SpatialLink
    {
        public FixedUnity(CompoundUnity compoundUnity, LocRot fixedLocRot)
        {
            if (MMM.Instance.AllowEditFixed)
            {
                this.locRot = new GameObjectLocRot(compoundUnity.GameObject, fixedLocRot);
            }
            else
            {
                this.locRot = new LocRotStatic(fixedLocRot);
            }
        }

        private LocRot locRot;
        public override LocRot LocRot
        {
            get
            {
                return this.locRot;
            }
        }

        public override SpatialLinkType Type { get { return SpatialLinkType.Fixed; } }
        public override bool CollidersActive { get { return true; } }

        public override bool IsAlive { get { return true; } }
    }
}
