using MolecularMachines.Framework.Model.ConcentrationControllers;
using MolecularMachines.Framework.Model.ConcentrationControllers.Presents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MolecularMachines.Import.Microtubule
{
    [EntityFactoryId("microtubule.ef.tubulin")]
    public class TubulinEntityFactory : SimpleEntityFactory
    {
        public TubulinEntityFactory() : base("tubulin", "main")
        {
        }
    }
}
