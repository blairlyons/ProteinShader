using MolecularMachines.Framework.Model;
using SimpleJSON;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MolecularMachines.Framework.Persistence.JsonMappers
{
    class ColorMapper
    {
        private FloatMapper floatMapper = new FloatMapper();

        public JSONNode ToJson(Color color)
        {
            return
                floatMapper.AsString(color.R) + ";" +
                floatMapper.AsString(color.G) + ";" +
                floatMapper.AsString(color.B) + ";" +
                floatMapper.AsString(color.A);
        }

        public Color ToModel(JSONNode data)
        {
            var components = data.AsString.Split(';');
            return
                new Color(
                    floatMapper.AsFloat(components[0]),
                    floatMapper.AsFloat(components[1]),
                    floatMapper.AsFloat(components[2]),
                    floatMapper.AsFloat(components[3])
                );
        }
    }
}
