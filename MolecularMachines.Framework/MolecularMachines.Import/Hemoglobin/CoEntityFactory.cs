using MolecularMachines.Framework.Geometry;
using MolecularMachines.Framework.Model;
using MolecularMachines.Framework.Model.ConcentrationControllers;
using MolecularMachines.Framework.Model.ConcentrationControllers.Presents;
using MolecularMachines.Framework.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MolecularMachines.Import.Hemoglobin
{
    [EntityFactoryId("hemoglobin.ef.freeCo")]
    public class CoEntityFactory : EntityFactory
    {
        public override void Create()
        {
            var entity = environment.AddEntity("co", false);
            entity.Compound.Float(
                new LocRotStatic(
                    GeometryUtils.RandomVectorInsideCompartment(environment.Compartments.First()),
                    GeometryUtils.RandomQuaternion()
                )
            );
        }

        public override void Delete(Entity entity)
        {
            entity.Dispose();
        }

        public override bool Filter(Entity entity)
        {
            return (entity.Class.Id == "co" && entity.BindingSites[0].IsFree);
        }
    }
}
