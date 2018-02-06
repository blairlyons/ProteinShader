using MolecularMachines.Framework.Model;
using SimpleJSON;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MolecularMachines.Framework.Persistence.JsonMappers
{
    class LocRotMapper
    {
        private VectorMapper vectorMapper = new VectorMapper();
        private QuaternionMapper quaternionMapper = new QuaternionMapper();

        public JSONNode ToJson(LocRot locRot)
        {
            var node = new JSONClass();

            node["loc"] = vectorMapper.ToJson(locRot.Location);
            node["rot"] = quaternionMapper.ToJson(locRot.Rotation);

            return node;
        }

        public LocRot ToModel(JSONNode node)
        {
            return new LocRotStatic(
                vectorMapper.ToModel(node["loc"]),
                quaternionMapper.ToModel(node["rot"])
            );
        }
    }
}
