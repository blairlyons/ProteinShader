using MolecularMachines.Framework.Model.Behaviors.Presents;
using MolecularMachines.Framework.Model.Entities;
using SimpleJSON;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace MolecularMachines.Framework.Persistence.JsonMappers
{
    class EntityClassMapper
    {
        private MarkerClassMapper markerClassMapper = new MarkerClassMapper();
        private ArrayMapper arrayMapper = new ArrayMapper();
        private EntityStructureClassMapper entityStructureClassMapper = new EntityStructureClassMapper();

        public JSONNode ToJson(EntityClass entityClass)
        {
            var node = new JSONClass();

            node["id"] = entityClass.Id;
            node["behavior"] = entityClass.BehaviorId;
            node["markers"] = arrayMapper.ToJson(entityClass.Markers, markerClassMapper.ToJson);
            node["structure"] = entityStructureClassMapper.ToJson(entityClass.StructureClass);

            return node;
        }

        public EntityClass ToModel(JSONNode node)
        {
            return new EntityClass(
                 node["id"].AsString,
                 entityStructureClassMapper.ToModel(node["structure"]),
                 node["behavior"].AsString,
                 arrayMapper.ToModel(node["markers"], markerClassMapper.ToModel)
             );
        }
    }
}
