using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MolecularMachines.Framework.Geometry
{
    // parts are taken from:
    // 
    // 

    /// <summary>
    /// Represents a quaternion that can be used for 3D rotation.
    /// 
    /// Parts of the implementation are taken from:
    /// http://www.technologicalutopia.com/sourcecode/xnageometry/quaternion.cs.htm
    /// http://physicsforgames.blogspot.co.at/2010/02/quaternions.html
    /// https://community.secondlife.com/t5/Scripting/What-is-the-formula-for-reversing-a-quaternion-rotation/td-p/131429
    /// https://bitbucket.org/sinbad/ogre/src/9db75e3ba05c/OgreMain/include/OgreVector3.h?fileviewer=file-view-default#cl-651
    /// 
    /// Attention:
    /// Not all properties and methods have been tested.
    /// There may be some methods that do not always return the promised result.
    /// If something unexpected happens, it is recommended to also check the implementation in this class.
    /// </summary>
    public struct Quaternion
    {
        public Quaternion(float x, float y, float z, float w)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
            this.W = w;
        }

        public float X { get; private set; }
        public float Y { get; private set; }
        public float Z { get; private set; }
        public float W { get; private set; }

        /// <summary>
        /// Identity rotation quaternion. Indicates "no rotation"
        /// </summary>
        public static Quaternion Identity
        {
            get { return new Quaternion(0, 0, 0, 1); }
        }

        /// <summary>
        /// Normalized quaternion
        /// </summary>
        public Quaternion Normalized
        {
            get
            {
                float num2 = (((this.X * this.X) + (this.Y * this.Y)) + (this.Z * this.Z)) + (this.W * this.W);
                float num = 1f / ((float)Math.Sqrt(num2));

                return new Quaternion(
                    this.X * num,
                    this.Y * num,
                    this.Z * num,
                    this.W * num
                );
            }
        }

        /// <summary>
        /// Rotate a vector around the origin (0,0,0) using this rotation quaternion.
        /// </summary>
        /// <param name="v">Vector to rotate</param>
        /// <returns>vecotr after rotation</returns>
        public Vector Rotate(Vector v)
        {
            return this * v;
        }

        /// <summary>
        /// Returns the inverted rotation, in order that <c>q</c> combined with <c>q.GetInvertedRotation()</c> results in <see cref="Identity"/>.
        /// </summary>
        /// <returns>Inverted quaternion</returns>
        public Quaternion GetInverted()
        {
            return new Quaternion(
                -this.X,
                -this.Y,
                -this.Z,
                this.W
            );
        }

        /// <summary>
        /// Returns a quaternion defined by a rotation around an arbitrary axis.
        /// For the axis <see cref="Vector.Length"/> should be <c>1</c> in order to obtain a normalized quaternion.
        /// </summary>
        /// <param name="axis">axis to rotate around</param>
        /// <param name="angle">Angle of the rotation</param>
        /// <returns>quaternion</returns>
        public static Quaternion FromAxisAngle(Vector axis, float angle)
        {

            float num2 = angle * 0.5f;
            float num = (float)Math.Sin((float)num2);
            float num3 = (float)Math.Cos((float)num2);

            return new Quaternion(axis.X * num, axis.Y * num, axis.Z * num, num3);
        }

        /// <summary>
        /// Returns a quaternion defined by the rotation angles around the x-, y- and z-axes (yaw, pitch, roll).
        /// </summary>
        /// <param name="yaw">angle of the rotation around the x-axis</param>
        /// <param name="pitch">angle of the rotation around the y-axis</param>
        /// <param name="roll">angle of the rotation around the z-axis</param>
        /// <returns>quaternion</returns>
        public static Quaternion FromYawPitchRoll(float yaw, float pitch, float roll)
        {
            float num9 = roll * 0.5f;
            float num6 = (float)Math.Sin((float)num9);
            float num5 = (float)Math.Cos((float)num9);
            float num8 = pitch * 0.5f;
            float num4 = (float)Math.Sin((float)num8);
            float num3 = (float)Math.Cos((float)num8);
            float num7 = yaw * 0.5f;
            float num2 = (float)Math.Sin((float)num7);
            float num = (float)Math.Cos((float)num7);

            return new Quaternion(
                ((num * num4) * num5) + ((num2 * num3) * num6),
                ((num2 * num3) * num5) - ((num * num4) * num6),
                ((num * num3) * num6) - ((num2 * num4) * num5),
                ((num * num3) * num5) + ((num2 * num4) * num6)
            );
        }

        /// <summary>
        /// Returns a quaternion that is rotates vector a to result vector b.
        /// </summary>
        /// <param name="a">initial vector</param>
        /// <param name="b">resulting vector</param>
        /// <returns>quaternion</returns>
        public static Quaternion FromVectorRotation(Vector a, Vector b)
        {
            Vector v0 = a.Normalized;
            Vector v1 = b.Normalized;

            float d = Vector.Dot(v0, v1);
            // If dot == 1, vectors are the same
            if (d >= 1.0f)
            {
                return Quaternion.Identity;
            }
            if (d < (1e-6f - 1.0f))
            {
                // Generate an axis
                Vector axis = Vector.Cross(Vector.AxisX, a);
                if (axis.Length < 1e-6f) // pick another if colinear
                { axis = Vector.Cross(Vector.AxisY, a); }

                axis = axis.Normalized;
                return FromAxisAngle(axis, (float)Math.PI);
            }
            else
            {
                float s = (float)Math.Sqrt((1 + d) * 2);
                float invs = 1 / s;

                Vector c = Vector.Cross(v0, v1);

                var q = new Quaternion(
                    c.X * invs,
                    c.Y * invs,
                    c.Z * invs,
                    s * 0.5f
                );

                q = q.Normalized;
                return q;
            }
        }

        /// <summary>
        /// Returns a quaternion that combines the rotation of two separate quaternions a.k.a. rotation concatenation.
        /// </summary>
        /// <param name="a">first quaternion</param>
        /// <param name="b">second quaternion</param>
        /// <returns>combined rotation quaternion</returns>
        public static Quaternion CombineRotation(Quaternion a, Quaternion b)
        {
            return a * b;
        }

        /// <summary>
        /// Linear interpolation of two rotation quaternions.
        /// </summary>
        /// <param name="quaternion1">start quaternion</param>
        /// <param name="quaternion2">end quaternion</param>
        /// <param name="amount">number between 0 and 1</param>
        /// <returns>Interpolated quaternion</returns>
        public static Quaternion Lerp(Quaternion quaternion1, Quaternion quaternion2, float amount)
        {
            float num = amount;
            float num2 = 1f - num;
            Quaternion quaternion = new Quaternion();
            float num5 = (((quaternion1.X * quaternion2.X) + (quaternion1.Y * quaternion2.Y)) + (quaternion1.Z * quaternion2.Z)) + (quaternion1.W * quaternion2.W);
            if (num5 >= 0f)
            {
                quaternion.X = (num2 * quaternion1.X) + (num * quaternion2.X);
                quaternion.Y = (num2 * quaternion1.Y) + (num * quaternion2.Y);
                quaternion.Z = (num2 * quaternion1.Z) + (num * quaternion2.Z);
                quaternion.W = (num2 * quaternion1.W) + (num * quaternion2.W);
            }
            else
            {
                quaternion.X = (num2 * quaternion1.X) - (num * quaternion2.X);
                quaternion.Y = (num2 * quaternion1.Y) - (num * quaternion2.Y);
                quaternion.Z = (num2 * quaternion1.Z) - (num * quaternion2.Z);
                quaternion.W = (num2 * quaternion1.W) - (num * quaternion2.W);
            }
            float num4 = (((quaternion.X * quaternion.X) + (quaternion.Y * quaternion.Y)) + (quaternion.Z * quaternion.Z)) + (quaternion.W * quaternion.W);
            float num3 = 1f / ((float)Math.Sqrt((double)num4));

            return new Quaternion(
                quaternion.X * num3,
                quaternion.Y * num3,
                quaternion.Z * num3,
                quaternion.W * num3
            );
        }


        /// <summary>
        /// Like <see cref="FromYawPitchRoll(float, float, float)"/> but in degrees instead of radiants.
        /// Yaw and pitch are also swapped to achieve the same result as in the Unity editor.
        /// </summary>
        /// <param name="yawDeg"></param>
        /// <param name="pitchDeg"></param>
        /// <param name="rollDeg"></param>
        /// <returns>quaternion</returns>
        public static Quaternion FromYawPitchRollDeg(float yawDeg, float pitchDeg, float rollDeg)
        {
            return Quaternion.FromYawPitchRoll(
                GeometryUtils.DegToRad(pitchDeg), //  <-+-- have been switched to match Unity rotation
                GeometryUtils.DegToRad(yawDeg),   //  <-+
                GeometryUtils.DegToRad(rollDeg)
            );
        }

        /// <summary>
        /// Quaternion multiplication with a vector (equals rotation)
        /// </summary>
        /// <param name="quat">Quaternion</param>
        /// <param name="vec">Vector</param>
        /// <returns>multiplicated vector</returns>
        public static Vector operator *(Quaternion quat, Vector vec)
        {
            float num = quat.X * 2f;
            float num2 = quat.Y * 2f;
            float num3 = quat.Z * 2f;
            float num4 = quat.X * num;
            float num5 = quat.Y * num2;
            float num6 = quat.Z * num3;
            float num7 = quat.X * num2;
            float num8 = quat.X * num3;
            float num9 = quat.Y * num3;
            float num10 = quat.W * num;
            float num11 = quat.W * num2;
            float num12 = quat.W * num3;

            return new Vector(
                (1f - (num5 + num6)) * vec.X + (num7 - num12) * vec.Y + (num8 + num11) * vec.Z,
                (num7 + num12) * vec.X + (1f - (num4 + num6)) * vec.Y + (num9 - num10) * vec.Z,
                (num8 - num11) * vec.X + (num9 + num10) * vec.Y + (1f - (num4 + num5)) * vec.Z
            );
        }

        /// <summary>
        /// Quaternion multiplication (equals concatenation)
        /// </summary>
        /// <param name="quaternion1">Quaternion1</param>
        /// <param name="quaternion2">Quaternion2</param>
        /// <returns>multiplicated vector</returns>
        public static Quaternion operator *(Quaternion quaternion1, Quaternion quaternion2)
        {
            float x = quaternion1.X;
            float y = quaternion1.Y;
            float z = quaternion1.Z;
            float w = quaternion1.W;
            float num4 = quaternion2.X;
            float num3 = quaternion2.Y;
            float num2 = quaternion2.Z;
            float num = quaternion2.W;
            float num12 = (y * num2) - (z * num3);
            float num11 = (z * num4) - (x * num2);
            float num10 = (x * num3) - (y * num4);
            float num9 = ((x * num4) + (y * num3)) + (z * num2);

            return new Quaternion(
                ((x * num) + (num4 * w)) + num12,
                ((y * num) + (num3 * w)) + num11,
                ((z * num) + (num2 * w)) + num10,
                (w * num) - num9
            );
        }

        public override string ToString()
        {
            return "[" + X.ToString("0.00") + " ; " + Y.ToString("0.00") + " ; " + Z.ToString("0.00") + " ; " + W.ToString("0.00") + "]";
        }

        private static bool CompareFloat(float a, float b, float epsilon)
        {
            return Math.Abs(a - b) < epsilon;
        }

        /// <summary>
        /// Compares the components of two quaternions
        /// </summary>
        /// <param name="a">quaternion 1</param>
        /// <param name="b">quaternion 2</param>
        /// <param name="epsilon">maximum difference of each component</param>
        /// <returns>true if the components of both quaternions are similar</returns>
        public static bool Compare(Quaternion a, Quaternion b, float epsilon)
        {
            return
                CompareFloat(a.X, b.X, epsilon) &&
                CompareFloat(a.Y, b.Y, epsilon) &&
                CompareFloat(a.Z, b.Z, epsilon) &&
                CompareFloat(a.W, b.W, epsilon);
        }
    }
}
