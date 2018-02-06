using MolecularMachines.Framework.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MolecularMachines.Framework.Model.SpatialLinks
{
    /// <summary>
    /// Dummy class, that does not really do anything, but provide placeholder instances.
    /// </summary>
    public class SpatialLinkDummy : SpatialLink
    {
        public SpatialLinkDummy(SpatialLinkType type, LocRot fixedLocRot, bool collidersActive)
        {
            this.locRot = new LocRotStatic(fixedLocRot);
            this.type = type;
            this.collidersActive = collidersActive;
        }

        private LocRot locRot;
        public override LocRot LocRot { get { return this.locRot; } }

        private SpatialLinkType type;
        public override SpatialLinkType Type { get { return this.type; } }

        private bool collidersActive;
        public override bool CollidersActive { get { return this.collidersActive; } }

        public override bool IsAlive
        {
            get
            {
                return true;
            }
        }
    }
}
