using MolecularMachines.Framework.DataStructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MolecularMachines.Framework.Model.Entities
{
    /// <summary>
    /// Holds the definition of a <see cref="EntityStructure"/>.
    /// Can be used to create <see cref="EntityStructure"/> instances (<see cref="EntityStructureClass.CreateInstance"/>).
    /// </summary>
    public class EntityStructureClass
    {
        /// <summary>
        /// New instance.
        /// </summary>
        /// <param name="id">ID</param>
        /// <param name="atoms">atoms</param>
        /// <param name="conformations">conformations</param>
        public EntityStructureClass(string id, IEnumerable<Atom> atoms, IEnumerable<Conformation> conformations)
        {
            this.Id = id;

            this.Atoms = new ImmutableList<Atom>(atoms);
            this.Conformations = new ImmutableList<Conformation>(conformations);

            if (this.Conformations.Count == 0) { throw new ArgumentException("minimum 1 conformation required"); }
            foreach (var conformation in Conformations)
            {
                if (conformation.AtomPositions.Count != this.Atoms.Count)
                { throw new ArgumentException($"AtomPositions count in conformation \"{conformation.Id}\" does not match Atoms count"); }
            }

            InitialConformation = this.Conformations[0];

            this.Mass = this.Atoms.Sum(atom => atom.Element.Mass);
        }

        /// <summary>
        /// Structure ID
        /// </summary>
        public string Id { get; private set; }

        /// <summary>
        /// Atoms
        /// </summary>
        public ImmutableList<Atom> Atoms { get; private set; }
        /// <summary>
        /// Conformations
        /// </summary>
        public ImmutableList<Conformation> Conformations { get; private set; }
        /// <summary>
        /// Initial Conformation
        /// </summary>
        public Conformation InitialConformation { get; private set; }

        /// <summary>
        /// Mass of the structure, determined by summing up <see cref="Element.Mass"/> (not physically correct) (Unit: u - unified atomic mass unit)
        /// </summary>
        public float Mass { get; private set; }

        private int counter = 0;

        /// <summary>
        /// Indicates that the same structure instance is shared between multiple entities.
        /// (To save memory).
        /// Structure sharing is enabled automatically if the <see cref="EntityStructureClass"/> only has one <see cref="Conformation"/> defined.
        /// </summary>
        public bool StructureSharing
        {
            get
            {
                return (Conformations.Count == 1);
            }
        }

        private EntityStructure sharedStructure;

        private EntityStructure CreateInstanceIntern()
        {
            counter++;
            return new EntityStructure(Id + counter.ToString(), this);
        }

        /// <summary>
        /// Create a <see cref="EntityStructure"/> instance.
        /// </summary>
        /// <returns></returns>
        public EntityStructure CreateInstance()
        {
            if (this.StructureSharing)
            {
                if (this.sharedStructure == null)
                {
                    // create if not yet created
                    this.sharedStructure = CreateInstanceIntern();
                }

                return this.sharedStructure;
            }
            else
            {
                // create new instace for every call
                return CreateInstanceIntern();
            }
        }
    }
}
