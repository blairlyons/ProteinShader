using MolecularMachines.Framework.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MolecularMachines.Framework.Model.Entities;

namespace MolecularMachines.Framework.Model.Compartments
{
    /// <summary>
    /// A compartment in a <see cref="Environment"/>.
    /// Is cubiod shaped.
    /// </summary>
    public class Compartment
    {
        /// <summary>
        /// New instance.
        /// </summary>
        /// <param name="id">ID of the compartment</param>
        /// <param name="p1">One corner point of the compartment</param>
        /// <param name="p2">Second corner point of the compartment</param>
        public Compartment(string id, Vector p1, Vector p2)
        {
            this.Id = id;

            this.min = new Vector(
                Math.Min(p1.X, p2.X),
                Math.Min(p1.Y, p2.Y),
                Math.Min(p1.Z, p2.Z)
            );

            this.max = new Vector(
                Math.Max(p1.X, p2.X),
                Math.Max(p1.Y, p2.Y),
                Math.Max(p1.Z, p2.Z)
            );
        }

        private Vector min;
        private Vector max;

        /// <summary>
        /// ID of the compartment
        /// </summary>
        public string Id { get; private set; }

        /// <summary>
        /// One corner point of the compartment
        /// </summary>
        public Vector CornerMin { get { return this.min; } }

        /// <summary>
        /// Second corner point of the compartment
        /// </summary>
        public Vector CornerMax { get { return this.max; } }

        /// <summary>
        /// Checks if a point is inside this compartment
        /// </summary>
        /// <param name="p">Point to test</param>
        /// <returns>true, if p is inside, false otherwise</returns>
        public bool IsPointInside(Vector p)
        {
            return (
                p.X >= min.X && p.X <= max.X &&
                p.Y >= min.Y && p.Y <= max.Y &&
                p.Z >= min.Z && p.Z <= max.Z
            );
        }

        /// <summary>
        /// Checks if an <see cref="Entity"/> is inside this compartment.
        /// </summary>
        /// <param name="entity"><see cref="Entity"/> to test</param>
        /// <returns>true, if p is inside, false otherwise</returns>
        public bool IsEntityInside(Entity entity)
        {
            return IsPointInside(entity.LocRot.Location);
        }

        /// <summary>
        /// Get the point on the surface, that is the nearest to specific point.
        /// If the point is inside this <see cref="Compartment"/>, the same point will be returned.
        /// </summary>
        /// <param name="p">Point</param>
        /// <returns>The point on the surface that is the nearest to p. If the point is inside this <see cref="Compartment"/>, p will be returned.</returns>
        public Vector GetNearestSurfacePoint(Vector p)
        {
            return new Vector(
                PingPong(p.X, min.X, max.X),
                PingPong(p.Y, min.Y, max.Y),
                PingPong(p.Z, min.Z, max.Z)
            );
        }

        private float PingPong(float f, float min, float max)
        {
            if (f<min) { return min; }
            else if (f>max) { return max; }
            else { return f; }
        }

        /// <summary>
        /// Returns the distance of a specific point to the compartment surface. If the point is inside this <see cref="Compartment"/>, 0 is returned.
        /// </summary>
        /// <param name="p">Point</param>
        /// <returns></returns>
        public float Distance(Vector p)
        {
            if (IsPointInside(p))
            {
                return 0f;
            }
            else
            {
                var nearestPoint = GetNearestSurfacePoint(p);
                return Vector.Distance(p, nearestPoint);
            }
        }
    }
}
