using MolecularMachines.Framework.Model.Compartments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MolecularMachines.Framework.Geometry
{
    /// <summary>
    /// Static class that provides some methods that are related to geometry.
    /// </summary>
    public static class GeometryUtils
    {
        /// <summary>
        /// Converts an angle from degress to radiant.
        /// </summary>
        /// <param name="v">angle in degrees</param>
        /// <returns>angle in radiants</returns>
        public static float DegToRad(double v)
        {
            return (float)(v * Math.PI / 180.0);
        }

        private static Random random = new Random(0);

        /// <summary>
        /// Returns a random float between a minimum and a maximum value
        /// </summary>
        /// <param name="min">minimum value</param>
        /// <param name="max">maximum value</param>
        /// <returns>random value between min and max</returns>
        public static float RandomFloat(float min, float max)
        {
            var delta = max - min;

            return delta * (float)random.NextDouble() + min;
        }

        /// <summary>
        /// Returns a random vector that is located inside a cuboid defined by two corners.
        /// </summary>
        /// <param name="cornerMin">Position of the first corner of the cuboid</param>
        /// <param name="cornerMax">Position of the second corner of the cuboid</param>
        /// <returns>Random vector inside the cuboid</returns>
        public static Vector RandomVectorInsideBox(Vector cornerMin, Vector cornerMax)
        {
            return
                new Vector(
                    RandomFloat(cornerMin.X, cornerMax.X),
                    RandomFloat(cornerMin.Y, cornerMax.Y),
                    RandomFloat(cornerMin.Z, cornerMax.Z)
                );
        }

        /// <summary>
        /// Returns a random vector that is located inside a compartment.
        /// </summary>
        /// <param name="compartment">Compartment</param>
        /// <returns>Random vector inside the compartment</returns>
        public static Vector RandomVectorInsideCompartment(Compartment compartment)
        {
            return
                RandomVectorInsideBox(
                    compartment.CornerMin,
                    compartment.CornerMax
                );
        }


        /// <summary>
        /// Returns a random vector inside a sphere defined by a center and radius.
        /// The distance to the center uniformly distributed. As a result, the distribution of points will be denser at the center than at radius distance.
        /// </summary>
        /// <param name="center">Center of the sphere</param>
        /// <param name="radius">Radius of the sphere. Also the maximum distance of the result vector from the center</param>
        /// <returns>Random vector inside the defined sphere</returns>
        public static Vector RandomVectorInPropabilitySphere(Vector center, float radius)
        {
            var distance = RandomFloat(0, radius);
            var result = center + RandomQuaternion().Rotate(Vector.AxisY * distance);

            return result;
        }

        /// <summary>
        /// Mathematical constant pi as float (The ratio of a circle's circumference to its diameter).
        /// </summary>
        public static readonly float pi = (float)Math.PI;

        /// <summary>
        /// Returns a random rotation quaternion.
        /// </summary>
        /// <returns>Random rotation quaternion</returns>
        public static Quaternion RandomQuaternion()
        {
            return RandomQuaternion(2 * pi);
        }

        /// <summary>
        /// Returns a random rotation quaternion, where the yaw, pitch and roll angles are limited between 0 and a specified upper bound.
        /// </summary>
        /// <param name="maxYaw">upper bound for yaw</param>
        /// <param name="maxPitch">upper bound for pitch</param>
        /// <param name="maxRoll">upper bound for roll</param>
        /// <returns>Random rotation quaternion</returns>
        public static Quaternion RandomQuaternion(float maxYaw, float maxPitch, float maxRoll)
        {
            return Quaternion.FromYawPitchRoll(RandomFloat(0, maxYaw), RandomFloat(0, maxPitch), RandomFloat(0, maxRoll));
        }

        /// <summary>
        /// Returns a random rotation quaternion, where the yaw, pitch and roll angles are limited between 0 and a specified upper bound.
        /// </summary>
        /// <param name="maxYawPitchRoll">upper bound for yaw, pitch and roll respectively</param>
        /// <returns></returns>
        public static Quaternion RandomQuaternion(float maxYawPitchRoll)
        {
            return RandomQuaternion(maxYawPitchRoll, maxYawPitchRoll, maxYawPitchRoll);
        }
    }
}
