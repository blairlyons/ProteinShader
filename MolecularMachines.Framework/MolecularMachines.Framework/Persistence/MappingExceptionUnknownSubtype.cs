using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MolecularMachines.Framework.Persistence
{
    class MappingExceptionUnknownSubtype : MappingException
    {
        public MappingExceptionUnknownSubtype(Type superType, Type type) :
            base("Unknown subtype of " + superType.Name + ": " + type.Name)
        {

        }
    }
}
