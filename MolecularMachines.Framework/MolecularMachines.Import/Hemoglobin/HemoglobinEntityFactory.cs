using MolecularMachines.Framework.Model.ConcentrationControllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MolecularMachines.Framework.Model.Entities;
using MolecularMachines.Framework.Model.ConcentrationControllers.Presents;

namespace MolecularMachines.Import.Hemoglobin
{
    [EntityFactoryId("hemoglobin.ef.hemoglobin")]
    public class HemoglobinEntityFactory : SimpleEntityFactory
    {
        public HemoglobinEntityFactory() : base("hemoglobin", "main")
        {
        }
    }
}
