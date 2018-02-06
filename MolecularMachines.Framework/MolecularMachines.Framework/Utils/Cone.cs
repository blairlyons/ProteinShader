using MolecularMachines.Framework.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MolecularMachines.Framework.Utils
{
    /// <summary>
    /// Helper class, that can be used to check if a point is inside a 3D cone.
    /// </summary>
    class Cone
    {
        /// <summary>
        /// Creates a new cone
        /// </summary>
        /// <param name="t">coordinates of apex point of cone</param>
        /// <param name="b">coordinates of center of basement circle</param>
        public Cone(Vector apex, Vector basementCircleCenter, float aperture)
        {
            this.t = apex;
            this.b = basementCircleCenter;

            // pre calculations
            axisVect = dif(t, b);

            cosHalfApertue = (float)Math.Cos(aperture / 2f);

            magnAxisVect = magn(axisVect);
        }

        public Vector Apex { get { return t; } }
        public Vector BasementCircleCenter { get { return b; } }

        private Vector t;
        private Vector b;

        // Vector pointing from apex to circle-center point.
        private Vector axisVect;

        private float cosHalfApertue;

        private float magnAxisVect;

        // source: http://stackoverflow.com/questions/10768142/verify-if-point-is-inside-a-cone-in-3d-space

        /**
         * @param x coordinates of point to be tested 
         * @param t coordinates of apex point of cone
         * @param b coordinates of center of basement circle
         * @param aperture in radians
         */
        public bool IsLyingInCone(Vector x)
        {
            // Vector pointing to X point from apex
            Vector apexToXVect = dif(t, x);

            // X is lying in cone only if it's lying in 
            // infinite version of its cone -- that is, 
            // not limited by "round basement".
            // We'll use dotProd() to 
            // determine angle between apexToXVect and axis.
            bool isInInfiniteCone = dotProd(apexToXVect, axisVect)
                                       / magn(apexToXVect) / magnAxisVect
                                         >
                                       // We can safely compare cos() of angles 
                                       // between vectors instead of bare angles.
                                       cosHalfApertue;


            if (!isInInfiniteCone) return false;

            // X is contained in cone only if projection of apexToXVect to axis
            // is shorter than axis. 
            // We'll use dotProd() to figure projection length.
            bool isUnderRoundCap = dotProd(apexToXVect, axisVect)
                                      / magnAxisVect
                                        <
                                      magnAxisVect;
            return isUnderRoundCap;
        }

        private static float dotProd(Vector a, Vector b)
        {
            return a.X * b.X + a.Y * b.Y + a.Z * b.Z;
        }

        private static Vector dif(Vector a, Vector b)
        {
            return (new Vector(
                    a.X - b.X,
                    a.Y - b.Y,
                    a.Z - b.Z
            ));
        }

        private static float magn(Vector a)
        {
            return (float)(Math.Sqrt(a.X * a.X + a.Y * a.Y + a.Z * a.Z));
        }
    }
}
