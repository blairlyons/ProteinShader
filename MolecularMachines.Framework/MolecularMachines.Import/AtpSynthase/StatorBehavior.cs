using MolecularMachines.Framework.Model.Behaviors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MolecularMachines.Import.AtpSynthase
{
    [EntityBehaviorId("atpSynthase.stator")]
    public class StatorBehavior : EntityBehavior
    {
        [BoundEntity("atpSynthaseSite")]
        public AtpSynthaseBehavior atpSynthase { get; set; }

        [State(InitialState = true)]
        public void Idle()
        {
            float rotationProgress = atpSynthase.RotationProgress;

            var conforamtions = this.Owner.Class.StructureClass.Conformations;
            int conformationIndex = (int)Math.Round(rotationProgress * conforamtions.Count) % conforamtions.Count;
            SetConformation(conforamtions[conformationIndex]);
        }
    }
}
