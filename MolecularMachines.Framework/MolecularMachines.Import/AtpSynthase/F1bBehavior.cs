using MolecularMachines.Framework.Model.Behaviors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MolecularMachines.Import.AtpSynthase
{
    [EntityBehaviorId("atpSynthase.f1b")]
    public class F1bBehavior : EntityBehavior
    {
        [BoundEntity("atpSynthaseSite")]
        public AtpSynthaseBehavior AtpSynthase { get; set; }

        [State(InitialState = true)]
        public void Idle()
        {
            float rotationProgress = AtpSynthase.RotationProgress;

            var conforamtions = this.Owner.Class.StructureClass.Conformations;
            int conformationIndex = (int)Math.Round(rotationProgress * conforamtions.Count) % conforamtions.Count;
            SetConformation(conforamtions[conformationIndex]);
        }
    }
}
