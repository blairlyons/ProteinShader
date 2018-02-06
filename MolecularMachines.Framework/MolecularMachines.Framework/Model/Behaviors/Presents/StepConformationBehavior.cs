using MolecularMachines.Framework.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MolecularMachines.Framework.Model.Behaviors.Presents
{
    /// <summary>
    /// <see cref="EntityBehavior"/> that iterates through all conformations stored 
    /// in the <see cref="Model.Entities.EntityStructureClass"/> of the
    /// <see cref="Model.Entities.Entity"/> (<see cref="Model.Entities.Entity.Class"/>).
    /// The conformation is changed every second.
    /// </summary>
    [EntityBehaviorId("present.step")]
    public class StepConformationBehavior : EntityBehavior
    {
        private TimeSpan interval = TimeSpan.FromSeconds(1);
        private Timer timer = new Timer(TimeSpan.FromSeconds(1));
        private int currentConformationIndex = 0;

        [State(InitialState = true)]
        public void WaitForTimeout()
        {
            float p;
            if (timer.IsDone(out p))
            {
                timer.Reset();
                SetState(ChangeConformation);
            }
        }

        [State]
        public void ChangeConformation()
        {
            currentConformationIndex = (currentConformationIndex + 1) % Owner.Class.StructureClass.Conformations.Count;
            SetConformation(Owner.Class.StructureClass.Conformations[currentConformationIndex]);

            SetState(WaitForTimeout);
        }
    }
}
