using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MolecularMachines.Framework.Geometry;

namespace MolecularMachines.Framework.Model
{
    /// <summary>
    /// Immutable Location and Rotation
    /// </summary>
    public class LocRotStatic : LocRot
    {
        public LocRotStatic(LocRot locRot)
            : this(locRot.Location, locRot.Rotation)
        { }

        public LocRotStatic(Vector location, Quaternion rotation)
        {
            this.location = location;
            this.rotation = rotation;
        }

        private Vector location;
        private Quaternion rotation;

        public override Vector Location
        {
            get { return location; }
            set { throw new InvalidOperationException("LocRotStatic can not be changed"); }
        }

        public override Quaternion Rotation
        {
            get { return rotation; }
            set { throw new InvalidOperationException("LocRotStatic can not be changed"); }
        }
    }
}
