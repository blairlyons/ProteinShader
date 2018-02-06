using MolecularMachines.Framework.Geometry;
using MolecularMachines.Framework.Model.Behaviors.Presents;
using MolecularMachines.Framework.Model.Entities;
using MolecularMachines.Framework.Model.Markers;
using MolecularMachines.Framework.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MolecularMachines.Framework.Model.Builders
{
    /// <summary>
    /// Helper class for creating a <see cref="EntityClass"/>.
    /// </summary>
    public class EntityClassBuilder
    {
        /// <summary>
        /// Create a new instance with predefined <see cref="EntityClass.Id"/>.
        /// </summary>
        /// <param name="id"><see cref="EntityClass.Id"/></param>
        public EntityClassBuilder(string id)
        {
            this.id = id;
            this.DefaultColor = Color.Random;
            this.BehaviorId = "present.empty";
        }

        private string id;
        /// <summary>
        /// <see cref="EntityClass.Id"/>
        /// </summary>
        public string Id { get { return this.id; } }

        /// <summary>
        /// Set the current conformation to work on by its id.
        /// If the ID does not exist yet, a new conformation will be created.
        /// </summary>
        /// <param name="conformationId"><see cref="Conformation.Id"/></param>
        public void SetConformation(string conformationId)
        {
            var foundConformation = FindConformation(conformationId);
            if (foundConformation == null)
            {
                foundConformation = new ConformationBuilder(conformationId, this.DefaultColor);
                conformations.Add(foundConformation);
            }

            this.currentConformation = foundConformation;
        }

        private ConformationBuilder FindConformation(string conformationId)
        {
            return (from c in this.conformations where c.Id == conformationId select c).FirstOrDefault();
        }

        /// <summary>
        /// Creates an intermediate conformation by linearly interpolating color, atom position and marker position and rotation of two existing conformations.
        /// </summary>
        /// <param name="index">Index at which the new conformation should be added</param>
        /// <param name="newConformationId"><see cref="Conformation.Id"/> of the new conformation</param>
        /// <param name="startConformationId">ID of start conformation (progress=0)</param>
        /// <param name="endConformationId">ID of end conformation (progress=1)</param>
        /// <param name="progress">Interpolation progress</param>
        public void InterpolateConformation(int index, string newConformationId, string startConformationId, string endConformationId, float progress)
        {
            var foundConformation = FindConformation(newConformationId);
            if (foundConformation != null)
            { throw new Exception("conformation with id \"" + newConformationId + "\" already exists"); }

            var startConformation = FindConformation(startConformationId);
            if (startConformation == null)
            { throw new Exception("startConformaitonId does not exist"); }

            var endConformation = FindConformation(endConformationId);
            if (startConformation == null)
            { throw new Exception("startConformaitonId does not exist"); }

            var conformation = new ConformationBuilder(newConformationId, Color.Lerp(startConformation.Color, endConformation.Color, progress));
            this.conformations.Insert(index, conformation);

            // add and interpolate atoms
            if (startConformation.Atoms.Count != endConformation.Atoms.Count)
            { throw new Exception("Atom Count does not match in start and end conformation"); }

            var atomCount = startConformation.Atoms.Count;
            var atoms = startConformation.Atoms.GetAtoms().ToArray();
            var startPositions = startConformation.Atoms.GetPositions().ToArray();
            var endPositions = endConformation.Atoms.GetPositions().ToArray();

            for (int i = 0; i < atomCount; i++)
            {
                var p = Vector.Lerp(startPositions[i], endPositions[i], progress);

                conformation.Atoms.AddAtom(atoms[i].Element.Symbol, p);
            }

            // interpolate marker locRots
            var startMarkerLocRots = startConformation.Markers.GetLocRots().ToArray();
            var endMarkerLocRots = endConformation.Markers.GetLocRots().ToArray();

            if (startMarkerLocRots.Length != endMarkerLocRots.Length)
            { throw new Exception("Marker LocRots Count does not match in start and end conformation"); }

            for (int i = 0; i < startMarkerLocRots.Length; i++)
            {
                var locRot = new LocRotState();
                locRot.SetLerp(startMarkerLocRots[i], endMarkerLocRots[i], progress);

                conformation.Markers.Add(locRot);
            }

            // TODO interpolate Colliders?
        }

        private void EnsureConformation()
        {
            if (currentConformation == null)
            {
                SetConformation("default");
            }
        }

        private List<ConformationBuilder> conformations = new List<ConformationBuilder>();
        private ConformationBuilder currentConformation;
        /// <summary>
        /// Current conformation set by <see cref="SetConformation(string)"/>.
        /// If no conformation was set, a "default" conformation is automatically created.
        /// </summary>
        public ConformationBuilder CurrentConformation
        {
            get
            {
                EnsureConformation();
                return currentConformation;
            }
        }

        /// <summary>
        /// All conformations
        /// </summary>
        public IEnumerable<ConformationBuilder> Conformations { get { return this.conformations; } }

        /// <summary>
        /// Add atoms to the current conformation (<see cref="CurrentConformation"/>).
        /// For more details see <see cref="AtomsBuilder.Add{TItem}(IEnumerable{TItem}, Func{TItem, string}, Func{TItem, Vector})"/>
        /// </summary>
        /// <typeparam name="TItem">For more details see <see cref="AtomsBuilder.Add{TItem}(IEnumerable{TItem}, Func{TItem, string}, Func{TItem, Vector})"/></typeparam>
        /// <param name="collection">For more details see <see cref="AtomsBuilder.Add{TItem}(IEnumerable{TItem}, Func{TItem, string}, Func{TItem, Vector})"/></param>
        /// <param name="symbolFunc">For more details see <see cref="AtomsBuilder.Add{TItem}(IEnumerable{TItem}, Func{TItem, string}, Func{TItem, Vector})"/></param>
        /// <param name="positionFunc">For more details see <see cref="AtomsBuilder.Add{TItem}(IEnumerable{TItem}, Func{TItem, string}, Func{TItem, Vector})"/></param>
        public void AddAtoms<TItem>(IEnumerable<TItem> collection, Func<TItem, string> symbolFunc, Func<TItem, Vector> positionFunc)
        {
            CurrentConformation.Atoms.Add(collection, symbolFunc, positionFunc);
        }

        private int MarkerIndexById(string id)
        {
            for (int i = 0; i < markers.Count; i++)
            {
                var item = markers[i];
                if (item.Id == id)
                {
                    return i;
                }
            }
            return -1;
        }

        private void MarkerIntern(string id, LocRot locRot, Func<MarkerClass> markerFactory)
        {
            var index = MarkerIndexById(id);

            if (index == -1)
            {
                // not found -> create
                markers.Add(markerFactory());

                index = MarkerIndexById(id);

                EnsureConformation();

                // set conformation position in each conformation
                foreach (var conformation in this.conformations)
                {
                    conformation.Markers.Add(index, locRot);
                }
            }
            else
            {
                // marker already existing ->
                // set conformation position in current conformaiton
                this.CurrentConformation.Markers.Add(index, locRot);
            }
        }

        /// <summary>
        /// Create or set a <see cref="Markers.BindingSite"/> <see cref="LocRot"/>.
        /// If id does not exist yet, <see cref="Markers.BindingSite"/> will be created.
        /// <see cref="LocRot"/> is set only for the <see cref="CurrentConformation"/>.
        /// </summary>
        /// <param name="id"><see cref="Markers.BindingSite"/> ID</param>
        /// <param name="locRot"><see cref="Markers.BindingSite"/> <see cref="LocRot"/></param>
        public void BindingSite(string id, LocRot locRot)
        {
            MarkerIntern(id, locRot, () => new BindingSiteClass(id));
        }

        /// <summary>
        /// Gets the <see cref="LocRot"/> of a <see cref="Markers.Marker"/> by its ID.
        /// If the marker does not exist, an exception is thrown.
        /// </summary>
        /// <param name="id"><see cref="Markers.Marker"/> ID</param>
        /// <returns></returns>
        public LocRot GetMarkerLocRot(string id)
        {
            var index = MarkerIndexById(id);
            if (index == -1)
            {
                throw new Exception("Marker with id " + id + " not existing");
            }

            return this.CurrentConformation.Markers.GetLocRotAt(index);
        }

        /// <summary>
        /// Copy current conformation to a new conformation
        /// </summary>
        /// <param name="newConformationId"><see cref="Conformation.Id"/> of the new <see cref="Conformation"/></param>
        public void CopyCurrentConformation(string newConformationId)
        {
            var oldConformation = this.CurrentConformation;
            SetConformation(newConformationId);
            this.CurrentConformation.CopyFrom(oldConformation);
        }

        /// <summary>
        /// Create or set a <see cref= "Markers.Marker" /> <see cref= "LocRot" />.
        /// If id does not exist yet, a new <see cref="Markers.Marker"/> will be created.
        /// <see cref="LocRot"/> is set only for the <see cref="CurrentConformation"/>.
        /// </summary>
        /// <param name="id"><see cref="Markers.Marker"/> ID</param>
        /// <param name="locRot"><see cref="Markers.Marker"/> <see cref="LocRot"/></param>
        public void Marker(string id, LocRot locRot)
        {
            MarkerIntern(id, locRot, () => new MarkerClass(id));
        }

        /// <summary>
        /// Create or set a <see cref= "Markers.Sensor" /> <see cref= "LocRot" />.
        /// If id does not exist yet, a new <see cref="Markers.Sensor"/> will be created.
        /// If id already exists, range and apertureAngle will not be updated.
        /// <see cref="LocRot"/> is set only for the <see cref="CurrentConformation"/>.
        /// </summary>
        /// <param name="id"><see cref="Markers.Marker"/> ID</param>
        /// <param name="locRot"><see cref="Markers.Marker"/> <see cref="LocRot"/></param>
        public void Sensor(string id, float range, float apertureAngle, LocRot locRot)
        {
            MarkerIntern(id, locRot, () => new SensorClass(id, range, apertureAngle));
        }

        /// <summary>
        /// Updates the <see cref="LocRot"/> of a <see cref="Markers.Sensor"/> with a specific ID.
        /// If the ID does not exist yet, a new <see cref="Markers.Sensor"/> is created with range=10 and apertureAngle=1.0
        /// </summary>
        /// <param name="id"><see cref="Markers.Sensor"/> ID</param>
        /// <param name="locRot"><see cref="Markers.Sensor"/> <see cref="LocRot"/></param>
        public void Sensor(string id, LocRot locRot)
        {
            Sensor(id, 10, 1.0f, locRot);
        }

        private List<MarkerClass> markers = new List<MarkerClass>();

        /// <summary>
        /// Default color for new conformations
        /// </summary>
        public Color DefaultColor { get; set; }

        /// <summary>
        /// ID of the Behavior
        /// </summary>
        public string BehaviorId { get; set; }

        /// <summary>
        /// Create a new <see cref="EntityClass"/> instance from the specified properties in this class.
        /// </summary>
        /// <returns></returns>
        public EntityClass Create()
        {
            EnsureConformation();

            var conformations = this.conformations.Select(b => b.CreateInstance());
            var structureClass = new EntityStructureClass(id, this.conformations[0].Atoms.GetAtoms(), conformations);
            var entityClass = new EntityClass(id, structureClass, this.BehaviorId, markers);
            return entityClass;
        }

        /// <summary>
        /// Add atom to <see cref="CurrentConformation"/>.
        /// See <see cref="AtomsBuilder.AddAtom(string, Vector)"/>.
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="position"></param>
        public void AddAtom(string symbol, Vector position)
        {
            this.CurrentConformation.Atoms.AddAtom(Element.BySymbol(symbol), position);
        }

        /// <summary>
        /// Add atom to <see cref="CurrentConformation"/>.
        /// See <see cref="AtomsBuilder.AddAtom(Element, Vector)"/>.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="position"></param>
        public void AddAtom(Element element, Vector position)
        {
            this.CurrentConformation.Atoms.AddAtom(element, position);
        }

        /// <summary>
        /// Use the center point of all atoms in the <see cref="CurrentConformation"/> as new origin.
        /// </summary>
        public void ReCenter()
        {
            this.SetOrigin(
                this.CurrentConformation.Atoms.CalcCenter()
            );
        }

        /// <summary>
        /// Move atoms and markers by setting a new origin for all conformations.
        /// </summary>
        /// <param name="origin">new origin</param>
        public void SetOrigin(Vector origin)
        {
            foreach (var conformation in this.conformations)
            {
                conformation.SetOrigin(origin);
            }
        }
    }
}
