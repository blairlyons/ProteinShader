using MolecularMachines.Framework.Model.Markers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MolecularMachines.Framework.Model.Trajectories
{
    public class TrajectoryBuilder
    {
        private List<TrajectorySection> sections = new List<TrajectorySection>();

        public TrajectoryBuilder Attract(ISpaceInherent applyAt, ISpaceInherent destination, float untilDistanceLessThan, TimeSpan timeout)
        {
            sections.Add(new Attraction(applyAt, destination, untilDistanceLessThan, timeout));

            return this;
        }

        public TrajectoryBuilder Attract(ISpaceInherent applyAt, ISpaceInherent destination)
        {
            return Attract(applyAt, destination, 10f, TimeSpan.FromSeconds(30));
        }

        public TrajectoryBuilder Movement(ISpaceInherent destination, TimeSpan duration, bool collidersActive)
        {
            sections.Add(new Movement(destination, duration, collidersActive));

            return this;
        }

        public TrajectoryBuilder Binding(BindingSite activeSite, BindingSite passiveSite)
        {
            sections.Add(new Binding(activeSite, passiveSite));

            return this;
        }

        public TrajectoryBuilder Float()
        {
            sections.Add(new Float());

            return this;
        }

        public Trajectory Create()
        {
            return new Trajectory(this.sections.ToArray());
        }
    }
}
