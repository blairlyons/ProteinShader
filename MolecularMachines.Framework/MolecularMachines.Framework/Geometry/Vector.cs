using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MolecularMachines.Framework.Geometry
{
    /// <summary>
    /// 3D Vector.
    /// </summary>
    public struct Vector
    {
        public Vector(float x, float y, float z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }

        public float X { get; private set; }
        public float Y { get; private set; }
        public float Z { get; private set; }

        public override string ToString()
        {
            return "[" + X.ToString("0.0") + " ; " + Y.ToString("0.0") + " ; " + Z.ToString("0.0") + "]";
        }

        public override int GetHashCode()
        {
            return X.GetHashCode() + 7 * Y.GetHashCode() + 11 * Z.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj == null) { return false; }
            if (!(obj is Vector)) { return false; }
            var other = (Vector)obj;
            return (this == other);
        }

        /// <summary>
        /// Interpolation of two vectors
        /// </summary>
        /// <param name="a">start vector</param>
        /// <param name="b">end vector</param>
        /// <param name="p">progress of the interpolation. Number between 0 and 1.</param>
        /// <returns></returns>
        public static Vector Lerp(Vector a, Vector b, float p)
        {
            return a + ((b - a) * p);
        }

        /// <summary>
        /// (0,0,0)
        /// </summary>
        public static Vector Zero
        {
            get { return new Vector(0, 0, 0); }
        }

        /// <summary>
        /// (1,0,0)
        /// </summary>
        public static Vector AxisX
        {
            get { return new Vector(1, 0, 0); }
        }

        /// <summary>
        /// (0,1,0)
        /// </summary>
        public static Vector AxisY
        {
            get { return new Vector(0, 1, 0); }
        }

        /// <summary>
        /// (0,0,1)
        /// </summary>
        public static Vector AxisZ
        {
            get { return new Vector(0, 0, 1); }
        }

        /// <summary>
        /// Euclidean distance between two vectors.
        /// </summary>
        /// <param name="center"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        public static float Distance(Vector a, Vector b)
        {
            var dx = a.X - b.X;
            var dy = a.Y - b.Y;
            var dz = a.Z - b.Z;

            return (float)Math.Sqrt(
                dx * dx +
                dy * dy +
                dz * dz
            );
        }

        /// <summary>
        /// Length of the vector.
        /// </summary>
        public float Length
        {
            get
            {
                return (float)Math.Sqrt(
                    this.X * this.X +
                    this.Y * this.Y +
                    this.Z * this.Z
                );
            }
        }

        /// <summary>
        /// Normalized vector. The resulting vector has a length of 1
        /// </summary>
        public Vector Normalized
        {
            get
            {
                return this / Length;
            }
        }

        public static bool operator ==(Vector a, Vector b)
        {
            return (a.X == b.X) && (a.Y == b.Y) && (a.Z == b.Z);
        }
        public static bool operator !=(Vector a, Vector b)
        {
            return !(a == b);
        }
        public static Vector operator *(Vector a, float b)
        {
            return new Vector(a.X * b, a.Y * b, a.Z * b);
        }
        public static Vector operator *(float b, Vector a)
        {
            return a * b;
        }
        public static Vector operator /(Vector a, float b)
        {
            return new Vector(a.X / b, a.Y / b, a.Z / b);
        }
        public static Vector operator +(Vector a, Vector b)
        {
            return new Vector(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        }
        public static Vector operator -(Vector a, Vector b)
        {
            return new Vector(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
        }

        /// <summary>
        /// Dot product or scalar product of two vectors
        /// </summary>
        /// <param name="vector1">first vector</param>
        /// <param name="vector2">second vector</param>
        /// <returns>Dot product</returns>
        public static float Dot(Vector vector1, Vector vector2)
        {
            return vector1.X * vector2.X + vector1.Y * vector2.Y + vector1.Z * vector2.Z;
        }

        /// <summary>
        /// Cross product or vector product of two vectors
        /// </summary>
        /// <param name="vector1">first vector</param>
        /// <param name="vector2">second vector</param>
        /// <returns>Cross product</returns>
        public static Vector Cross(Vector vector1, Vector vector2)
        {
            return new Vector(
                vector1.Y * vector2.Z - vector2.Y * vector1.Z,
                -(vector1.X * vector2.Z - vector2.X * vector1.Z),
                vector1.X * vector2.Y - vector2.X * vector1.Y
            );
        }

        private static bool CompareFloat(float a, float b, float epsilon)
        {
            return Math.Abs(a - b) < epsilon;
        }

        /// <summary>
        /// Compares if two vectors are similar (eacu component of the vector does not differ more than a defined epsilon)
        /// </summary>
        /// <param name="a">First vector</param>
        /// <param name="b">Second vector</param>
        /// <param name="epsilon">maximum difference of each component</param>
        /// <returns>true if the vectors are similar, false otherwise</returns>
        public static bool Compare(Vector a, Vector b, float epsilon)
        {
            return
                CompareFloat(a.X, b.X, epsilon) &&
                CompareFloat(a.Y, b.Y, epsilon) &&
                CompareFloat(a.Z, b.Z, epsilon);
        }
    }
}
