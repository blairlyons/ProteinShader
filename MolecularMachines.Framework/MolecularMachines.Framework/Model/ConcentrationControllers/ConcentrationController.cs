using MolecularMachines.Framework.Logging;
using MolecularMachines.Framework.Model.ConcentrationControllers.Presents;
using MolecularMachines.Framework.Model.Entities;
using MolecularMachines.Framework.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MolecularMachines.Framework.Model.ConcentrationControllers
{
    /// <summary>
    /// Represents a concentration controller, that can be used to adjust the concentration of specific entities in a <see cref="MMEnvironment"/>.
    /// </summary>
    public class ConcentrationController : IFrameUpdate
    {
        /// <summary>
        /// New instance.
        /// </summary>
        /// <param name="environment"><see cref="MMEnvironment"/></param>
        /// <param name="entityFactoryId">ID of the <see cref="EntityFactory"/> (attributed with <see cref="EntityFactoryIdAttribute"/>)</param>
        /// <param name="name">Display name</param>
        /// <param name="minCount">Minimum count of the controller</param>
        /// <param name="maxCount">Maximum count of the controller</param>
        public ConcentrationController(MMEnvironment environment, string entityFactoryId, string name, int minCount, int maxCount)
        {
            if (environment == null) { throw new ArgumentNullException(nameof(environment)); }

            this.EntityFactoryId = entityFactoryId;

            this.environment = environment;

            this.Name = name;
            this.MinCount = minCount;
            this.MaxCount = maxCount;
            
            ResolveEntityFactory();
        }

        /// <summary>
        /// Display name
        /// </summary>
        public string Name { get; private set; }
        /// <summary>
        /// Minimum count of the contorller
        /// </summary>
        public int MinCount { get; private set; }
        /// <summary>
        /// Maximum count of the contorller
        /// </summary>
        public int MaxCount { get; private set; }
        /// <summary>
        /// ID of the <see cref="EntityFactory"/> (attributed with <see cref="EntityFactoryIdAttribute"/>)
        /// </summary>
        public string EntityFactoryId { get; private set; }

        /// <summary>
        /// Gets the current actual count.
        /// </summary>
        public int CurrentCount { get; private set; }

        private MMEnvironment environment;
        private EntityFactory entityFactory;

        private int desiredCount;
        private bool setCount = false;

        /// <summary>
        /// Sets the desired count.
        /// </summary>
        /// <param name="count">desired count</param>
        public void SetCount(int count)
        {
            this.desiredCount = count;
            this.setCount = true;
        }

        private static AnnotatedIdTypeMapping<EntityFactoryIdAttribute, EntityFactory> entityFactoryIdTypeMapping = null;

        private void ResolveEntityFactory()
        {
            if (entityFactoryIdTypeMapping == null)
            {
                entityFactoryIdTypeMapping = new AnnotatedIdTypeMapping<EntityFactoryIdAttribute, EntityFactory>(false, typeof(EmptyEntityFactory));
            }

            var type = entityFactoryIdTypeMapping.GetTypeById(this.EntityFactoryId);
            this.entityFactory = (EntityFactory)Activator.CreateInstance(type);
            this.entityFactory.Init(this.environment);
        }

        /// <summary>
        /// Must be called every frame.
        /// Removes or adds <see cref="Entity"/>s according to the desired count.
        /// </summary>
        public void Update()
        {
            var matchinEntities = this.environment.Entities.Where(this.entityFactory.Filter);
            this.CurrentCount = matchinEntities.Count();

            if (this.setCount)
            {
                this.setCount = false;

                var delta = this.CurrentCount - this.desiredCount;

                if (delta != 0)
                {
                    Log.Info("CC: " + Name + " with delta: " + delta);
                }

                while (delta < 0)
                {
                    entityFactory.Create();
                    delta++;
                }

                if (delta > 0)
                {
                    var toDelete = matchinEntities.Skip(this.CurrentCount - delta).ToArray();

                    foreach (var randomEntity in toDelete)
                    {
                        entityFactory.Delete(randomEntity);
                    }
                }
            }
        }
    }
}
