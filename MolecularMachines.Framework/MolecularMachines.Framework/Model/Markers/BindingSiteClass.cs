using MolecularMachines.Framework.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MolecularMachines.Framework.Model.Markers
{
    /// <summary>
    /// Holds the definition of a <see cref="BindingSite"/>.
    /// </summary>
    public class BindingSiteClass : MarkerClass
    {
        public BindingSiteClass(string id) : base(id)
        { }

        public override Marker CreateInstance(Entity owner)
        {
            return new BindingSite(owner, this, owner.Structure.MarkerLocalLocRotById(owner, this.Id));
        }
    }
}
