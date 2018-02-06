using MolecularMachines.Framework.Model.Colliders;
using SimpleJSON;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MolecularMachines.Framework.Persistence.JsonMappers
{
    class ColliderGeometryMapper
    {
        private SphereColliderGeometryMapper sphereColliderGeometryMapper = new SphereColliderGeometryMapper();

        public JSONNode ToJson(ColliderGeometry colliderGeometry)
        {
            var type = colliderGeometry.GetType();

            JSONNode node;

            if (type.Equals(typeof(SphereColliderGeometry)))
            {
                node = sphereColliderGeometryMapper.ToJson((SphereColliderGeometry)colliderGeometry);
                node["type"] = "sphere";
            }
            else
            {
                throw new MappingExceptionUnknownSubtype(typeof(ColliderGeometry), type);
            }

            return node;
        }

        public ColliderGeometry ToModel(JSONNode node)
        {
            var type = node["type"].AsString;

            ColliderGeometry result;

            if (type == "sphere")
            {
                result = sphereColliderGeometryMapper.ToModel(node);
            }
            else
            {
                throw new MappingException("unknown collider type: " + type);
            }

            return result;
        }
    }
}
