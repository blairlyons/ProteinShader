using MolecularMachines.Framework.DataStructures;
using MolecularMachines.Framework.Geometry;
using MolecularMachines.Framework.Model.Compartments;
using MolecularMachines.Framework.Model.Compounds;
using MolecularMachines.Framework.Model.ConcentrationControllers;
using MolecularMachines.Framework.Model.Entities;
using MolecularMachines.Framework.Model.SpatialLinks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MolecularMachines.Framework.Model
{
    public class MMEnvironment
    {
        public MMEnvironment()
        {
            Reset();
        }

        public string Name { get; set; }

        //
        // EntityClasses
        //

        private List<EntityClass> entityClasses = new List<EntityClass>();
        public IEnumerable<EntityClass> EntityClasses { get { return this.entityClasses; } }

        public void AddEntityClass(EntityClass entityClass)
        {
            this.entityClasses.Add(entityClass);

            if (entityClass.StructureClass.StructureSharing)
            {
                // create a shared EntityStructure now, that is used by
                // all other Entities.
                // If an Entity is added, no new EntityStructure-Instance is created

                var structure = entityClass.StructureClass.CreateInstance();
                AddStructureShared(structure);
            }
        }

        public EntityClass EntityClassById(string id)
        {
            var result = EntityClassByIdOrNull(id);
            if (result == null)
            {
                throw new Exception($"unknown entity class: {id}");
            }

            return result;
        }

        public EntityClass EntityClassByIdOrNull(string id)
        {
            return
                (
                    from e in entityClasses
                    where e.Id == id
                    select e
                ).FirstOrDefault();
        }

        private void ResetEntityClasses()
        {
            this.entityClasses.Clear();
        }

        //
        // Entities 
        //

        private IterateList<Entity> entities = new IterateList<Entity>();
        public IEnumerable<Entity> Entities { get { return this.entities; } }

        public Entity AddEntity(string entityClassId, bool setCompoundFloat = true)
        {
            var entityClass = EntityClassById(entityClassId);
            return AddEntity(entityClass, setCompoundFloat);
        }

        public Entity AddEntity(EntityClass entityClass, bool setCompoundFloat = true)
        {
            var entity = OnCreateEntity(entityClass);

            this.entities.Add(entity);

            if (entityClass.StructureClass.StructureSharing)
            {
                // nothing to do here, becaused shared structures are already
                // created and added in AddEntityClass.
            }
            else
            {
                // add individual structure
                AddStructureIndividual(entity.Structure);
            }

            var compound = CreateCompound(entity);
            if (setCompoundFloat)
            {
                compound.Float(LocRot.Zero);
            }

            return entity;
        }

        protected virtual Entity OnCreateEntity(EntityClass entityClass)
        {
            return new Entity(this, entityClass);
        }

        private void UpdateEntities()
        {
            using (var enumerator = this.entities.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    var entity = enumerator.Current;

                    if (entity.IsDisposed)
                    {
                        enumerator.Remove();
                        OnRemoveEntity(entity);
                    }
                    else
                    {
                        entity.Update();
                    }
                }
            }
        }

        private void ResetEntities()
        {
            using (var enumerator = this.entities.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    var entity = enumerator.Current;

                    entity.Dispose();
                    enumerator.Remove();
                    OnRemoveEntity(entity);
                }
            }
        }

        protected virtual void OnRemoveEntity(Entity entity)
        {
            if (entity.Class.StructureClass.StructureSharing)
            {
                // nothing to do here, becaused shared structures is
                // handled by EntityClass
            }
            else
            {
                // remove individual structure
                OnRemoveStructureIndividual(entity, entity.Structure);
            }
        }

        //
        // EntityStructures
        //

        private IterateList<EntityStructure> entityStructuresShared = new IterateList<EntityStructure>();
        private IterateList<EntityStructure> entityStructuresIndividual = new IterateList<EntityStructure>();
        public IEnumerable<EntityStructure> EntityStructures { get { return this.entityStructuresShared.Concat(this.entityStructuresIndividual); } }

        private void AddStructureIndividual(EntityStructure structure)
        {
            OnAddStructureIndividual(structure);
        }

        private void AddStructureShared(EntityStructure structure)
        {
            OnAddStructureShared(structure);
        }

        protected virtual void OnAddStructureShared(EntityStructure structure)
        {
            this.entityStructuresShared.Add(structure);
        }

        protected virtual void OnAddStructureIndividual(EntityStructure structure)
        {
            this.entityStructuresIndividual.Add(structure);
        }

        private void OnRemoveStructureIndividual(Entity entity, EntityStructure structure)
        {
            this.entityStructuresIndividual.Remove(structure);
        }

        private void UpdateEntityStructures()
        {
            foreach (var structure in this.EntityStructures)
            {
                structure.Update();
            }
        }

        private void ResetEntityStructures()
        {
            this.entityStructuresIndividual.Clear();
            this.entityStructuresShared.Clear();
        }

        //
        // Compunds
        //

        private IterateList<Compound> compounds = new IterateList<Compound>();
        public IEnumerable<Compound> Compounds { get { return this.compounds; } }

        public Compound CreateCompound(Entity rootEntity)
        {
            var compound = OnCreateCompound(rootEntity);
            this.compounds.Add(compound);
            compound.Start();

            return compound;
        }

        protected virtual Compound OnCreateCompound(Entity rootEntity)
        {
            return new Compound(this, rootEntity);
        }

        private void UpdateCompounds()
        {
            using (var enumerator = this.compounds.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    var compound = enumerator.Current;

                    if (compound.IsDisposed)
                    {
                        enumerator.Remove();
                    }
                    else
                    {
                        compound.Update();
                    }
                }
            }
        }

        private void ResetCompounds()
        {
            using (var enumerator = this.compounds.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    var compound = enumerator.Current;

                    compound.Dispose();
                    enumerator.Remove();
                }
            }
        }

        //
        // Compartments
        //

        private IterateList<Compartment> compartments = new IterateList<Compartment>();
        public IEnumerable<Compartment> Compartments { get { return this.compartments; } }

        public void AddCompartment(Compartment compartment)
        {
            this.compartments.Add(compartment);
        }

        public Compartment CompartmentByPoint(Vector p)
        {
            return
                (
                    from c in compartments
                    where c.IsPointInside(p)
                    select c
                ).FirstOrDefault();
        }

        public Compartment NearestCompartment(Vector p)
        {
            Compartment nearest = null;
            float nearestDistance = float.MaxValue;
            foreach (var c in this.compartments)
            {
                var d = c.Distance(p);
                if (d < nearestDistance)
                {
                    nearestDistance = d;
                    nearest = c;
                }
            }

            return nearest;
        }

        public Compartment CompartmentById(string id)
        {
            return
                (
                    from c in compartments
                    where c.Id == id
                    select c
                ).First();
        }

        private void ResetCompartments()
        {
            this.compounds.Clear();
        }

        //
        // Concentration Controllers
        //
        private List<ConcentrationController> concentrationControllers = new List<ConcentrationController>();
        public IEnumerable<ConcentrationController> ConcentrationControllers { get { return this.concentrationControllers; } }

        public void AddConcentrationController(ConcentrationController concentrationController)
        {
            this.concentrationControllers.Add(concentrationController);
        }

        private void ResetConcentrationControllers()
        {
            this.concentrationControllers.Clear();
        }

        private void UpdateConcentrationControllers()
        {
            foreach (var concentrationController in this.concentrationControllers)
            {
                concentrationController.Update();
            }
        }

        //
        // General Stuff
        //

        public void Reset()
        {
            this.Name = "untitled environment";

            ResetEntities();
            ResetEntityStructures();
            ResetCompounds();
            ResetEntityClasses();
            ResetCompartments();
            ResetConcentrationControllers();

            this.OnReset();
        }

        protected virtual void OnReset()
        {

        }

        public void Update()
        {
            UpdateConcentrationControllers();

            UpdateEntityStructures();
            UpdateEntities();
            UpdateCompounds();

            OnUpdate();
        }

        protected virtual void OnUpdate()
        {

        }

        public string GetDiagnosticString()
        {
            var s = new StringBuilder();
            foreach (var compound in this.compounds)
            {
                s.AppendLine("Compound " + compound.SpatialLink?.GetType().Name);
                foreach (var entity in compound.Entities)
                {
                    s.AppendLine("  " + entity.ToString());
                }
            }

            return s.ToString();
        }
    }
}
