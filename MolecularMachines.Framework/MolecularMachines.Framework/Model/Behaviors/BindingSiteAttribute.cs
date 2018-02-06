using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MolecularMachines.Framework.Model.Behaviors
{
    /// <summary>
    /// Attribute that can be used on properties in <see cref="EntityBehavior"/> classes to inject a <see cref="Model.Markers.BindingSite"/> object.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class BindingSiteAttribute : Attribute
    {
        /// <summary>
        /// <see cref="Model.Markers.BindingSite"/> injection attribute.
        /// </summary>
        /// <param name="bindingSiteId">ID of the binding site</param>
        public BindingSiteAttribute(string bindingSiteId) {
            this.Id = bindingSiteId;
        }

        /// <summary>
        /// ID of the binding site
        /// </summary>
        public string Id { get; private set; }
    }
}
