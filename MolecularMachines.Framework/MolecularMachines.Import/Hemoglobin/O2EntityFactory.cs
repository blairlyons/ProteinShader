using MolecularMachines.Framework.Model.ConcentrationControllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MolecularMachines.Framework.Model;
using MolecularMachines.Framework.Model.Entities;
using MolecularMachines.Framework.Geometry;
using MolecularMachines.Framework.Model.ConcentrationControllers.Presents;

namespace MolecularMachines.Import.Hemoglobin
{
    [EntityFactoryId("hemoglobin.ef.freeO2")]
    public class O2EntityFactory : EntityFactory
    {
        public override void Create()
        {
            var entity = environment.AddEntity("o2", false);
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
            return (entity.Class.Id == "o2" && entity.BindingSites[0].IsFree);
        }
    }
}
