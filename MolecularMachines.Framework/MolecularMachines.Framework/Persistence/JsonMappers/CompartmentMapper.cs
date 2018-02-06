using MolecularMachines.Framework.Model.Compartments;
using SimpleJSON;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MolecularMachines.Framework.Persistence.JsonMappers
{
    class CompartmentMapper
    {
        private VectorMapper vectorMapper = new VectorMapper();

        public JSONNode ToJson(Compartment compartment)
        {
            var node = new JSONClass();

            node["id"] = compartment.Id;
            node["corner1"] = vectorMapper.ToJson(compartment.CornerMin);
            node["corner2"] = vectorMapper.ToJson(compartment.CornerMax);

            return node;
        }

        public Compartment ToModel(JSONNode node)
        {
            return new Compartment(
                node["id"].AsString,
                vectorMapper.ToModel(node["corner1"]),
                vectorMapper.ToModel(node["corner2"])
            );
        }
    }
}
