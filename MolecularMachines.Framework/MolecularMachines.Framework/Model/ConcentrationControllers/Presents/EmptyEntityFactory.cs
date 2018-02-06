using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MolecularMachines.Framework.Model.Entities;

namespace MolecularMachines.Framework.Model.ConcentrationControllers.Presents
{
    /// <summary>
    /// <see cref="EntityFactory"/> that does nothing.
    /// </summary>
    [EntityFactoryId("present.ef.empty")]
    public class EmptyEntityFactory : EntityFactory
    {
        public override void Create()
        {
            
        }

        public override void Delete(Entity entity)
        {
            
        }

        public override bool Filter(Entity entity)
        {
            return false;
        }
    }
}
