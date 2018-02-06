using MolecularMachines.Framework.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MolecularMachines.Framework.Model.ConcentrationControllers
{
    /// <summary>
    /// Abstract EntityFactory class.
    /// An implemented class can be used to Identify, Create and Delete <see cref="Entity"/>s that meet certain conditions.
    /// To identify an implementation using an ID, the implemented class must be attributed with the <see cref="EntityFactoryIdAttribute"/>.
    /// </summary>
    public abstract class EntityFactory
    {
        /// <summary>
        /// Is called by <see cref="MMEnvironment"/> to initialize.
        /// </summary>
        /// <param name="environment"><see cref="MMEnvironment"/> this factory belongs to</param>
        public virtual void Init(MMEnvironment environment)
        {
            this.environment = environment;
        }

        protected MMEnvironment environment;

        /// <summary>
        /// Checks if an <see cref="Entity"/> is handled by this <see cref="EntityFactory"/>.
        /// </summary>
        /// <param name="entity"><see cref="Entity"/> to test</param>
        /// <returns>true, if the <see cref="Entity"/> is handled by this <see cref="EntityFactory"/>, false otherwise</returns>
        public abstract bool Filter(Entity entity);

        /// <summary>
        /// Create a new instance o <see cref="Entity"/>.
        /// </summary>
        public abstract void Create();

        /// <summary>
        /// Delete an <see cref="Entity"/> instance.
        /// </summary>
        /// <param name="entity"><see cref="Entity"/> to delete</param>
        public abstract void Delete(Entity entity);
    }
}
