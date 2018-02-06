using MolecularMachines.Framework.Geometry;
using MolecularMachines.Framework.Model.Behaviors;
using MolecularMachines.Framework.Model.Markers;
using MolecularMachines.Import.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MolecularMachines.Import.Kinesin2
{
    [EntityBehaviorId("kinesin2.kinesin")]
    public class KinesinBehavior : EntityBehavior
    {
        [BindingSite("neckLinker1")]
        public BindingSite NeckLinker1Site { get; set; }

        [BindingSite("neckLinker2")]
        public BindingSite NeckLinker2Site { get; set; }

        public HeadBehavior Head1 { get; set; }
        public HeadBehavior Head2 { get; set; }

        private RandomRotation randomNeckLinker1RotationNormal;
        private RandomRotation randomNeckLinker1RotationNeckZipperTriggered;

        private static readonly float pi = GeometryUtils.pi;

        [State(InitialState = true)]
        public void Init()
        {
            randomNeckLinker1RotationNormal = new RandomRotation(NeckLinker1Site.LocalLocRot, 0, 0, -1.4f, -1.4f, pi - 0.2f, pi + 0.1f);
            randomNeckLinker1RotationNeckZipperTriggered = new RandomRotation(NeckLinker1Site.LocalLocRot, 0.3f, -0.2f, 0.0f, 0.45f, -0.5f, 0.5f);

            Head1 = (HeadBehavior)NeckLinker1Site.OtherEntity.BindingSiteById("head").OtherEntity.Behavior;
            Head2 = (HeadBehavior)NeckLinker2Site.OtherEntity.BindingSiteById("head").OtherEntity.Behavior;

            Head1.Kinesin = this;
            Head1.OtherHead = Head2;

            Head2.Kinesin = this;
            Head2.OtherHead = Head1;

            SetState(Normal);
        }

        [State]
        public void Normal()
        {
            if (Head1.IsNeckZipperTriggered || Head2.IsNeckZipperTriggered)
            {
                randomNeckLinker1RotationNeckZipperTriggered.Reset();
                SetState(NeckZipperTriggered);
            }
            else
            {
                randomNeckLinker1RotationNormal.Update();
            }
        }

        [State]
        public void NeckZipperTriggered()
        {
            if (!Head1.IsNeckZipperTriggered && !Head2.IsNeckZipperTriggered)
            {
                randomNeckLinker1RotationNormal.Reset();
                SetState(Normal);
            }
            else
            {
                randomNeckLinker1RotationNeckZipperTriggered.Update();
            }
        }
    }
}
