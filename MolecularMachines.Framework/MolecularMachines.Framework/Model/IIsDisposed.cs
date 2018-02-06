using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MolecularMachines.Framework.Model
{
    public interface IIsDisposed : IDisposable
    {
        bool IsDisposed { get; }
    }
}
