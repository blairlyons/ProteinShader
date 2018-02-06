using MolecularMachines.Framework.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MolecularMachines.Framework.Model.Markers
{
    /// <summary>
    /// Holds definition for a <see cref="Marker"/>.
    /// </summary>
    public class MarkerClass
    {
        public MarkerClass(string id)
        {
            this.Id = id;
        }

        public string Id { get; private set; }

        public virtual Marker CreateInstance(Entity owner)
        {
            return new Marker(owner, this, owner.Structure.MarkerLocalLocRotById(owner, this.Id));
        }
    }
}
