using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MolecularMachines.Framework.Model.Behaviors
{
    /// <summary>
    /// Attribute that can be used on properties in <see cref="EntityBehavior"/> 
    /// classes to inject a <see cref="Model.Entities.Entity"/> object, or a 
    /// <see cref="Model.Behaviors.EntityBehavior"/> object of a entity bound to
    /// a specific binding site.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class BoundEntityAttribute : Attribute
    {
        /// <summary>
        /// <see cref="Model.Entities.Entity"/> or <see cref="Model.Entities.EntityBehavior"/> injection attribute.
        /// </summary>
        /// <param name="bindingSiteId">ID of the binding site</param>
        public BoundEntityAttribute(string bindingSiteId)
        {
            this.BindingSiteId = bindingSiteId;
        }

        /// <summary>
        /// ID of the binding site
        /// </summary>
        public string BindingSiteId { get; private set; }
    }
}
