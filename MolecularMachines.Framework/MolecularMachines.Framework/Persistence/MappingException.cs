using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MolecularMachines.Framework.Persistence
{
    class MappingException : Exception
    {
        public MappingException(string message) : base(message) { }
    }
}
