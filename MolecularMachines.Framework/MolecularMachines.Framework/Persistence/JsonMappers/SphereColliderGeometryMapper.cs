using MolecularMachines.Framework.Model.Colliders;
using SimpleJSON;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MolecularMachines.Framework.Persistence.JsonMappers
{
    class SphereColliderGeometryMapper
    {
        private VectorMapper vectorMapper = new VectorMapper();

        public JSONNode ToJson(SphereColliderGeometry sphereColliderGeometry)
        {
            var node = new JSONClass();

            node["center"] = vectorMapper.ToJson(sphereColliderGeometry.Center);
            node["radius"].AsFloat = sphereColliderGeometry.Radius;

            return node;
        }

        public SphereColliderGeometry ToModel(JSONNode node)
        {
            return new SphereColliderGeometry(
                vectorMapper.ToModel(node["center"]),
                node["radius"].AsFloat
            );
        }
    }
}
