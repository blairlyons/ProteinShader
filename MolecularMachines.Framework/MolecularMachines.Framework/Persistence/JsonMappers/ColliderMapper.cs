using MolecularMachines.Framework.Model.Colliders;
using SimpleJSON;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MolecularMachines.Framework.Persistence.JsonMappers
{
    class ColliderMapper
    {
        private ColliderGeometryMapper colliderGeometryMapper = new ColliderGeometryMapper();
        private ArrayMapper arrayMapper = new ArrayMapper();

        public JSONNode ToJson(Collider collider)
        {
            var node = new JSONClass();

            node["geometries"] = arrayMapper.ToJson(collider, colliderGeometryMapper.ToJson);

            return node;
        }

        public Collider ToModel(JSONNode node)
        {
            return new Collider(
                arrayMapper.ToModel(node["geometries"], colliderGeometryMapper.ToModel).ToArray()
            );
        }
    }
}
