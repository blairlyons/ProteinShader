using MolecularMachines.Framework.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MolecularMachines.Framework.Model.Builders
{
    /// <summary>
    /// Helper class to associate a specific <see cref="Atom"/> with a position (<see cref="Vector"/>).
    /// </summary>
    public class AtomsBuilder
    {
        private List<Atom> atoms = new List<Atom>();
        private List<Vector> positions = new List<Vector>();

        /// <summary>
        /// Adds a new atom with a element symbol and a position
        /// </summary>
        /// <param name="symbol">Element-Symbol</param>
        /// <param name="position">Atom position</param>
        public void AddAtom(string symbol, Vector position)
        {
            AddAtom(Element.BySymbol(symbol), position);
        }

        /// <summary>
        /// Adds a new atom with a element and a position
        /// </summary>
        /// <param name="element">Atom-Element</param>
        /// <param name="position">Atom position</param>
        public void AddAtom(Element element, Vector position)
        {
            atoms.Add(new Atom(element));
            positions.Add(position);
        }

        /// <summary>
        /// Adds multiple atoms from a <see cref="IEnumerable{T}"/>.
        /// </summary>
        /// <typeparam name="TItem">Type of the collection</typeparam>
        /// <param name="collection">collection</param>
        /// <param name="symbolFunc">Function for getting an atom element from a collection item</param>
        /// <param name="positionFunc">Function for getting an atom position from a colleciton item</param>
        public void Add<TItem>(IEnumerable<TItem> collection, Func<TItem, string> symbolFunc, Func<TItem, Vector> positionFunc)
        {
            foreach (var item in collection)
            {
                AddAtom(
                    symbolFunc(item),
                    positionFunc(item)
                );
            }
        }

        /// <summary>
        /// Removes an Atom at a specific index
        /// </summary>
        /// <param name="index"></param>
        public void RemoveAt(int index)
        {
            this.atoms.RemoveAt(index);
            this.positions.RemoveAt(index);
        }

        /// <summary>
        /// Calculates and returns the center of all atom positions.
        /// </summary>
        /// <returns>Center</returns>
        public Vector CalcCenter()
        {
            var center = Vector.Zero;
            foreach (var p in positions)
            {
                center += p;
            }
            center = center / positions.Count;

            return center;
        }

        /// <summary>
        /// Changes the position of all atoms by a manipulation function
        /// </summary>
        /// <param name="manipulationFunc">Manipulation function. The parameter is the old position, the returned value is used as new position</param>
        private void ManipulatePositions(Func<Vector, Vector> manipulationFunc)
        {
            for (int i = 0; i < positions.Count; i++)
            {
                positions[i] = manipulationFunc(positions[i]);
            }
        }

        /// <summary>
        /// Moves all atom position by a specified amount
        /// </summary>
        /// <param name="v">delta vector</param>
        public void Move(Vector v)
        {
            ManipulatePositions((old) => old + v);
        }

        /// <summary>
        /// Rotates all atom positions by a specified quaternion
        /// </summary>
        /// <param name="q">rotation quaternion</param>
        public void Rotate(Quaternion q)
        {
            ManipulatePositions((old) => q.Rotate(old));
        }

        /// <summary>
        /// Returns all positions that were added
        /// </summary>
        /// <returns>positions</returns>
        public IEnumerable<Vector> GetPositions() { return positions; }

        /// <summary>
        /// Returns all atoms that were added
        /// </summary>
        /// <returns>atoms</returns>
        public IEnumerable<Atom> GetAtoms() { return atoms; }

        public void CopyFrom(AtomsBuilder other)
        {
            this.atoms.Clear();
            this.atoms.AddRange(other.atoms);

            this.positions.Clear();
            this.positions.AddRange(other.positions);
        }

        /// <summary>
        /// Number of added atoms
        /// </summary>
        public int Count
        {
            get
            {
                return this.atoms.Count; // which == this.positions.Count
            }
        }

        /// <summary>
        /// Move atoms by setting a new origin
        /// </summary>
        /// <param name="origin">new origin</param>
        public void SetOrigin(Vector origin)
        {
            Move(Vector.Zero - origin);
        }
    }
}
