using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MolecularMachines.Framework.Logging
{
    /// <summary>
    /// Exposes <see cref="Info(string)"/>, <see cref="Warn(string)"/> and <see cref="Error(string)"/> methods to write in to a concrete log.
    /// </summary>
    public interface ILogWriter
    {
        void Info(string message);
        void Warn(string message);
        void Error(string message);
    }
}
