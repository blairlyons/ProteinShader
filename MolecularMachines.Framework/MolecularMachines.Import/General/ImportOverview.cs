using MolecularMachines.Framework.Geometry;
using MolecularMachines.Framework.Model;
using MolecularMachines.Framework.Model.Builders;
using MolecularMachines.Import.Utils;
using PdbXReader.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace MolecularMachines.Import.General
{
    class ImportOverview : ImportBase
    {
        public override void Import()
        {
            this.builders = new Dictionary<string, EntityClassBuilder>();

            this.Environment.Name = "Overview: " + Path.GetFileName(this.RootPath);

            var cifFiles = Directory.GetFiles(this.RootPath, "*.cif");

            this.Environment.AddCompartment(new Framework.Model.Compartments.Compartment("default", new Vector(-500, -500, -500), new Vector(500, 500, 500)));

            for (int i = 0; i < cifFiles.Length; i++)
            {
                var cifFile = cifFiles[i];
                ImportConformation(cifFile, "c"+i.ToString());
            }

            foreach (var builder in builders.Values)
            {
                // cut down Atom Count if count is not equal
                int minimumCount = (from c in builder.Conformations select c.Atoms.Count).Min();
                foreach (var conformation in builder.Conformations)
                {
                    int initialCount = conformation.Atoms.Count;
                    if (initialCount > minimumCount)
                    {
                        Console.WriteLine(conformation.Id + ": cutting down from " + initialCount + " to " + minimumCount + " (" + (minimumCount - initialCount) + ")");
                    }

                    while (conformation.Atoms.Count > minimumCount)
                    {
                        conformation.Atoms.RemoveAt(conformation.Atoms.Count - 1);
                    }
                }

                var entityClass = builder.Create();
                this.Environment.AddEntityClass(entityClass);
                var entity = this.Environment.AddEntity(entityClass, false);
                entity.Compound.Fix(LocRot.Zero);
            }
        }

        private Dictionary<string, EntityClassBuilder> builders;

        private EntityClassBuilder GetBuilderById(string id)
        {
            EntityClassBuilder result;

            if (!this.builders.TryGetValue(id, out result))
            {
                result = new EntityClassBuilder(id);
                this.builders.Add(id, result);
            }

            return result;
        }

        private Vector? Center = null;

        public void ImportConformation(string cifFile, string conformationId)
        {
            var entry = ImportUtils.ReadPdbXCif(cifFile);

            Console.WriteLine(cifFile + "/" + conformationId);

            if (this.Center == null)
            {
                this.Center = ImportUtils.CalcCenter(entry.AtomSites.Select(atomSite => new Vector(atomSite.X, atomSite.Y, atomSite.Z)));
            }

            foreach (var item in entry.AsymetricUnits)
            {
                var id = item.Id;
                var builder = GetBuilderById(id);
                builder.BehaviorId = "test.overview";
                var atomSites = (from atomSite in entry.AtomSites where atomSite.AsymmetricUnit == item select atomSite);

                builder.SetConformation(conformationId);

                builder.AddAtoms(
                    atomSites,
                    (atomSite) => atomSite.Symbol,
                    (atomSite) => new Vector(atomSite.X, atomSite.Y, atomSite.Z) - this.Center.Value
                );
            }
        }
    }
}
