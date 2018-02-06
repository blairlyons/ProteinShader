using MolecularMachines.Framework.Geometry;
using MolecularMachines.Framework.Model.Builders;
using MolecularMachines.Framework.Model.Compartments;
using MolecularMachines.Import;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MolecularMachines.AtpSynthase.Import.General
{
    public class ImportExample : ImportBase
    {
        public override void Import()
        {
            var compartment = new Compartment(
                "main", 
                new Vector(-250, -250, -250), 
                new Vector(250, 250, 250)
            );
            this.Environment.AddCompartment(compartment);

            var hemoglobinClass = new EntityClassBuilder("hemoglobin");
            hemoglobinClass.AddAtoms(LoadCif("2hhb.cif"));
            hemoglobinClass.ReCenter();

            for (int i = 0; i < 100; i++)
            {
                CreateEntityFloating(hemoglobinClass, compartment);
            }
        }
    }
}
