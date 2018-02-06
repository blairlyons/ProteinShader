using MolecularMachines.Framework.Geometry;
using SimpleJSON;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MolecularMachines.Framework.Persistence.JsonMappers
{
    class QuaternionMapper
    {
        private FloatMapper floatMapper = new FloatMapper();

        public JSONNode ToJson(Quaternion quaternion)
        {
            return
                floatMapper.AsString(quaternion.X) + ";" +
                floatMapper.AsString(quaternion.Y) + ";" +
                floatMapper.AsString(quaternion.Z) + ";" +
                floatMapper.AsString(quaternion.W);
        }

        public Quaternion ToModel(JSONNode data)
        {
            var components = data.AsString.Split(';');
            return
                new Quaternion(
                    floatMapper.AsFloat(components[0]),
                    floatMapper.AsFloat(components[1]),
                    floatMapper.AsFloat(components[2]),
                    floatMapper.AsFloat(components[3])
                );
        }
    }
}
