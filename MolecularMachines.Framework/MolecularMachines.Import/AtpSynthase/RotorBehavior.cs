using MolecularMachines.Framework.Geometry;
using MolecularMachines.Framework.Logging;
using MolecularMachines.Framework.Model;
using MolecularMachines.Framework.Model.Behaviors;
using MolecularMachines.Framework.Model.Entities;
using MolecularMachines.Framework.Model.Markers;
using MolecularMachines.Framework.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MolecularMachines.Import.AtpSynthase
{
    [EntityBehaviorId("atpSynthase.rotor")]
    public class RotorBehavior : EntityBehavior
    {
        [BindingSite("atpSynthaseSite")]
        public BindingSite AtpSynthaseSite { get; set; }

        private bool smooth = true;

        private LocRot atpSynthaseLocRot;

        private F0cBehavior[] f0cs;
        private Quaternion[] rotations;
        private int activeF0cIndex;

        public F0cBehavior CurrentF0c { get; private set; }

        private Quaternion rotationStart;
        private Quaternion rotationFinal;
        private Interval rotationBrownianInterval;
        private float rotationBrownianProgress = 0;
        private Timer rotationSmoothTimer = new Timer(TimeSpan.FromSeconds(0.5));

        public bool IsRotating { get; private set; }

        public int ActiveF0cIndex
        {
            get { return this.activeF0cIndex; }
        }

        public void Rotate()
        {
            var lastIndex = activeF0cIndex;
            activeF0cIndex = (activeF0cIndex + 1) % f0cs.Length;

            this.CurrentF0c = f0cs[activeF0cIndex];

            rotationBrownianProgress = 0;
            rotationSmoothTimer.Reset();

            rotationStart = rotations[lastIndex];
            rotationFinal = rotations[activeF0cIndex];

            SetState(Rotating);
        }

        public float RotationProgress
        {
            get { return (float)activeF0cIndex / (float)f0cs.Length; }
        }

        [State(InitialState = true)]
        public void Init()
        {
            // some setup

            bool allBound = (this.Owner.BindingSites.Where(bindingSite => !bindingSite.IsBound).Count() == 0);
            if (allBound)
            {
                Log.Info("all bound...");

                this.f0cs =
                    this.Owner.BindingSites
                        .Where(bindingSite => bindingSite.Id.Contains("f0c"))
                        .Select(bindingSite => (F0cBehavior)bindingSite.OtherEntity.Behavior)
                        .Reverse()
                        .ToArray();

                activeF0cIndex = 0;
                this.CurrentF0c = f0cs[activeF0cIndex];

                var angleDelta = 2 * GeometryUtils.pi / f0cs.Length;
                var angleOffset = 6 * angleDelta; // needed for the CurrentF0c to be at the stator
                this.rotations =
                    Enumerable.Range(0, f0cs.Length)
                    .Select(index => Quaternion.FromAxisAngle(Vector.AxisZ, angleOffset + angleDelta * index))
                    .ToArray();

                this.IsRotating = false;

                this.atpSynthaseLocRot = AtpSynthaseSite.LocalLocRot;

                this.rotationBrownianInterval = new Interval(
                    TimeSpan.FromSeconds(0.1),
                    () => { this.rotationBrownianProgress += (float)random.NextDouble() * 0.1f; }
                );

                SetState(Idle);
            }
        }

        [State]
        public void Idle()
        {

        }

        [State]
        public void Rotating()
        {
            IsRotating = true;
            float p;

            if (smooth) {

                if (rotationSmoothTimer.IsDone(out p))
                {
                    IsRotating = false;
                    SetState(Idle);
                }

                p = EntityStructure.ProgressTransfrom(p);
            }
            else
            {
                rotationBrownianInterval.Update();

                if (rotationBrownianProgress >= 1f)
                {
                    p = 1f;
                    IsRotating = false;
                    SetState(Idle);
                }
                else
                {
                    p = rotationBrownianProgress;
                }
            }

            atpSynthaseLocRot.Rotation = Quaternion.Lerp(rotationStart, rotationFinal, p);
        }

        private Random random = new Random();
    }
}
