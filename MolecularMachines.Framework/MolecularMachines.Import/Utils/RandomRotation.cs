using MolecularMachines.Framework.Geometry;
using MolecularMachines.Framework.Model;
using MolecularMachines.Framework.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MolecularMachines.Import.Utils
{
    class RandomRotation : IFrameUpdate
    {
        public RandomRotation(LocRot locRot, float yawMin, float yawMax, float pitchMin, float pitchMax, float rollMin, float rollMax)
        {
            this.locRot = locRot;

            this.yawMin = yawMin;
            this.yawMax = yawMax;
            this.pitchMin = pitchMin;
            this.pitchMax = pitchMax;
            this.rollMin = rollMin;
            this.rollMax = rollMax;

            InitRotation(locRot.Rotation);
        }

        private LocRot locRot;

        private Timer timer = new Timer(TimeSpan.FromSeconds(0.2));

        private Quaternion start;
        private Quaternion end;

        private float yawMin;
        private float yawMax;
        private float pitchMin;
        private float pitchMax;
        private float rollMin;
        private float rollMax;

        private void InitRotation(Quaternion newStart)
        {
            this.start = newStart;
            this.end =
                Quaternion.FromYawPitchRoll(
                    GeometryUtils.RandomFloat(yawMin, yawMax),
                    GeometryUtils.RandomFloat(pitchMin, pitchMax),
                    GeometryUtils.RandomFloat(rollMin, rollMax)
                );
        }

        public void Reset()
        {
            InitRotation(this.locRot.Rotation);
            timer.Reset();
        }

        public void Update()
        {
            float p;
            if (timer.IsDone(out p))
            {
                timer.Reset();
                InitRotation(this.end);
                p = 0;
            }

            this.locRot.Rotation = Quaternion.Lerp(this.start, this.end, p);
        }
    }
}
