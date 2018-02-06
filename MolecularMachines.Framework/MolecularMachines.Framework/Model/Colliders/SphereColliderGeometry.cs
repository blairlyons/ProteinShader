using MolecularMachines.Framework.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MolecularMachines.Framework.Model.Colliders
{
    /// <summary>
    /// A <see cref="Collider"/> primitive in the shape of a sphere.
    /// </summary>
    public class SphereColliderGeometry : ColliderGeometry
    {
        /// <summary>
        /// New instance
        /// </summary>
        /// <param name="center">Center of the sphere, relative to the entity origin</param>
        /// <param name="radius">Radius of the sphere</param>
        public SphereColliderGeometry(Vector center, float radius)
        {
            this.Center = center;
            this.Radius = radius;
        }

        /// <summary>
        /// Center of the sphere, relative to the entity origin
        /// </summary>
        public Vector Center { get; private set; }

        /// <summary>
        /// Radius of the sphere
        /// </summary>
        public float Radius { get; private set; }
    }
}
