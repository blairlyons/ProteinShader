using MolecularMachines.Framework.Geometry;
using MolecularMachines.Framework.Model.Behaviors;
using MolecularMachines.Framework.Model.Markers;
using MolecularMachines.Framework.Utils;
using MolecularMachines.Import.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MolecularMachines.Import.KinesinConcept
{
    [EntityBehaviorId("kinesin.head")]
    public class HeadBehavior : EntityBehavior
    {
        public HeadBehavior()
        {
            this.id = idCounter++;
        }

        private int id = 0;
        private static int idCounter = 0;

        [BindingSite("neckSite")]
        public BindingSite NeckSite { get; set; }

        [BindingSite("tubulinSite")]
        public BindingSite TubulinSite { get; set; }

        [Sensor("tubulinSensor")]
        public Sensor TubulinSensor { get; set; }

        private RandomRotation neckRandom;
        private Timer stepTimer = new Timer(TimeSpan.FromSeconds(1));

        private static readonly float pi = GeometryUtils.pi;

        [State(InitialState = true)]
        public void Init()
        {
            this.neckRandom = new RandomRotation(NeckSite.LocalLocRot, -0.2f, 0.2f, -0.2f, 0.2f, pi - 0.1f, pi + 0.1f);
            SetState(Free);
        }

        private void NeckRandom()
        {
            this.neckRandom.Update();
        }

        [State(Conformation = "free")]
        public void Free()
        {
            NeckRandom();

            TubulinBehavior tubulin;
            if (TubulinSensor.FindNearestWithBehavior<TubulinBehavior>(t => true, out tubulin))
            {
                tubulin.KinesinSite.InstantBind(this.TubulinSite);

                stepTimer.Reset();
                SetState(BoundWeak);
            }
        }

        [State(Conformation = "weaklyBound")]
        public void BoundWeak()
        {
            NeckRandom();

            if (stepTimer.IsDone())
            {
                stepTimer.Reset();
                SetState(ReleaseAdp);
            }
        }

        [State]
        public void ReleaseAdp()
        {
            NeckRandom();

            if (stepTimer.IsDone())
            {
                stepTimer.Reset();
                SetState(BindAtp);
            }
        }

        [State]
        public void BindAtp()
        {
            NeckRandom();

            if (stepTimer.IsDone())
            {
                stepTimer.Reset();
                SetState(ThrowNeckForward);
            }
        }

        [State(Conformation = "stronglyBound")]
        public void ThrowNeckForward()
        {
            float p;
            if (stepTimer.IsDone(out p))
            {
                TubulinSite.ReleaseBond();

                stepTimer.Reset();
                SetState(Inactive);
            }
        }

        [State(Conformation = "free")]
        public void Inactive()
        {
            if (stepTimer.IsDone())
            {
                stepTimer.Reset();
                SetState(Free);
            }
        }
    }
}
