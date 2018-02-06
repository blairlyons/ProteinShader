using MolecularMachines.Framework.Model;
using MolecularMachines.Framework.Model.ConcentrationControllers;
using SimpleJSON;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MolecularMachines.Framework.Persistence.JsonMappers
{
    class ConcentrationControllerMapper
    {
        public JSONNode ToJson(ConcentrationController concentrationController)
        {
            var node = new JSONClass();

            node["name"] = concentrationController.Name;
            node["entityFactoryId"] = concentrationController.EntityFactoryId;
            node["min"].AsInt = concentrationController.MinCount;
            node["max"].AsInt = concentrationController.MaxCount;

            return node;
        }

        public ConcentrationController ToModel(MMEnvironment environment, JSONNode node)
        {
            return new ConcentrationController(
                environment,
                node["entityFactoryId"].AsString,
                node["name"].AsString,
                node["min"].AsInt,
                node["max"].AsInt
            );
        }
    }
}
