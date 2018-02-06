using MolecularMachines.Framework.Model.Markers;
using SimpleJSON;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MolecularMachines.Framework.Persistence.JsonMappers
{
    class MarkerClassMapper
    {
        private BindingSiteClassMapper bindingSiteClassMapper = new BindingSiteClassMapper();
        private SensorClassMapper sensorClassMapper = new SensorClassMapper();

        public JSONNode ToJson(MarkerClass markerClass)
        {
            var node = new JSONClass();

            node["id"] = markerClass.Id;

            if (markerClass.GetType().Equals(typeof(BindingSiteClass)))
            {
                var bindingSiteClass = (BindingSiteClass)markerClass;
                node["type"] = "bindingSite";

                bindingSiteClassMapper.ToJson(node, bindingSiteClass);
            }
            else if (markerClass.GetType().Equals(typeof(SensorClass)))
            {
                var sensorClass = (SensorClass)markerClass;
                node["type"] = "sensor";

                sensorClassMapper.ToJson(node, sensorClass);
            }
            else if (markerClass.GetType().Equals(typeof(MarkerClass)))
            {
                node["type"] = "marker";
            }
            else
            {
                throw new MappingExceptionUnknownSubtype(typeof(MarkerClass), markerClass.GetType());
            }


            return node;
        }

        public MarkerClass ToModel(JSONNode node)
        {
            var type = node["type"].AsString;
            var id = node["id"].AsString;

            MarkerClass result;

            if (type == "bindingSite")
            {
                result = bindingSiteClassMapper.ToModel(node, id);
            }
            else if (type == "sensor")
            {
                result = sensorClassMapper.ToModel(node, id);
            }
            else if (type == "marker")
            {
                result = new MarkerClass(id);
            }
            else
            {
                throw new MappingException("unknown marker type: " + type);
            }

            return result;
        }
    }
}
