using MolecularMachines.Framework.DataStructures;
using MolecularMachines.Framework.Geometry;
using MolecularMachines.Framework.Logging;
using MolecularMachines.Framework.Model.Entities;
using MolecularMachines.Framework.Model.SpatialLinks;
using MolecularMachines.Framework.Model.Trajectories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MolecularMachines.Framework.Model.Compounds
{
    /// <summary>
    /// Represents a compound of bound together <see cref="Entities"/>
    /// </summary>
    public class Compound : IFrameUpdate, IIsDisposed
    {
        /// <summary>
        /// New instance.
        /// After this, <see cref="Start"/> must be called.
        /// </summary>
        /// <param name="environment">Environment this new compound belongs to</param>
        /// <param name="rootEntity">arbitrary root entity of the compound</param>
        public Compound(MMEnvironment environment, Entity rootEntity)
        {
            if (environment == null) { throw new ArgumentNullException(nameof(environment)); }
            if (rootEntity == null) { throw new ArgumentNullException(nameof(rootEntity)); }

            this.Environment = environment;
            this.RootEntity = rootEntity;
            this.Entities = new IterateList<Entity>();

            // create an initial SpatialLink to avoid null
            this.SpatialLink = new SpatialLinkDummy(SpatialLinkType.Undefined, this.RootEntity.LocRot, true);
        }

        /// <summary>
        /// must be called after constructor.
        /// Is necessary, becuase otherwise OnEntityAdd would be called before the subtype-constructor is called
        /// </summary>
        public void Start()
        {
            this.AddEntity(this.RootEntity);
        }

        /// <summary>
        /// Environment
        /// </summary>
        public MMEnvironment Environment { get; private set; }
        /// <summary>
        /// <see cref="SpatialLink"/> of the compound
        /// </summary>
        public SpatialLink SpatialLink { get; private set; }
        /// <summary>
        /// Arbitrary root of the compound.
        /// </summary>
        public Entity RootEntity { get; private set; }
        /// <summary>
        /// List containing all entities of the compound, including <see cref="RootEntity"/>.
        /// </summary>
        public IterateList<Entity> Entities { get; private set; }

        /// <summary>
        /// Remove entities from this <see cref="Compound"/>.
        /// </summary>
        /// <param name="entitiesToRemove">Entities that must be removed</param>
        public void RemoveEntities(HashSet<Entity> entitiesToRemove)
        {
            using (var enumerator = this.Entities.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    var entity = enumerator.Current;

                    if (entitiesToRemove.Contains(entity))
                    {
                        enumerator.Remove();
                        this.OnEntityRemove(entity);
                    }
                }
            }
        }

        /// <summary>
        /// Add a new entity to this <see cref="Compound"/>
        /// </summary>
        /// <param name="entity">new <see cref="Entity"/>. <see cref="Entity.Compound"/> must be null.</param>
        public void AddEntity(Entity entity)
        {
            if (entity.Compound != null) { throw new Exception("A entity can only be added to a compound if entity.Compound==null."); }

            this.Entities.Add(entity);
            this.OnEntityAdd(entity);
        }

        private float mass;
        /// <summary>
        /// Get or set the mass of the compound.
        /// </summary>
        public float Mass
        {
            get { return this.mass; }
            set
            {
                if (value != this.mass)
                {
                    this.mass = value;
                    this.OnMassChanged();
                }
            }
        }

        private void UpdateMass()
        {
            this.Mass = this.Entities.Sum(entity => entity.Structure.Class.Mass);
        }

        /// <summary>
        /// Called when the <see cref="Mass"/> changed.
        /// </summary>
        protected virtual void OnMassChanged()
        { }

        /// <summary>
        /// Merge the <see cref="Entity"/>s of another <see cref="Compound"/> to this <see cref="Compound"/>.
        /// </summary>
        /// <param name="otherCompund"></param>
        public void Merge(Compound otherCompund)
        {
            if (this == otherCompund)
            {
                // merge this compound with itself
                // -> nothing to do here
            }
            else
            {
                // save needed data
                var otherSpatialLink = otherCompund.SpatialLink;
                var otherRootEntity = otherCompund.RootEntity;
                var otherEntities = otherCompund.Entities.ToArray();

                // dispose otherCompund.
                otherCompund.Dispose();

                // add the entities from otherCompound
                foreach (var entity in otherEntities)
                {
                    this.AddEntity(entity);
                }

                // Update properties
                UpdateMass();
            }
        }

        /// <summary>
        /// Must be called, when some <see cref="Entity"/>s may not bound together anymore.
        /// The slit away <see cref="Entity"/>s are then assigned to a new <see cref="Compound"/> that follows a defined <see cref="Trajectory"/>.
        /// </summary>
        /// <param name="trajectory">Split trajectory</param>
        public void Split(Trajectory trajectory)
        {
            var splitter = new CompoundSplitter(this);
            splitter.Split(trajectory);

            // Update properties
            UpdateMass();
        }

        /// <summary>
        /// Called when an <see cref="Entity"/> is added to the compound.
        /// </summary>
        /// <param name="entity">new entity</param>
        protected virtual void OnEntityAdd(Entity entity)
        {
            entity.Compound = this;
        }

        /// <summary>
        /// Called when an <see cref="Entity"/> is removed from the compound.
        /// </summary>
        /// <param name="entity">removed entity</param>
        protected virtual void OnEntityRemove(Entity entity)
        {
            entity.Compound = null;
        }

        /// <summary>
        /// Must be called every frame.
        /// Aligns its <see cref="Entities"/>.
        /// And updates <see cref="SpatialLink"/>.
        /// </summary>
        public void Update()
        {
            if (!this.IsDisposed)
            {
                new CompoundEntityAligner().Align(this.RootEntity, this.SpatialLink.LocRot);

                this.SpatialLink.Update();
            }

            if (!this.IsDisposed)
            {
                this.OnUpdate();
            }

        }

        /// <summary>
        /// Is called when <see cref="Update"/> is called and this is not disposed yet.
        /// </summary>
        protected virtual void OnUpdate()
        {

        }

        /// <summary>
        /// Is called when <see cref="Dispose"/> is called.
        /// </summary>
        protected virtual void OnDispose()
        {

        }

        /// <summary>
        /// Position <see cref="Compound"/> at a fixed <see cref="LocRot"/>.
        /// </summary>
        /// <param name="fixedLocRot">Fixed <see cref="LocRot"/></param>
        public void Fix(LocRot fixedLocRot)
        {
            this.SpatialLink = OnFix(fixedLocRot);
        }

        /// <summary>
        /// Is called when <see cref="Fix(LocRot)"/> is called.
        /// Must return a corresponding <see cref="SpatialLink"/> instance.
        /// </summary>
        /// <param name="fixedLocRot">Fixed <see cref="LocRot"/></param>
        /// <returns></returns>
        protected virtual SpatialLink OnFix(LocRot fixedLocRot)
        {
            return new SpatialLinkDummy(SpatialLinkType.Fixed, fixedLocRot, true);
        }

        /// <summary>
        /// Lets this <see cref="Compound"/> float starting at current position.
        /// </summary>
        public void Float()
        {
            this.Float(this.SpatialLink.LocRot);
        }

        /// <summary>
        /// Lets this <see cref="Compound"/> float starting at a defined position.
        /// </summary>
        /// <param name="initialLocRot">start position</param>
        public void Float(LocRot initialLocRot)
        {
            this.SpatialLink = OnFloat(initialLocRot);
        }


        /// <summary>
        /// Is called when <see cref="Float(LocRot)"/> is called.
        /// Must return a corresponding <see cref="SpatialLink"/> instance.
        /// </summary>
        /// <param name="initialLocRot">starting <see cref="LocRot"/></param>
        /// <returns></returns>
        protected virtual SpatialLink OnFloat(LocRot initialLocRot)
        {
            return new SpatialLinkDummy(SpatialLinkType.Floating, initialLocRot, true);
        }

        /// <summary>
        /// Lets this <see cref="Compound"/> follow a defined <see cref="Trajectories.Trajectory"/>.
        /// </summary>
        /// <param name="trajectory">trajectory</param>
        public void Trajectory(Trajectory trajectory)
        {
            this.SpatialLink = OnTrajectory(trajectory);
        }
        /// <summary>
        /// Is called when <see cref="Trajectory(Trajectories.Trajectory)"/> is called.
        /// Must return a corresponding <see cref="SpatialLink"/> instance.
        /// </summary>
        /// <param name="trajectory">trajectory</param>
        /// <returns></returns>
        protected virtual SpatialLink OnTrajectory(Trajectory trajectory)
        {
            return new SpatialLinkDummy(SpatialLinkType.Trajectory, LocRot.Zero, true);
        }

        /// <summary>
        /// Dispose this instance.
        /// </summary>
        public void Dispose()
        {
            if (!this.IsDisposed)
            {
                this.OnDispose();

                // remove all entities (also includes RootEntity)
                using (var enumerator = this.Entities.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        var entity = enumerator.Current;
                        enumerator.Remove();
                        this.OnEntityRemove(entity);
                    }
                }

                // reset RootEntity. This also indicates, that the Compound is Disposed
                this.RootEntity = null;

                // release other resources
                this.SpatialLink = null;
            }
        }

        /// <summary>
        /// true, if <see cref="Dispose"/> was called.
        /// </summary>
        public bool IsDisposed
        {
            get
            {
                return (RootEntity == null);
            }
        }
    }
}
