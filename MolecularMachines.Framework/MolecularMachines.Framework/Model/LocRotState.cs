using MolecularMachines.Framework.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MolecularMachines.Framework.Model
{
    /// <summary>
    /// Location and Rotation simply stored in memory
    /// </summary>
    public class LocRotState : LocRot
    {
        public LocRotState() : this(Vector.Zero, Quaternion.Identity)
        { }

        public LocRotState(LocRot initialValue) : this(initialValue.Location, initialValue.Rotation)
        { }

        public LocRotState(Vector initialLocation, Quaternion initialRotation)
        {
            this.Location = initialLocation;
            this.Rotation = initialRotation;
        }
        
        public override Vector Location { get; set; }
        public override Quaternion Rotation { get; set; }
    }
}
