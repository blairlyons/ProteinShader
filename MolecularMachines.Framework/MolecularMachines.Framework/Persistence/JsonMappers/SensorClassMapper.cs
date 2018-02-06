using MolecularMachines.Framework.Model.Markers;
using SimpleJSON;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MolecularMachines.Framework.Persistence.JsonMappers
{
    class SensorClassMapper
    {
        public void ToJson(JSONNode node, SensorClass sensorClass)
        {
            node["id"] = sensorClass.Id;
            node["range"].AsFloat = sensorClass.Range;
            node["aperture"].AsFloat = sensorClass.ApertureAngle;
        }

        public SensorClass ToModel(JSONNode node, string id)
        {
            return new SensorClass(
                id,
                node["range"].AsFloat,
                node["aperture"].AsFloat
            );

        }
    }
}
