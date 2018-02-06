using MolecularMachines.Framework.Logging;
using MolecularMachines.Framework.Model.Behaviors;
using MolecularMachines.Framework.Model.Entities;
using MolecularMachines.Framework.Model.Markers;
using MolecularMachines.Framework.Model.Trajectories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MolecularMachines.Import.AtpSynthase
{
    [EntityBehaviorId("atpSynthase.f1a")]
    public class F1aBehavior : EntityBehavior
    {
        [BoundEntity("atpSynthaseSite")]
        public AtpSynthaseBehavior AtpSynthase { get; set; }

        [Sensor("sensor")]
        public Sensor Sensor { get; set; }

        [Marker("releaseMarker")]
        public Marker ReleaseMarker { get; set; }

        [BindingSite("adpSite")]
        public BindingSite AdpSite { get; set; }

        [BindingSite("prSite")]
        public BindingSite PrSite { get; set; }

        [State(InitialState = true)]
        public void Main()
        {
            float rotationProgress = AtpSynthase.RotationProgress;

            var conforamtions = this.Owner.Class.StructureClass.Conformations;
            int conformationIndex = (int)Math.Round(rotationProgress * conforamtions.Count) % conforamtions.Count;
            SetConformation(conforamtions[conformationIndex]);
        }

        public void BindAdpAndPr()
        {
            if (AdpSite.IsFree)
            {
                AdpBehavior adp;
                if (Sensor.FindNearestWithBehavior<AdpBehavior>(a => a.IsAdp && a.IsFree, out adp))
                {
                    var trajectory = new TrajectoryBuilder()
                        //.Attract(adp.MainSite, this.AdpSite)
                        .Movement(this.AdpSite, TimeSpan.FromSeconds(1.0), false)
                        .Binding(this.AdpSite, adp.MainSite)
                        .Create();

                    AdpSite.InitiateBinding(adp.MainSite);
                    adp.Owner.Compound.Trajectory(trajectory);
                }
            }

            if (PrSite.IsFree)
            {
                PrBehavior pr;
                if (Sensor.FindNearestWithBehavior<PrBehavior>(b => b.IsFree, out pr))
                {
                    var trajectory = new TrajectoryBuilder()
                        //.Attract(pr.AdpSite, this.PrSite)
                        .Movement(this.PrSite, TimeSpan.FromSeconds(1.0), false)
                        .Binding(this.PrSite, pr.AdpSite)
                        .Create();

                    PrSite.InitiateBinding(pr.AdpSite);
                    pr.Owner.Compound.Trajectory(trajectory);
                }
            }
        }

        public bool IsAdpAndPrBinding
        {
            get
            {
                return AdpSite.IsBinding || PrSite.IsBinding;
            }
        }

        public bool IsAdpAndPrBound
        {
            get { return AdpSite.IsBound && PrSite.IsBound; }
        }

        public void MakeAtp()
        {
            if (!IsAdpAndPrBound)
            {
                throw new Exception("can not make ATP if AdpSite or PrSite is not bound");
            }

            var adp = (AdpBehavior)AdpSite.OtherEntity.Behavior;
            var pr = (PrBehavior)PrSite.OtherEntity.Behavior;

            Log.Info("make ATP out of "+adp.Owner.DiagnosticId+" and "+pr.Owner.DiagnosticId);

            adp.PrSite.InstantBind(pr.AdpSite);
        }

        public void ReleaseAtp()
        {
            if (AdpSite.IsBound)
            {
                var trajectory = new TrajectoryBuilder()
                    //.Attract(AdpSite.OtherEntity, ReleaseMarker)
                    .Movement(this.ReleaseMarker, TimeSpan.FromSeconds(1.0), false)
                    .Float()
                    .Create();

                AdpSite.ReleaseBond(trajectory);
            }
        }
    }
}
