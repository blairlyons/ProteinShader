using MolecularMachines.Framework.DataStructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MolecularMachines.Framework.Model.Behaviors;
using System.Reflection;
using MolecularMachines.Framework.Utils;
using MolecularMachines.Framework.Logging;
using MolecularMachines.Framework.Model.Markers;
using MolecularMachines.Framework.Model.ConcentrationControllers.Presents;

namespace MolecularMachines.Framework.Model.Entities
{
    /// <summary>
    /// Holds the definition of an <see cref="Entity"/>.
    /// Can be used to create <see cref="Entity"/> instances (<see cref="EntityClass.CreateInstance"/>).
    /// </summary>
    public class EntityClass
    {
        /// <summary>
        /// New <see cref="EntityClass"/>
        /// </summary>
        /// <param name="id">ID of the class</param>
        /// <param name="structureClass">Structure</param>
        /// <param name="behaviorId">ID of the behavior</param>
        /// <param name="markers"></param>
        public EntityClass(string id, EntityStructureClass structureClass, string behaviorId, IEnumerable<MarkerClass> markers)
        {
            this.Id = id;
            this.StructureClass = structureClass;
            this.BehaviorId = behaviorId;

            this.Markers = new ImmutableList<MarkerClass>(markers);

            foreach (var conformation in this.StructureClass.Conformations)
            {
                if (conformation.MarkerLocRots.Count != this.Markers.Count)
                { throw new ArgumentException("BidingSiteLocRots count in conformation does not match BindingSites count"); }
            }

            ResolveBehaviorType();
        }

        /// <summary>
        /// Returns the index of a marker by its ID.
        /// If the ID does not exist, an exception is thrown.
        /// </summary>
        /// <param name="id">marker ID</param>
        /// <returns></returns>
        public int MarkerIndexById(string id)
        {
            for (int i = 0; i < this.Markers.Count; i++)
            {
                var marker = this.Markers[i];
                if (marker.Id == id) { return i; }
            }

            throw new Exception($"no marker with id \"{id}\" found");
        }

        private void ResolveBehaviorType()
        {
            if (entityBehaviorIdTypeMapping == null)
            {
                entityBehaviorIdTypeMapping = new AnnotatedIdTypeMapping<EntityBehaviorIdAttribute, EntityBehavior>(false, typeof(EmptyBehavior));
            }

            this.behaviorType = entityBehaviorIdTypeMapping.GetTypeById(this.BehaviorId);
        }

        private static AnnotatedIdTypeMapping<EntityBehaviorIdAttribute, EntityBehavior> entityBehaviorIdTypeMapping = null;

        /// <summary>
        /// Creates the Behavior for an <see cref="Entity"/>.
        /// </summary>
        /// <param name="entity">Entity to create a behavior for.</param>
        /// <returns></returns>
        public EntityBehavior CreateBehavior(Entity entity)
        {
            var behaviorObj = Activator.CreateInstance(this.behaviorType);
            EntityBehavior behavior = (EntityBehavior)behaviorObj;
            behavior.Owner = entity;
            return behavior;
        }

        /// <summary>
        /// ID of this class
        /// </summary>
        public string Id { get; private set; }

        /// <summary>
        /// ID of behavior
        /// </summary>
        public string BehaviorId { get; private set; }
        private Type behaviorType;

        /// <summary>
        /// Structure class
        /// </summary>
        public EntityStructureClass StructureClass { get; private set; }

        /// <summary>
        /// Markers
        /// </summary>
        public ImmutableList<MarkerClass> Markers { get; private set; }
    }
}
