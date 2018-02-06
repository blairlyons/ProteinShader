using MolecularMachines.Framework.Geometry;
using MolecularMachines.Framework.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MolecularMachines.Framework.Model.Entities
{
    /// <summary>
    /// The structure of an <see cref="Entity"/>. Contains the current local positions of atoms, binding sites, etc.
    /// Individual <see cref="Entity"/> usually have individual <see cref="EntityStructure"/> instances.
    /// However, <see cref="Entity"/>s which do never change, or only change together can share a <see cref="EntityStructure"/> instance (for memory and speed optimization reasons).
    /// </summary>
    public class EntityStructure : IFrameUpdate
    {
        /// <summary>
        /// New instance.
        /// </summary>
        /// <param name="id">ID of the structure</param>
        /// <param name="entityStructureClass">StructureClass that is used as template</param>
        public EntityStructure(string id, EntityStructureClass entityStructureClass)
        {
            this.Id = id;

            this.Class = entityStructureClass;

            var c = entityStructureClass.InitialConformation;

            this.CurrentConformation = c;

            this.CurrentAtomPositions = c.AtomPositions.ToArray();
            this.CurrentMarkerLocRots = c.MarkerLocRots.Select(lr => new LocRotState(lr)).ToArray();
            this.CurrentColor = c.Color;
        }

        /// <summary>
        /// ID
        /// </summary>
        public string Id { get; private set; }

        /// <summary>
        /// Structure Class that was used as template
        /// </summary>
        public EntityStructureClass Class { get; private set; }

        /// <summary>
        /// Current Atom Positions
        /// </summary>
        public Vector[] CurrentAtomPositions { get; private set; }
        /// <summary>
        /// Current Marker <see cref="LocRot"/>s.
        /// </summary>
        public LocRotState[] CurrentMarkerLocRots { get; private set; }
        /// <summary>
        /// Current Color
        /// </summary>
        public Color CurrentColor { get; set; }
        /// <summary>
        /// Current Conformation
        /// </summary>
        public Conformation CurrentConformation { get; private set; }

        private Vector[] previousAtomPositions;
        private LocRotState[] previousMarkerLocRots;
        private Color previousColor;

        private Timer conformationChangeTimer = new Timer(TimeSpan.FromSeconds(1.0));
        private bool conformationChanging = false;

        /// <summary>
        /// Is true, when the structure changed after <see cref="ExecuteIfDirty(Action)"/> was called.
        /// </summary>
        public bool IsDirty { get; private set; }
        /// <summary>
        /// Executes an <see cref="Action"/> when <see cref="IsDirty"/> is true, and sets <see cref="IsDirty"/> to false.
        /// </summary>
        /// <param name="a"></param>
        public void ExecuteIfDirty(Action a)
        {
            if (IsDirty)
            {
                IsDirty = false;
                a();
            }
        }

        private Conformation ConformationById(string conformationId)
        {
            foreach (var c in Class.Conformations)
            {
                if (c.Id == conformationId) { return c; }
            }
            return null;
        }

        /// <summary>
        /// Changes the <see cref="Conformation"/> of the structure in a specified amount of time.
        /// </summary>
        /// <param name="conformationId">ID of the new conformation</param>
        /// <param name="duration">Duration in seconds</param>
        public void SetConformation(string conformationId, float duration = 1.0f)
        {
            var c = ConformationById(conformationId);
            if (c == null)
            { throw new Exception("Conforamtion with id \"" + conformationId + "\" not found"); }
            else
            { SetConformation(c, duration); }
        }

        /// <summary>
        /// Changes the <see cref="Conformation"/> of the structure in a specified amount of time.
        /// </summary>
        /// <param name="conformation">new conformation</param>
        /// <param name="duration">Duration in seconds</param>
        public void SetConformation(Conformation conformation, float duration = 1.0f)
        {
            if (conformation != this.CurrentConformation)
            {
                if (this.previousAtomPositions == null) { this.previousAtomPositions = new Vector[this.CurrentAtomPositions.Length]; }
                if (this.previousMarkerLocRots == null)
                {
                    this.previousMarkerLocRots = new LocRotState[this.CurrentMarkerLocRots.Length];
                    for (int i = 0; i < this.previousMarkerLocRots.Length; i++)
                    {
                        this.previousMarkerLocRots[i] = new LocRotState();
                    }
                }

                // copy old Atom-Positions
                for (int i = 0; i < this.CurrentAtomPositions.Length; i++)
                {
                    this.previousAtomPositions[i] = this.CurrentAtomPositions[i];
                }

                // copy old marker LocRots
                for (int i = 0; i < this.previousMarkerLocRots.Length; i++)
                {
                    this.previousMarkerLocRots[i].Set(this.CurrentMarkerLocRots[i]);
                }

                // copy old color
                this.previousColor = this.CurrentColor;

                this.CurrentConformation = conformation;

                conformationChangeTimer = new Timer(TimeSpan.FromSeconds(duration));

                conformationChanging = true;
                IsDirty = true;
            }
        }

        /// <summary>
        /// Must be called every frame.
        /// </summary>
        public void Update()
        {
            if (conformationChanging)
            {
                IsDirty = true;

                float p;
                if (this.conformationChangeTimer.IsDone(out p))
                {
                    conformationChanging = false;
                }

                var f = ProgressTransfrom(p);

                // Position atoms
                for (int i = 0; i < CurrentAtomPositions.Length; i++)
                {
                    var previous = this.previousAtomPositions[i];
                    var destination = this.CurrentConformation.AtomPositions[i];

                    CurrentAtomPositions[i] = Vector.Lerp(previous, destination, f);
                }

                // Position markers
                for (int i = 0; i < CurrentMarkerLocRots.Length; i++)
                {
                    var previous = this.previousMarkerLocRots[i];
                    var destination = this.CurrentConformation.MarkerLocRots[i];

                    this.CurrentMarkerLocRots[i].SetLerp(previous, destination, f);
                }

                // Color
                this.CurrentColor = Color.Lerp(this.previousColor, this.CurrentConformation.Color, f);
            }
        }

        /// <summary>
        /// Tranforms the progress in a non-linear way.
        /// </summary>
        /// <param name="p">progress</param>
        /// <returns>transformed progress</returns>
        public static float ProgressTransfrom(float p)
        {
            var f = 1 - p;
            f = -((f * f * f) - 1);

            return f;
        }

        /// <summary>
        /// Get the current Marker <see cref="LocRot"/> by the <see cref="Markers.Marker"/> ID.
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <param name="markerId">Marker ID</param>
        /// <returns></returns>
        public LocRot MarkerLocalLocRotById(Entity entity, string markerId)
        {
            var index = entity.Class.MarkerIndexById(markerId);
            return this.CurrentMarkerLocRots[index];
        }
    }
}
