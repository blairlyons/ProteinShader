using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MolecularMachines.Framework.Model
{
    /// <summary>
    /// Is implemented by classes that are needed to be updated every frame.
    /// The parent object must call <see cref="Update"/> every frame.
    /// </summary>
    public interface IFrameUpdate
    {
        /// <summary>
        /// Called every frame
        /// </summary>
        void Update();
    }
}
