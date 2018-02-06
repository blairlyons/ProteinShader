using MolecularMachines.Framework.Geometry;
using SimpleJSON;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MolecularMachines.Framework.Persistence.JsonMappers
{
    class VectorMapper
    {
        private FloatMapper floatMapper = new FloatMapper();

        public JSONNode ToJson(Vector vector)
        {
            return
                floatMapper.AsString(vector.X) + ";" +
                floatMapper.AsString(vector.Y) + ";" +
                floatMapper.AsString(vector.Z);
        }

        public Vector ToModel(JSONNode data)
        {
            var components = data.AsString.Split(';');
            return
                new Vector(
                    floatMapper.AsFloat(components[0]),
                    floatMapper.AsFloat(components[1]),
                    floatMapper.AsFloat(components[2])
                );
        }
    }
}
