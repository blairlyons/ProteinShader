using MolecularMachines.Framework.Model.Compounds;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MolecularMachines.Framework.Model.Trajectories
{
    public class Trajectory
    {
        public Trajectory(TrajectorySection[] sections)
        {
            this.sections = sections;
        }

        private int sectionIndex = -1;
        private TrajectorySection[] sections;
        private DateTime sectionStarted;

        public bool Next(Compound compound) // TODO compound argument needed?   
        {
            sectionIndex++;
            this.sectionStarted = DateTime.Now;
            bool hasNext = (sectionIndex < sections.Length);
            return hasNext;
        }

        public TimeSpan CurrentSectionTime
        {
            get
            {
                return (DateTime.Now - this.sectionStarted);
            }
        }

        public TrajectorySection CurrentSection
        {
            get { return this.sections[sectionIndex]; }
        }
    }
}
