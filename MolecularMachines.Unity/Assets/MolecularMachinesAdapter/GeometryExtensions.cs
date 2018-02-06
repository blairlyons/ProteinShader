using MolecularMachines.Framework.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.MolecularMachinesAdapter
{
    public static class GeometryExtensions
    {
        public static Vector3 ToUnity(this Vector vector)
        {
            return new Vector3(vector.X, vector.Y, vector.Z);
        }

        public static UnityEngine.Quaternion ToUnity(this MolecularMachines.Framework.Geometry.Quaternion quaternion)
        {
            return new UnityEngine.Quaternion(
                quaternion.X,
                quaternion.Y,
                quaternion.Z,
                quaternion.W
            );
        }

        public static Vector ToModel(this Vector3 vector)
        {
            return new Vector(vector.x, vector.y, vector.z);
        }

        public static MolecularMachines.Framework.Geometry.Quaternion ToModel(this UnityEngine.Quaternion quaternion)
        {
            return new MolecularMachines.Framework.Geometry.Quaternion(
                quaternion.x,
                quaternion.y,
                quaternion.z,
                quaternion.w
            );
        }

        public static UnityEngine.Color ToUnity(this MolecularMachines.Framework.Model.Color color)
        {
            return new UnityEngine.Color(
                color.R,
                color.G,
                color.B,
                color.A
            );
        }
    }
}
