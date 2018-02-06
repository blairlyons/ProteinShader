using MolecularMachines.Framework.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MolecularMachines.Framework.Model.Colliders;
using MolecularMachines.Framework.Geometry;
using MolecularMachines.Framework.Model.Markers;

namespace MolecularMachines.Framework.Model.Builders
{
    /// <summary>
    /// Helper class for creating a <see cref="Conformation"/>.
    /// </summary>
    public class ConformationBuilder
    {
        /// <summary>
        /// Create a new instance with a predefined ID and color.
        /// </summary>
        /// <param name="id"><see cref="Conformation.Id"/></param>
        /// <param name="color"><see cref="Conformation.Color"/></param>
        public ConformationBuilder(string id, Color color)
        {
            this.Id = id;
            this.Atoms = new AtomsBuilder();
            this.Markers = new LocRotListBuilder();
            this.Color = color;
        }

        /// <summary>
        /// <see cref="Conformation.Id"/>
        /// </summary>
        public string Id { get; private set; }

        /// <summary>
        /// Atoms and their position in the <see cref="Conformation"/>
        /// </summary>
        public AtomsBuilder Atoms { get; private set; }

        /// <summary>
        /// <see cref="Marker"/>s and their <see cref="LocRot"/> in this <see cref="Conformation"/>
        /// </summary>
        public LocRotListBuilder Markers { get; private set; }
        
        /// <summary>
        /// <see cref="Conformation.Collider"/>. If <c>null</c>, the collider will be automatically created by using an approximate <see cref="SphereColliderGeometry"/>.
        /// </summary>
        public Collider Collider { get; set; }

        /// <summary>
        /// <see cref="Conformation.Color"/>
        /// </summary>
        public Color Color { get; set; }

        private Collider GenerateCollider()
        {
            var atoms = this.Atoms.GetAtoms().ToArray();
            var positions = this.Atoms.GetPositions().ToArray();

            var positionSum = Vector.Zero;

            if (atoms.Length > 0)
            {
                var center = positions.Aggregate((a, b) => a + b) / atoms.Length;
                float radius = 0;

                for (int i = 0; i < atoms.Length; i++)
                {
                    var atom = atoms[i];
                    var position = positions[i];

                    // TODO consider atom size
                    var distanceFromCenter = Vector.Distance(center, position);
                    if (radius < distanceFromCenter)
                    {
                        radius = distanceFromCenter;
                    }
                }

                return new Collider(new SphereColliderGeometry(center, radius));
            }
            else
            {
                return Collider.Empty;
            }
        }

        /// <summary>
        /// Copy properties from another <see cref="ConformationBuilder"/>.
        /// </summary>
        /// <param name="other"><see cref="ConformationBuilder"/> to copy from</param>
        public void CopyFrom(ConformationBuilder other)
        {
            this.Atoms.CopyFrom(other.Atoms);
            this.Markers.CopyFrom(other.Markers);
            this.Collider = other.Collider;
            this.Color = other.Color;
        }

        /// <summary>
        /// Create a <see cref="Conformation"/> from the specified properties.
        /// </summary>
        /// <returns></returns>
        public Conformation CreateInstance()
        {
            return new Conformation(
                Id,
                this.Atoms.GetPositions(),
                this.Markers.GetLocRots(),
                this.Collider != null ? this.Collider : GenerateCollider(),
                this.Color
            );
        }

        /// <summary>
        /// Move atoms and markers by setting a new origin.
        /// </summary>
        /// <param name="origin">new origin</param>
        public void SetOrigin(Vector origin)
        {
            this.Atoms.SetOrigin(origin);
            this.Markers.SetOrigin(origin);
        }
    }
}
