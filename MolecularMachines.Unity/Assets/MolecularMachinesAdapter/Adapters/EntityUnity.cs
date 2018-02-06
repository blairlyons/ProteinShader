using MolecularMachines.Framework.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MolecularMachines.Framework.Model;
using MolecularMachines.Framework.Model.Colliders;
using UnityEngine;

namespace Assets.MolecularMachinesAdapter.Adapters
{
    class EntityUnity : Entity
    {
        public EntityUnity(MMEnvironment environment, EntityClass entityClass) : base(environment, entityClass)
        {
            this.GameObject = new GameObject(entityClass.Id);
            LocRot = new GameObjectLocRot(this.GameObject);
        }

        public GameObject GameObject { get; private set; }

        protected override void OnUpdate()
        {
            base.OnUpdate();

            var currentCollider = this.Structure.CurrentConformation.Collider;

            // TODO simplify block Compound.SpatialLink can not be null
            bool currentColliderActive;
            if (this.Compound == null) { currentColliderActive = true; Debug.LogWarning("Entity Update: Compound == null"); }
            else if (this.Compound.SpatialLink == null) { currentColliderActive = true; Debug.LogWarning("Entity Update: Compound.SpatialLink == null"); }
            else { currentColliderActive = this.Compound.SpatialLink.CollidersActive; }

            if (currentCollider != this.lastCollider || currentColliderActive != this.lastColliderActive )
            {
                this.lastCollider = currentCollider;
                this.lastColliderActive = currentColliderActive;

                RemoveColliders(); // remove old colliders first
                if (currentColliderActive && MMM.Instance.collidersActive)
                {
                    AddColliders();
                }
            }
        }

        private void RemoveColliders()
        {
            // delete existing colliders
            var existingColliders = GameObject.GetComponents<SphereCollider>();

            foreach (var c in existingColliders)
            {
                GameObject.Destroy(c);
            }

        }

        private void AddColliders()
        {
            var collider = this.Structure.CurrentConformation.Collider;

            foreach (var geometry in collider)
            {
                var type = geometry.GetType();

                var sphereColliderGeometry = (geometry as SphereColliderGeometry);
                if (sphereColliderGeometry != null)
                {
                    var unityCollider = this.GameObject.AddComponent<SphereCollider>();
                    unityCollider.center = sphereColliderGeometry.Center.ToUnity();
                    unityCollider.radius = sphereColliderGeometry.Radius;
                }
                else
                {
                    throw new Exception("unkown collider geometry type: " + type.Name);
                }
            }
        }

        private MolecularMachines.Framework.Model.Colliders.Collider lastCollider = null;
        private bool lastColliderActive = true;

        protected override void OnDispose()
        {
            base.OnDispose();

            GameObject.Destroy(this.GameObject);
        }
    }
}
