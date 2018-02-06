using MolecularMachines.Framework.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MolecularMachines.Framework.Model
{
    /// <summary>
    /// Provides a location together with a rotation.
    /// </summary>
    public abstract class LocRot : ISpaceInherent
    {
        public abstract Vector Location { get; set; }
        public abstract Quaternion Rotation { get; set; }

        public static LocRotStatic Combine(LocRot root, LocRot relative)
        {
            return new LocRotStatic(
                root.Location + root.Rotation.Rotate(relative.Location),
                root.Rotation * relative.Rotation
            );
        }

        public void SetLerp(LocRot from, LocRot to, float p)
        {
            this.Location = Vector.Lerp(from.Location, to.Location, p);
            this.Rotation = Quaternion.Lerp(from.Rotation, to.Rotation, p);
        }

        public static bool Compare(LocRot a, LocRot b, float epsilon)
        {
            return
                Vector.Compare(a.Location, b.Location, epsilon) &&
                Quaternion.Compare(a.Rotation, b.Rotation, epsilon);
        }

        LocRot ISpaceInherent.LocRot
        {
            get
            {
                return this;
            }
        }

        bool ISpaceInherent.IsAlive
        {
            get
            {
                return true;
            }
        }

        public static readonly LocRot Zero = new LocRotStatic(Vector.Zero, Quaternion.Identity);

        public void Set(LocRot locRot)
        {
            this.Location = locRot.Location;
            this.Rotation = locRot.Rotation;
        }

        public override string ToString()
        {
            return "loc: " + this.Location + "; rot: " + this.Rotation;
        }
    }
}
