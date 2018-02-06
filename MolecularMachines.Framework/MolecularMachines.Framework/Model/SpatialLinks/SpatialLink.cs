using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MolecularMachines.Framework.Model.SpatialLinks
{
    /// <summary>
    /// Links a <see cref="Compounds.Compound"/> to a spatial location.
    /// </summary>
    public abstract class SpatialLink : ISpaceInherent
    {
        public abstract SpatialLinkType Type { get; }
        /// <summary>
        /// Returns the current location and rotation
        /// </summary>
        public abstract LocRot LocRot { get; }
        /// <summary>
        /// Returns true, if the <see cref="Colliders.Collider"/> are activated for this instance.
        /// </summary>
        public abstract bool CollidersActive { get; }
        /// <summary>
        /// Returns true, if the owner is still alive, and not disposed.
        /// </summary>
        public abstract bool IsAlive { get; }

        public void Update()
        {
            OnUpdate();
        }

        protected virtual void OnUpdate()
        {

        }

        public override string ToString()
        {
            return Type.ToString() + " " + LocRot.ToString();
        }
    }
}
