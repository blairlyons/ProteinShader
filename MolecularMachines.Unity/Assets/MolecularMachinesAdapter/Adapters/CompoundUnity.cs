using MolecularMachines.Framework.Model;
using MolecularMachines.Framework.Model.Compounds;
using MolecularMachines.Framework.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using MolecularMachines.Framework.Model.SpatialLinks;
using Assets.MolecularMachinesAdapter.Adapters.SpatialLinks;
using MolecularMachines.Framework.Model.Trajectories;
using Assets.MolecularMachinesAdapter.Adapters.Trajectories;

namespace Assets.MolecularMachinesAdapter.Adapters
{
    public class CompoundUnity : Compound
    {
        public CompoundUnity(MMEnvironment environment, Entity rootEntity) : base(environment, rootEntity)
        {
            this.GameObject = new GameObject("c");

            // copy initial location from rootEntity
            this.GameObject.transform.position = rootEntity.LocRot.Location.ToUnity();
            this.GameObject.transform.rotation = rootEntity.LocRot.Rotation.ToUnity();

            // rigidbody settings
            this.Rigidbody = this.GameObject.AddComponent<Rigidbody>();

            this.Rigidbody.drag = 2f;
            this.Rigidbody.angularDrag = 0.5f;
            this.Rigidbody.useGravity = false;

            this.id = idCounter++;
        }

        private int id; // TODO remove this
        private static int idCounter = 0;

        public GameObject GameObject { get; private set; }
        public Rigidbody Rigidbody { get; private set; }

        protected override void OnEntityAdd(Entity entity)
        {
            base.OnEntityAdd(entity);

            var entityUnity = (EntityUnity)entity;
            entityUnity.GameObject.transform.parent = this.GameObject.transform;
        }

        protected override void OnEntityRemove(Entity entity)
        {
            base.OnEntityRemove(entity);

            var entityUnity = (EntityUnity)entity;
            entityUnity.GameObject.transform.parent = null;
        }

        protected override void OnMassChanged()
        {
            base.OnMassChanged();
            this.Rigidbody.mass = this.Mass;
        }

        protected override void OnUpdate()
        {
            try
            {
                base.OnUpdate();
            }
            catch (Exception e)
            {
                Debug.LogWarning(e.Message+" ... in base.OnUpdate()");
            }
        }

        protected override SpatialLink OnFix(LocRot fixedLocRot)
        {
            return new FixedUnity(this, fixedLocRot);
        }

        protected override SpatialLink OnFloat(LocRot initialLocRot)
        {
            return new FloatingUnity(this, initialLocRot);
        }

        protected override SpatialLink OnTrajectory(Trajectory trajectory)
        {
            return new TrajectoryUnity(this, trajectory);
        }

        public override string ToString()
        {
            var s = new StringBuilder();
            s.Append(id.ToString());
            s.Append(": ");
            foreach (var entity in this.Entities)
            {
                s.Append(entity.Class.Id);
                s.Append("; ");
            }

            return s.ToString();
        }

        protected override void OnDispose()
        {
            base.OnDispose();

            GameObject.Destroy(this.GameObject);
        }
    }
}
