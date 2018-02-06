using MolecularMachines.Framework.Geometry;
using MolecularMachines.Framework.Model.Compartments;
using MolecularMachines.Framework.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MolecularMachines.Framework.Model.ConcentrationControllers.Presents
{
    /// <summary>
    /// Simple <see cref="EntityFactory"/> that creates or removes random instances of a specific <see cref="EntityClass"/> ID in a specific <see cref="Compartment"/>.
    /// </summary>
    public abstract class SimpleEntityFactory : EntityFactory
    {
        /// <summary>
        /// New instance.
        /// </summary>
        /// <param name="entityClassId"><see cref="EntityClass"/> ID</param>
        /// <param name="compartmentId"><see cref="Compartment"/> ID</param>
        public SimpleEntityFactory(string entityClassId, string compartmentId)
        {
            this.entityClassId = entityClassId;
            this.compartmentId = compartmentId;
        }

        private EntityClass entityClass;
        private Compartment compartment;

        private string entityClassId;
        private string compartmentId;

        public override void Init(MMEnvironment environment)
        {
            base.Init(environment);

            this.entityClass = environment.EntityClassById(this.entityClassId);
            this.compartment = environment.CompartmentById(this.compartmentId);
        }

        public override void Create()
        {
            var entity = environment.AddEntity(this.entityClass, false);
            entity.Compound.Float(
                new LocRotStatic(
                    GeometryUtils.RandomVectorInsideCompartment(this.compartment),
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
            return (entity.Class == this.entityClass);// && (compartment.IsEntityInside(entity));
        }
    }
}
