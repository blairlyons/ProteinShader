using MolecularMachines.Framework.Model.SpatialLinks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MolecularMachines.Framework.Model;
using UnityEngine;
using MolecularMachines.Framework.Model.Trajectories;

namespace Assets.MolecularMachinesAdapter.Adapters.Trajectories
{
    class TrajectoryUnity : SpatialLink
    {
        public TrajectoryUnity(CompoundUnity compoundUnity, Trajectory trajectory)
        {
            this.compoundUnity = compoundUnity;
            this.trajectory = trajectory;
            this.locRot = new GameObjectLocRot(compoundUnity.GameObject);

            this.Next();
        }

        private CompoundUnity compoundUnity;
        private Trajectory trajectory;
        public GameObjectLocRot locRot;

        public override LocRot LocRot
        {
            get
            {
                return locRot;
            }
        }

        private ITrajectorySectionUnity currentSection;

        public override SpatialLinkType Type { get { return SpatialLinkType.Trajectory; } }

        public TimeSpan CurrentSectionTime { get { return trajectory.CurrentSectionTime; } }

        public CompoundUnity Compund { get { return this.compoundUnity; } }

        protected override void OnUpdate()
        {
            base.OnUpdate();

            if (this.currentSection != null)
            {
                currentSection.Update();
            }
        }

        public void Next()
        {
            var ls = this.currentSection;

            if (this.trajectory.Next(compoundUnity))
            {
                var section = this.trajectory.CurrentSection;

                if (section is Attraction) { this.currentSection = new AttractionUnity(this, section); }
                else if (section is Binding) { this.currentSection = new BindingUnity(this, section); }
                else if (section is Float) { this.currentSection = new FloatUnity(this, section); }
                else if (section is Movement) { this.currentSection = new MovementUnity(this, section); }
                else
                {
                    throw new Exception("no unity implementation for trajectory section type: " + section.GetType().Name);
                }

                this.currentSection.Start();
            }
            else
            {
                this.currentSection = null;
            }
        }

        private bool collidersActive = true;
        public void SetCollidersActive(bool collidersActive)
        {
            this.collidersActive = collidersActive;
        }
        public override bool CollidersActive { get { return collidersActive; } }

        public override bool IsAlive
        {
            get
            {
                return !this.compoundUnity.RootEntity.IsDisposed;
            }
        }
    }
}
