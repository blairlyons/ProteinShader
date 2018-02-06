using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MolecularMachines.Framework.Model.Behaviors
{
    /// <summary>
    /// Marks a method as state method in a <see cref="EntityBehavior"/> class.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class StateAttribute : Attribute
    {
        /// <summary>
        /// Indicates that this is the initial state of the <see cref="EntityBehavior"/>.
        /// </summary>
        public bool InitialState { get; set; }

        /// <summary>
        /// When set, the conformation of the controlled <see cref="Model.Entities.Entity"/> 
        /// is changed to the specified conformation when the state is entered.
        /// </summary>
        public string Conformation { get; set; }
    }
}
