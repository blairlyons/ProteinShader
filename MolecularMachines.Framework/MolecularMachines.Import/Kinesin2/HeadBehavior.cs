using MolecularMachines.Framework.Geometry;
using MolecularMachines.Framework.Logging;
using MolecularMachines.Framework.Model.Behaviors;
using MolecularMachines.Framework.Model.Markers;
using MolecularMachines.Import.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MolecularMachines.Import.Kinesin2
{
    [EntityBehaviorId("kinesin2.head")]
    public class HeadBehavior : EntityBehavior
    {
        public HeadBehavior()
        {
        }

        protected override void OnStateChange(StateMethod oldState, StateMethod newState)
        {
            Log.Info("STATE " + this.Owner.Class.Id + " -> " + newState.Method.Name);
        }

        [BindingSite("neckLinker")]
        public BindingSite NeckSite { get; set; }

        [BindingSite("tubulin")]
        public BindingSite TubulinSite { get; set; }

        [Sensor("tubulinSensor")]
        public Sensor TubulinSensor { get; set; }

        public KinesinBehavior Kinesin { get; set; }
        public HeadBehavior OtherHead { get; set; }

        private RandomRotation neckRandom;

        private static readonly float pi = GeometryUtils.pi;

        private float StepTime = 1f;
        private float PauseTime = 1f;

        [State(InitialState = true)]
        public void Init()
        {
            // Kinesin is injected by KinesinBehavior
            if (Kinesin != null)
            {
                if (Owner.Class.Id.Contains("Head1"))
                {
                    this.neckRandom = new RandomRotation(NeckSite.LocalLocRot, -pi - 0.1f, -pi + 0.1f, -0.2f, 0.2f, -0.1f, +0.1f);
                }
                else
                {
                    this.neckRandom = new RandomRotation(NeckSite.LocalLocRot, pi - 0.2f, pi + 0.2f, -0.2f, 0.2f, -0.1f, +0.1f);
                }

                this.neckRandom.Update();

                if (Owner.Class.Id.Contains("Head2"))
                {
                    SetState(Pause);
                }
                else
                {
                    SetState(FindMicrotubule);
                }
            }
        }

        private void NeckLinkerBrownianMotion()
        {
            this.neckRandom.Update();
        }

        [State(Conformation = "free")]
        public void FindMicrotubule()
        {
            NeckLinkerBrownianMotion();

            // ensure that both heads are free
            if (OtherHead.IsCurrentState(OtherHead.FindMicrotubule) || OtherHead.IsCurrentState(OtherHead.Pause))
            {
                TubulinBetaBehavior tubulinBeta;
                if (TubulinSensor.FindNearestWithBehavior<TubulinBetaBehavior>(tb => tb.DockSite.IsFree, out tubulinBeta))
                {
                    tubulinBeta.DockSite.InstantBind(this.TubulinSite);

                    SetState(MoveToMicrotubule);
                }
            }
        }

        [State]
        public void MoveToMicrotubule()
        {
            NeckLinkerBrownianMotion();

            if (TubulinSite.IsBound)
            {
                SetState(WeakBindToMicrotubule);
            }
        }

        [State(Conformation = "boundWeak")]
        public void WeakBindToMicrotubule()
        {
            NeckLinkerBrownianMotion();

            if (SecondsInState >= StepTime)
            {
                SetState(TightBindToMicrotubule);
            }
        }

        [State(Conformation = "boundTight")]
        public void TightBindToMicrotubule()
        {
            NeckLinkerBrownianMotion();

            if (SecondsInState >= StepTime)
            {
                SetState(ReleaseAdp);
            }
        }

        [State]
        public void ReleaseAdp()
        {
            NeckLinkerBrownianMotion();

            SetState(BindAtp);
        }

        [State]
        public void BindAtp()
        {
            NeckLinkerBrownianMotion();

            SetState(TriggerNeckZipper);
        }

        [State(Conformation = "neckZipperTriggered")]
        public void TriggerNeckZipper()
        {
            if (SecondsInState >= StepTime)
            {
                SetState(HydrolizeAtp);
            }
        }

        [State(Conformation = "boundWeakNeckZipperTriggered")]
        public void HydrolizeAtp()
        {
            if (SecondsInState >= StepTime)
            {
                TubulinSite.OtherSite.ReleaseBond();
                neckRandom.Reset();
                SetState(Pause);
            }
        }

        [State]
        public void Pause()
        {
            NeckLinkerBrownianMotion();

            if (SecondsInState >= PauseTime)
            {
                SetState(FindMicrotubule);
            }
        }

        public bool IsNeckZipperTriggered
        {
            get
            {
                var conformationId = this.Owner.Structure.CurrentConformation.Id;
                return (conformationId == "neckZipperTriggered" || conformationId == "boundWeakNeckZipperTriggered");
            }
        }
    }
}
