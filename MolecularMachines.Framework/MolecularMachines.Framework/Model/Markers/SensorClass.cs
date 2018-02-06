using MolecularMachines.Framework.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MolecularMachines.Framework.Model.Markers
{
    /// <summary>
    /// Holds the definition of a <see cref="Sensor"/>.
    /// </summary>
    public class SensorClass : MarkerClass
    {
        public SensorClass(string id, float range, float apertureAngle) : base(id)
        {
            this.Range = range;
            this.ApertureAngle = apertureAngle;
        }

        public float Range { get; private set; }
        public float ApertureAngle { get; private set; }

        public override Marker CreateInstance(Entity owner)
        {
            return new Sensor(owner, this, owner.Structure.MarkerLocalLocRotById(owner, this.Id));
        }
    }
}
