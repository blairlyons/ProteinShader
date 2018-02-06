using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MolecularMachines.Framework.Model.Behaviors
{
    /// <summary>
    /// Attribute that can be used on properties in <see cref="EntityBehavior"/> classes to inject a <see cref="Model.Markers.Marker"/> object.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class MarkerAttribute : Attribute
    {
        /// <summary>
        /// <see cref="Model.Markers.Marker"/> injection attribute.
        /// </summary>
        /// <param name="id">ID of the marker</param>
        public MarkerAttribute(string id)
        {
            this.Id = id;
        }

        /// <summary>
        /// ID of the marker
        /// </summary>
        public string Id { get; private set; }
    }
}
