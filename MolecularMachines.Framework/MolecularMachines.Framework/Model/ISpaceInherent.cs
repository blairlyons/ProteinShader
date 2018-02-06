using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MolecularMachines.Framework.Model
{
    public interface ISpaceInherent
    {
        LocRot LocRot { get; }
        bool IsAlive { get; }
    }
}
