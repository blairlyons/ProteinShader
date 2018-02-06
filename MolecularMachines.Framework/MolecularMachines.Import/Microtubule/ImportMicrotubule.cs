using MolecularMachines.Framework.Geometry;
using MolecularMachines.Framework.Model;
using MolecularMachines.Framework.Model.Builders;
using MolecularMachines.Framework.Model.Compartments;
using MolecularMachines.Framework.Model.Entities;
using MolecularMachines.Framework.Model.Markers;
using MolecularMachines.Import;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MolecularMachines.Import.Microtubule
{
    class ImportMicrotubule : ImportBase
    {
        private EntityClassBuilder tubulin = new EntityClassBuilder("tubulin");
        private PdbXReader.Model.Entry entry;
        private Compartment compartment;

        public override void Import()
        {
            var pdbsPath = this.RootPath;

            this.Environment.Name = "Microtubule";

            this.compartment = new Compartment("main", new Vector(-250, -250, -250), new Vector(250, 250, 250));
            this.Environment.AddCompartment(compartment);

            this.entry = LoadCif(@"1tub.cif");

            ImportTubulin();

            //
            // Entities
            //

            var tubulinClass = this.tubulin.Create();
            this.Environment.AddEntityClass(tubulinClass);

            /*
            Entity lastTubuli = null;
            for (int i = 0; i < 200; i++)
            {
                var t = this.Environment.AddEntity(tubulinClass);
                if (i == 0)
                {
                    t.Compound.Fix(LocRot.Zero);
                }

                if (lastTubuli != null)
                {
                    lastTubuli.BindingSiteById("next").InstantBind(t.BindingSiteById("previous"));
                }

                lastTubuli = t;
            }
            */

            this.Environment.AddConcentrationController(new Framework.Model.ConcentrationControllers.ConcentrationController(this.Environment, "microtubule.ef.tubulin", "Tubulin", 0, 500));
        }

        private void ImportTubulin()
        {
            tubulin.BehaviorId = "microtubule.tubulin";
            tubulin.AddAtoms(entry.AtomSites, atomSite => atomSite.Symbol, atomSite => new Vector(atomSite.X, atomSite.Y, atomSite.Z));
            tubulin.ReCenter();

            tubulin.BindingSite(
                "previous",
                new LocRotStatic(
                    new Vector(-40, 0, 0),
                    Quaternion.Identity
                )
            );

            var pi = GeometryUtils.pi;

            var nextLocRot = new LocRotStatic(
                    new Vector(40, 0, 0),
                    Quaternion.CombineRotation(BindingSite.BindingQuaternion, Quaternion.FromYawPitchRoll(2 * pi / 66, 0, 2 * pi / 14))
                );

            tubulin.BindingSite(
                "next",
                nextLocRot
            );
            tubulin.Sensor(
                "sensor",
                80,
                2,
                nextLocRot
            );

        }
    }
}
