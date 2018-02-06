using MolecularMachines.Framework.Model.Behaviors;
using MolecularMachines.Framework.Model.Markers;
using MolecularMachines.Framework.Model.SpatialLinks;
using MolecularMachines.Framework.Model.Trajectories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MolecularMachines.Import.Microtubule
{
    [EntityBehaviorId("microtubule.tubulin")]
    public class TubulinBehavior : EntityBehavior
    {
        [BindingSite("previous")]
        public BindingSite PreviousSite { get; set; }

        [BindingSite("next")]
        public BindingSite NextSite { get; set; }

        [Sensor("sensor")]
        public Sensor Sensor { get; set; }

        [State(InitialState = true)]
        public void Free()
        {
            TubulinBehavior nearest;
            if (
                    Sensor.FindNearestWithBehavior<TubulinBehavior>(
                        b => b.PreviousSite.IsFree && b.Owner.Compound.SpatialLink.Type == SpatialLinkType.Floating,
                        out nearest
                    )
               )
            {
                NextSite.InitiateBinding(nearest.PreviousSite);

                var trajectory = new TrajectoryBuilder()
                    .Attract(nearest.PreviousSite, this.NextSite, 40, TimeSpan.FromSeconds(5))
                    .Binding(this.NextSite, nearest.PreviousSite)
                    .Create();

                nearest.Owner.Compound.Trajectory(trajectory);

                SetState(Bound);
            }
        }

        [State]
        public void Bound()
        {
            if (NextSite.IsFree)
            {
                SetState(Free);
            }
        }
    }
}
