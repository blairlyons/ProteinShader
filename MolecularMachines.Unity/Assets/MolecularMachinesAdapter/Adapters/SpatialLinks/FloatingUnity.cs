using MolecularMachines.Framework.Model.SpatialLinks;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MolecularMachines.Framework.Model;
using MolecularMachines.Framework.Model.Compartments;
using MolecularMachines.Framework.Geometry;
using UnityEngine;

namespace Assets.MolecularMachinesAdapter.Adapters.SpatialLinks
{
    class FloatingUnity : SpatialLink
    {
        public FloatingUnity(CompoundUnity compoundUnity, LocRot initialLocRot)
        {
            this.compoundUnity = compoundUnity;
            this.locRot = new GameObjectLocRot(compoundUnity.GameObject);
            this.locRot.Set(initialLocRot);

            this.compartment = compoundUnity.Environment.CompartmentByPoint(initialLocRot.Location);
            if (this.compartment == null)
            {
                Debug.LogError("Can only float inside compartment. no compartment found for " + initialLocRot.Location);

                this.compartment = compoundUnity.Environment.NearestCompartment(initialLocRot.Location);

                Debug.LogWarning("Backoff compartment: "+compartment.Id);
            }
        }

        private CompoundUnity compoundUnity;
        public GameObjectLocRot locRot;
        private Compartment compartment;

        public override LocRot LocRot
        {
            get
            {
                return locRot;
            }
        }

        private float directionChange = 0;
        private Vector3 direction;

        protected override void OnUpdate()
        {
            base.OnUpdate();

            if (this.compartment != null)
            {
                var l = locRot.Location;
                float x = l.X;
                float y = l.Y;
                float z = l.Z;

                var min = this.compartment.CornerMin;
                var max = this.compartment.CornerMax;

                bool reset = false;

                if (x < min.X)
                {
                    reset = true;
                    x = min.X;
                }
                if (y < min.Y)
                {
                    reset = true;
                    y = min.Y;
                }
                if (z < min.Z)
                {
                    reset = true;
                    z = min.Z;
                }

                if (x > max.X)
                {
                    reset = true;
                    x = max.X;
                }
                if (y > max.Y)
                {
                    reset = true;
                    y = max.Y;
                }
                if (z > max.Z)
                {
                    reset = true;
                    z = max.Z;
                }

                if (reset)
                {
                    //LocRot.Location = new Vector(x, y, z);
                    this.compoundUnity.Rigidbody.MovePosition(new Vector3(x, y, z));
                }
            }

            // Brownian Motion
            this.directionChange -= Time.deltaTime;
            if (this.directionChange <= 0)
            {
                this.directionChange = Random.Range(0.1f, 2f);
                this.direction = Random.onUnitSphere * 10f;
            }

            this.compoundUnity.Rigidbody.velocity = direction;
        }

        public override SpatialLinkType Type { get { return SpatialLinkType.Floating; } }
        public override bool CollidersActive { get { return true; } }

        public override bool IsAlive
        {
            get
            {
                return !this.compoundUnity.RootEntity.IsDisposed;
            }
        }
    }
}
