using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MolecularMachines.Framework.Model.Behaviors
{
    /// <summary>
    /// <see cref="EntityBehavior"/> that just does nothing.
    /// </summary>
    [EntityBehaviorId("present.empty")]
    public class EmptyBehavior : EntityBehavior
    {
        [State(InitialState = true)]
        public void Idle()
        {

        }
    }
}
