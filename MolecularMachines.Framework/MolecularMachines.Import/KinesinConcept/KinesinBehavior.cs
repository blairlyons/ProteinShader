using MolecularMachines.Framework.Geometry;
using MolecularMachines.Framework.Model.Behaviors;
using MolecularMachines.Framework.Model.Markers;
using MolecularMachines.Import.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MolecularMachines.Import.KinesinConcept
{
    [EntityBehaviorId("kinesin.kinesin")]
    public class KinesinBehavior : EntityBehavior
    {
        [BindingSite("neckLinker1Site")]
        public BindingSite NeckLinker1Site { get; set; }

        [BindingSite("neckLinker2Site")]
        public BindingSite NeckLinker2Site { get; set; }

        private RandomRotation neckLinker1Random;
        private RandomRotation neckLinker2Random;

        [State(InitialState = true)]
        public void Init()
        {
            var normalPitch1 = 0f;
            var normalPitch2 = GeometryUtils.pi;
            var deltaPitch = 0.8f;

            this.neckLinker1Random = new RandomRotation(NeckLinker1Site.LocalLocRot, 0, 0, normalPitch1 - deltaPitch, normalPitch1 - deltaPitch, 0, 0);
            this.neckLinker2Random = new RandomRotation(NeckLinker2Site.LocalLocRot, 0, 0, normalPitch2 - deltaPitch, normalPitch2 - deltaPitch, 0, 0);

            SetState(Main);
        }

        [State]
        public void Main()
        {
            this.neckLinker1Random.Update();
            this.neckLinker2Random.Update();
        }
    }
}
