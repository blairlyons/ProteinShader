using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MolecularMachines.Framework.Model
{
    /// <summary>
    /// Color
    /// </summary>
    public struct Color
    {
        public Color(float r, float g, float b, float a)
        {
            this.R = r;
            this.G = g;
            this.B = b;
            this.A = a;
        }

        public Color(float r, float g, float b) : this(r, g, b, 1.0f)
        { }

        public float R { get; private set; }
        public float G { get; private set; }
        public float B { get; private set; }
        public float A { get; private set; }

        public static Color Random
        {
            get
            {
                return new Color(
                    (float)random.NextDouble(),
                    (float)random.NextDouble(),
                    (float)random.NextDouble(),
                    1.0f
                );
            }
        }

        private static Random random = new System.Random(0);

        public static Color Lerp(Color color1, Color color2, float p)
        {
            var q = 1 - p;

            return new Color(
                color2.R * p + color1.R * q,
                color2.G * p + color1.G * q,
                color2.B * p + color1.B * q,
                color2.A * p + color1.A * q
            );
        }

        public override string ToString()
        {
            return R.ToString("0.00") + " " + G.ToString("0.00") + " " + B.ToString("0.00") + " a:" + A.ToString("0.00");
        }
    }
}
