using MolecularMachines.Framework.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using MolecularMachines.Framework.Geometry;

namespace Assets.MolecularMachinesAdapter.Adapters
{
    class GameObjectLocRot : LocRot
    {
        public GameObjectLocRot(GameObject gameObject)
        {
            this.gameObject = gameObject;
        }

        public GameObjectLocRot(GameObject gameObject, LocRot initial) : this(gameObject)
        {
            this.Location = initial.Location;
            this.Rotation = initial.Rotation;
        }

        private GameObject gameObject;

        public override Vector Location
        {
            get { return gameObject.transform.position.ToModel(); }
            set { gameObject.transform.position = value.ToUnity(); }
        }

        public override MolecularMachines.Framework.Geometry.Quaternion Rotation
        {
            get { return gameObject.transform.rotation.ToModel(); }
            set { gameObject.transform.rotation = value.ToUnity(); }
        }
    }
}
