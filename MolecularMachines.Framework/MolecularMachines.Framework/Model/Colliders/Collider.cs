using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MolecularMachines.Framework.Model.Colliders
{
    /// <summary>
    /// Defines a collider for an <see cref="Entities.Entity"/>.
    /// A collider is built out of multiple <see cref="ColliderGeometry"/> primitives.
    /// </summary>
    public class Collider : IEnumerable<ColliderGeometry>
    {
        /// <summary>
        /// Create a new instance.
        /// </summary>
        /// <param name="colliderGeometries">primitives that make up the collider</param>
        public Collider(params ColliderGeometry[] colliderGeometries)
        {
            this.colliderGeometries = colliderGeometries;
        }

        private ColliderGeometry[] colliderGeometries;

        /// <summary>
        /// Get primitives that make up the collider.
        /// </summary>
        /// <returns>Collection of collider geometries.</returns>
        public IEnumerator<ColliderGeometry> GetEnumerator()
        {
            return ((IEnumerable<ColliderGeometry>)this.colliderGeometries).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        /// <summary>
        /// Empty <see cref="Collider"/> without <see cref="ColliderGeometry"/>.
        /// </summary>
        public static Collider Empty
        {
            get
            {
                return new Collider();
            }
        }
    }
}
