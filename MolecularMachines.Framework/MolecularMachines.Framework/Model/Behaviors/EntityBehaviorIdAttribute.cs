using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MolecularMachines.Framework.Model.Behaviors
{
    /// <summary>
    /// Assignes an ID to a <see cref="EntityBehavior"/> class.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class EntityBehaviorIdAttribute : Attribute, IStringId
    {
        public EntityBehaviorIdAttribute(string id)
        {
            this.Id = id;
        }

        public string Id { get; private set; }
    }
}
