using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MolecularMachines.Framework.Model.ConcentrationControllers
{
    /// <summary>
    /// Assigns an ID to a <see cref="EntityFactory"/> implementation class.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class EntityFactoryIdAttribute : Attribute, IStringId
    {
        /// <summary>
        /// Assigns an ID to a <see cref="EntityFactory"/> implementation class.
        /// </summary>
        /// <param name="id">ID</param>
        public EntityFactoryIdAttribute(string id)
        {
            this.Id = id;
        }

        /// <summary>
        /// ID
        /// </summary>
        public string Id { get; private set; }
    }
}
