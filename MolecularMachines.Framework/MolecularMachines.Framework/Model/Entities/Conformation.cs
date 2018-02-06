using MolecularMachines.Framework.DataStructures;
using MolecularMachines.Framework.Geometry;
using MolecularMachines.Framework.Model.Colliders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MolecularMachines.Framework.Model.Entities
{
    /// <summary>
    /// Conformation of an <see cref="Entity"/>.
    /// Contains the positions of <see cref="Atom"/>s and the <see cref="LocRot"/> of <see cref="Markers.Marker"/>s.
    /// </summary>
    public class Conformation
    {
        /// <summary>
        /// Create new instance
        /// </summary>
        /// <param name="id">Conformation ID</param>
        /// <param name="atomPositions">Atom Positions</param>
        /// <param name="markerLocRots">Marker <see cref="LocRot"/>s</param>
        /// <param name="collider">Collider</param>
        /// <param name="color">Color</param>
        public Conformation(string id, IEnumerable<Vector> atomPositions, IEnumerable<LocRot> markerLocRots, Collider collider, Color color)
        {
            this.Id = id;
            this.AtomPositions = new ImmutableList<Vector>(atomPositions);
            this.MarkerLocRots = new ImmutableList<LocRotStatic>(markerLocRots == null ? null : markerLocRots.Select(lr => new LocRotStatic(lr)));
            this.Collider = collider;
            this.Color = color;
        }

        /// <summary>
        /// Conformation ID
        /// </summary>
        public string Id { get; private set; }
        /// <summary>
        /// Atom Positions
        /// </summary>
        public ImmutableList<Vector> AtomPositions { get; private set; }
        /// <summary>
        /// Marker <see cref="LocRot"/>s
        /// </summary>
        public ImmutableList<LocRotStatic> MarkerLocRots { get; private set; }
        /// <summary>
        /// Collider
        /// </summary>
        public Collider Collider { get; private set; }

        /// <summary>
        /// Color
        /// </summary>
        public Color Color { get; private set; }
    }
}
