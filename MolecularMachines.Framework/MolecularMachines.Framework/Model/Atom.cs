using MolecularMachines.Framework.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MolecularMachines.Framework.Model
{
    /// <summary>
    /// Represents an Atom identity in an <see cref="EntityClass"/>.
    /// </summary>
    public class Atom
    {
        public Atom(Element element)
        {
            this.Element = element;
        }

        public Element Element { get; private set; }
    }
}
