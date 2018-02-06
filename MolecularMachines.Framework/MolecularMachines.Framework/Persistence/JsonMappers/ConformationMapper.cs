using MolecularMachines.Framework.Model.Entities;
using SimpleJSON;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MolecularMachines.Framework.Persistence.JsonMappers
{
    class ConformationMapper
    {
        private ArrayMapper arrayMapper = new ArrayMapper();
        private VectorMapper vectorMapper = new VectorMapper();
        private LocRotMapper locRotMapper = new LocRotMapper();
        private ColliderMapper colliderMapper = new ColliderMapper();
        private ColorMapper colorMapper = new ColorMapper();

        public JSONNode ToJson(Conformation conformation)
        {
            var node = new JSONClass();

            node["id"] = conformation.Id;
            node["color"] = colorMapper.ToJson(conformation.Color);
            node["atomPositions"] = arrayMapper.ToJson(conformation.AtomPositions, vectorMapper.ToJson);
            node["markerLocRots"] = arrayMapper.ToJson(conformation.MarkerLocRots, locRotMapper.ToJson);
            node["collider"] = colliderMapper.ToJson(conformation.Collider);

            return node;
        }

        public Conformation ToModel(JSONNode node)
        {
            return new Conformation(
                node["id"].AsString,
                arrayMapper.ToModel(node["atomPositions"], vectorMapper.ToModel),
                arrayMapper.ToModel(node["markerLocRots"], locRotMapper.ToModel),
                colliderMapper.ToModel(node["collider"]),
                colorMapper.ToModel(node["color"])
            );
        }
    }
}
