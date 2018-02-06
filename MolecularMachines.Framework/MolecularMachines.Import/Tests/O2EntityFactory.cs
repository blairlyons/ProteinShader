using MolecularMachines.Framework.Model.ConcentrationControllers;
using MolecularMachines.Framework.Model.ConcentrationControllers.Presents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MolecularMachines.Import.Tests
{
    [EntityFactoryId("hemoglobin.ef.o2")]
    public class O2EntityFactory : SimpleEntityFactory
    {
        public O2EntityFactory() : base("o2", "main")
        {
        }
    }
}
