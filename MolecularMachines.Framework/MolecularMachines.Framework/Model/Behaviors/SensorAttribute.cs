using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MolecularMachines.Framework.Model.Behaviors
{
    /// <summary>
    /// Attribute that can be used on properties in <see cref="EntityBehavior"/> classes to inject a <see cref="Model.Markers.Sensor"/> object.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class SensorAttribute : Attribute
    {
        /// <summary>
        /// <see cref="Model.Markers.Sensor"/> injection attribute.
        /// </summary>
        /// <param name="id">ID of the sensor</param>
        public SensorAttribute(string id)
        {
            this.Id = id;
        }

        /// <summary>
        /// ID of the sensor
        /// </summary>
        public string Id { get; private set; }
    }
}
