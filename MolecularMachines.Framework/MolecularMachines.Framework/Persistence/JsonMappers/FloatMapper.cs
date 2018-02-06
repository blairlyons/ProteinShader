using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace MolecularMachines.Framework.Persistence.JsonMappers
{
    class FloatMapper
    {
        public string AsString(float f)
        {
            return f.ToString(CultureInfo.InvariantCulture);
        }

        public float AsFloat(string s)
        {
            return float.Parse(s, CultureInfo.InvariantCulture);
        }
    }
}
