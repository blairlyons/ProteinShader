using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MolecularMachines.Framework.Model.Markers
{
    /// <summary>
    /// State of a <see cref="BindingSite"/>.
    /// </summary>
    public enum BindingState
    {
        /// <summary>
        /// Nothing is bound.
        /// </summary>
        Free,

        /// <summary>
        /// The <see cref="BindingSite"/> is currently binding to another <see cref="BindingSite"/>.
        /// It is currently too far away. It marks a <see cref="BindingSite"/> as already occupied.
        /// </summary>
        Binding,

        /// <summary>
        /// The <see cref="BindingSite"/> is bound to another <see cref="BindingSite"/>.
        /// </summary>
        Bound
    }
}
