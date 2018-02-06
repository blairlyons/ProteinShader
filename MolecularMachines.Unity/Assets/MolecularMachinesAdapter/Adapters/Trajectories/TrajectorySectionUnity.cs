using MolecularMachines.Framework.Model;
using MolecularMachines.Framework.Model.SpatialLinks;
using MolecularMachines.Framework.Model.Trajectories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.MolecularMachinesAdapter.Adapters.Trajectories
{
    abstract class TrajectorySectionUnity<TSection> : ITrajectorySectionUnity
        where TSection : TrajectorySection
    {
        public TrajectorySectionUnity(TrajectoryUnity trajectoryUnity, TrajectorySection section)
        {
            this.trajectoryUnity = trajectoryUnity;
            this.info = (TSection)section;
        }

        private TrajectoryUnity trajectoryUnity;
        protected TSection info;

        public TimeSpan CurrentSectionTime { get { return trajectoryUnity.CurrentSectionTime; } }

        public CompoundUnity Compound
        {
            get { return this.trajectoryUnity.Compund; }
        }

        public TrajectoryUnity TrajectoryUnity
        {
            get { return this.trajectoryUnity; }
        }

        public LocRot LocRot
        {
            get { return this.trajectoryUnity.LocRot; }
        }

        protected void Next()
        {
            trajectoryUnity.Next();
        }

        public virtual void Start()
        {

        }

        public abstract void Update();
    }
}
