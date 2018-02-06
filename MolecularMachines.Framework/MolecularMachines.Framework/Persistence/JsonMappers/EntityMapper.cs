using MolecularMachines.Framework.Model;
using MolecularMachines.Framework.Model.Compounds;
using MolecularMachines.Framework.Model.Entities;
using MolecularMachines.Framework.Model.SpatialLinks;
using SimpleJSON;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MolecularMachines.Framework.Persistence.JsonMappers
{
    class EntityMapper
    {
        private LocRotMapper locRotMapper = new LocRotMapper();

        public JSONNode ToJson(Entity entity, IdItemStore<Entity> entityIdStore)
        {
            var node = new JSONClass();

            node["id"] = entityIdStore.Add(entity);
            node["class"] = entity.Class.Id;

            if (entity.Compound.RootEntity == entity)
            {
                var spatialLinkNode = SpatialLinkToJson(entity.Compound.SpatialLink);
                if (spatialLinkNode != null)
                {
                    node["spatialLink"] = spatialLinkNode;
                }
            }

            return node;
        }

        private JSONNode SpatialLinkToJson(SpatialLink spatialLink)
        {
            var node = new JSONClass();

            switch (spatialLink.Type)
            {
                case SpatialLinkType.Fixed:

                    node["type"] = "fixed";
                    node["locRot"] = locRotMapper.ToJson(spatialLink.LocRot);

                    break;
                case SpatialLinkType.Floating:

                    node["type"] = "floating";
                    node["locRot"] = locRotMapper.ToJson(spatialLink.LocRot);

                    break;
                default:
                    // do nothing
                    return null;
            }

            return node;
        }

        public Entity ToModel(MMEnvironment environment, IdItemStore<Entity> entityIdStore, JSONNode node)
        {
            var entityClassId = node["class"].AsString;
            var entity = environment.AddEntity(entityClassId, false);
            entityIdStore.Add(entity, node["id"].AsString);

            if (node.Contains("spatialLink"))
            {
                var spatialLinkNode = node["spatialLink"].AsObject;
                SpatialLinkToModel(entity.Compound, spatialLinkNode);
            }

            return entity;
        }

        private void SpatialLinkToModel(Compound compound, JSONClass spatialLinkNode)
        {
            var type = spatialLinkNode["type"].AsString;

            if (type == "fixed")
            {
                var locRot = locRotMapper.ToModel(spatialLinkNode["locRot"]);
                compound.Fix(locRot);
            }
            else if (type == "floating")
            {
                var locRot = locRotMapper.ToModel(spatialLinkNode["locRot"]);
                compound.Float(locRot);
            }
            else
            {
                throw new MappingException("unknown spatialLink type: " + type);
            }
        }
    }
}
