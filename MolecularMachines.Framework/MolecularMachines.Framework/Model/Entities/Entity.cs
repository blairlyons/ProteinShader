using MolecularMachines.Framework.Geometry;
using MolecularMachines.Framework.Model.Behaviors;
using MolecularMachines.Framework.Model.Behaviors.Presents;
using MolecularMachines.Framework.Model.Compounds;
using MolecularMachines.Framework.Model.Markers;
using MolecularMachines.Framework.Model.SpatialLinks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MolecularMachines.Framework.Model.Entities
{
    /// <summary>
    /// An <see cref="Entity"/> represents the smallest, indivisible unit of a molecule in a scene.
    /// 
    /// The property <see cref="Compound"/> must be set before calling <see cref="Update"/>.
    /// </summary>
    public class Entity : IFrameUpdate, ISpaceInherent, IIsDisposed
    {
        public Entity(MMEnvironment environment, EntityClass entityClass)
        {
            if (environment == null) { throw new ArgumentNullException(nameof(environment)); }
            if (entityClass == null) { throw new ArgumentNullException(nameof(entityClass)); }

            this.Environment = environment;
            this.Class = entityClass;
            this.Structure = entityClass.StructureClass.CreateInstance();

            var allMarkers = entityClass.Markers.Select(markerClass => markerClass.CreateInstance(this)).ToArray();
            this.BindingSites = allMarkers.Where(m => m is BindingSite).Select(m => (BindingSite)m).ToArray();
            this.Sensors = allMarkers.Where(m => m is Sensor).Select(m => (Sensor)m).ToArray();
            this.OtherMarkers = allMarkers.Except(this.BindingSites).Except(this.Sensors).ToArray();

            this.Behavior = entityClass.CreateBehavior(this);

            this.LocRot = new LocRotState();
        }

        /// <summary>
        /// Returns a <see cref="BindingSite"/> by its ID.
        /// If it does not exist, an exception is thrown.
        /// </summary>
        /// <param name="id">ID to find</param>
        /// <returns>Found item</returns>
        public BindingSite BindingSiteById(string id)
        {
            foreach (var item in this.BindingSites)
            {
                if (item.Id == id) { return item; }
            }

            throw new Exception("BindingSite with id \"" + id + "\" not found");
        }

        /// <summary>
        /// Returns a <see cref="Sensor"/> by its ID.
        /// If it does not exist, an exception is thrown.
        /// </summary>
        /// <param name="id">ID to find</param>
        /// <returns>Found item</returns>
        public Sensor SensorById(string id)
        {
            foreach (var item in this.Sensors)
            {
                if (item.Id == id) { return item; }
            }

            throw new Exception("Sensor with id \"" + id + "\" not found");
        }

        /// <summary>
        /// <see cref="MMEnvironment"/> this instance belongs to.
        /// </summary>
        public MMEnvironment Environment { get; private set; }

        /// <summary>
        /// Class of this instance.
        /// </summary>
        public EntityClass Class { get; private set; }

        /// <summary>
        /// Returns a <see cref="Marker"/> by its ID.
        /// If it does not exist, an exception is thrown.
        /// </summary>
        /// <param name="id">ID to find</param>
        /// <returns>Found item</returns>
        public Marker MarkerById(string id)
        {
            foreach (var item in this.AllMarkers)
            {
                if (item.Id == id) { return item; }
            }

            throw new Exception("marker with id \"" + id + "\" not found");
        }

        /// <summary>
        /// Contains all <see cref="BindingSite"/>s.
        /// </summary>
        public BindingSite[] BindingSites { get; private set; }
        /// <summary>
        /// Contains all <see cref="Sensor"/>s.
        /// </summary>
        public Sensor[] Sensors { get; private set; }
        /// <summary>
        /// Contains all <see cref="Marker"/>s that are not <see cref="BindingSite"/>s and not <see cref="Sensor"/>s.
        /// </summary>
        public Marker[] OtherMarkers { get; private set; }
        /// <summary>
        /// Contains all <see cref="Marker"/>s.
        /// </summary>
        public IEnumerable<Marker> AllMarkers
        {
            get
            {
                return this.OtherMarkers.Concat<Marker>(this.BindingSites).Concat(this.Sensors);
            }
        }

        /// <summary>
        /// Structure
        /// </summary>
        public EntityStructure Structure { get; private set; }
        /// <summary>
        /// Behavior that controls this instance.
        /// </summary>
        public EntityBehavior Behavior { get; private set; }

        /// <summary>
        /// Compound this instance belongs to.
        /// </summary>
        public Compound Compound { get; set; }

        /// <summary>
        /// Current LocRot. Is set by <see cref="Compound"/>.
        /// </summary>
        public LocRot LocRot { get; protected set; }

        /// <summary>
        /// Must be called every frame.
        /// Calls <see cref="EntityBehavior.Update"/> of <see cref="Behavior"/>.
        /// </summary>
        public void Update()
        {
            Behavior.Update();

            //foreach (var marker in this.Markers)
            //{
            //    marker.Update();
            //}

            OnUpdate();
        }

        /// <summary>
        /// Is called when <see cref="Update"/> is called.
        /// </summary>
        protected virtual void OnUpdate()
        {

        }

        /// <summary>
        /// Deletes this instance.
        /// </summary>
        public void Dispose()
        {
            OnDispose();
            this.isDisposed = true;
        }

        /// <summary>
        /// Is called when <see cref="Dispose"/> is called.
        /// Disposes the <see cref="Behavior"/> and releases all bonds
        /// </summary>
        protected virtual void OnDispose()
        {
            // Dispose behavior
            Behavior.Dispose();

            // Release all bonds --> leaves compound
            foreach (var bindingSite in this.BindingSites)
            {
                bindingSite.ReleaseBond();
            }

            // release compound
            if (this.Compound != null)
            {
                this.Compound.Dispose(); // Because all BindingSites were released previously, this entity has an own Compound which now can be disposed.
            }
        }


        private bool isDisposed = false;
        /// <summary>
        /// true, if <see cref="Dispose"/> was called.
        /// </summary>
        public bool IsDisposed
        {
            get
            {
                return isDisposed;
            }
        }

        bool ISpaceInherent.IsAlive
        {
            get
            {
                return !this.IsDisposed;
            }
        }

        private static int counter;
        private int id = counter++;
        /// <summary>
        /// ID for Debug purposes.
        /// </summary>
        public string DiagnosticId
        {
            get { return Class.Id + "(" + id.ToString("X") + ")"; }
        }

        public override string ToString()
        {
            var s = DiagnosticId + " [ ";

            for (int i = 0; i < this.BindingSites.Length; i++)
            {
                var bindingSite = this.BindingSites[i];
                if (i != 0) { s += " , "; }

                if (bindingSite.IsFree)
                {
                    s += "_";
                }
                else if (bindingSite.IsBound)
                {
                    s += bindingSite.OtherSite.Owner.DiagnosticId;
                }
                else if (bindingSite.IsBinding)
                {
                    s += "~" + bindingSite.OtherSite.Owner.DiagnosticId;
                }
                else { s += "?"; }
            }

            s += " ]";

            if (this.Compound == null)
            {
                s += " compundless";
            }

            return s;
        }
    }
}
