using MolecularMachines.Framework.Model;
using MolecularMachines.Framework.Model.Entities;
using MolecularMachines.Framework.Persistence.JsonMappers;
using SimpleJSON;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MolecularMachines.Framework.Persistence
{
    class EnvironmentMapper
    {
        private ArrayMapper arrayMapper = new ArrayMapper();
        private CompartmentMapper compartmentMapper = new CompartmentMapper();
        private EntityMapper entityMapper = new EntityMapper();
        private IdItemStore<Entity> entityIdStore = new IdItemStore<Entity>();
        private BondMapper bondMapper = new BondMapper();
        private ConcentrationControllerMapper concentrationControllerMapper = new ConcentrationControllerMapper();

        public JSONNode ToJson(MMEnvironment environment)
        {
            var node = new JSONClass();

            node["name"] = environment.Name + "";
            node["compartments"] = arrayMapper.ToJson(environment.Compartments, compartmentMapper.ToJson);
            node["entities"] = arrayMapper.ToJson(environment.Entities, MapEntityToJson);
            node["bonds"] = arrayMapper.ToJson(BondsToJson(environment), n => n);
            node["concentrationControllers"] = arrayMapper.ToJson(environment.ConcentrationControllers, concentrationControllerMapper.ToJson);

            return node;
        }

        private JSONNode MapEntityToJson(Entity entity)
        {
            return entityMapper.ToJson(entity, this.entityIdStore);
        }

        private IEnumerable<JSONNode> EntityBondsToJson(Entity entity)
        {
            // iterate all bindingsSites to find bonds
            foreach (var bindingSite in entity.BindingSites)
            {
                if (bondMapper.IsSaveWorthy(bindingSite))
                {
                    yield return bondMapper.ToJson(bindingSite, this.entityIdStore);
                }
            }
        }

        private IEnumerable<JSONNode> BondsToJson(MMEnvironment environment)
        {
            foreach (var compound in environment.Compounds)
            {
                if (!compound.IsDisposed)
                {
                    // save all bonds of the entities, except the root entity
                    foreach (var entity in compound.Entities)
                    {
                        if (entity != compound.RootEntity)
                        {
                            foreach (var jsonNode in EntityBondsToJson(entity))
                            {
                                yield return jsonNode;
                            }
                        }
                    }

                    // save rootEntity at last
                    // so after restore, it rules the compound again as RootEntity
                    foreach (var jsonNode in EntityBondsToJson(compound.RootEntity))
                    {
                        yield return jsonNode;
                    }
                }
            }
        }

        public void ToModel(MMEnvironment environment, JSONNode node)
        {
            environment.Name = node["name"].AsString;

            // Compartments
            var compartments = arrayMapper.ToModel(node["compartments"], compartmentMapper.ToModel);
            foreach (var compartment in compartments)
            {
                environment.AddCompartment(compartment);
            }

            // Entities
            foreach (JSONNode entityNode in node["entities"].AsArray)
            {
                // Entities are added to environment in entityMapper.ToModel
                entityMapper.ToModel(environment, this.entityIdStore, entityNode);
            }

            // Bonds
            foreach (JSONNode bondNode in node["bonds"].AsArray)
            {
                // Bonds are applied in bondMapper.ToModel
                bondMapper.ToModel(bondNode, this.entityIdStore);
            }

            // ConcentrationControllers
            foreach (JSONNode concentrationControllerNode in node["concentrationControllers"].AsArray)
            {
                var concentrationController = concentrationControllerMapper.ToModel(environment, concentrationControllerNode);
                environment.AddConcentrationController(concentrationController);
            }
        }
    }
}
