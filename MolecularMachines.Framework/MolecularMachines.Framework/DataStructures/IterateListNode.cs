using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MolecularMachines.Framework.DataStructures
{
    /// <summary>
    /// A node that is internally used in an <see cref="IterateList{T}"/>.
    /// </summary>
    /// <typeparam name="T">Type of the value</typeparam>
    public class IterateListNode<T>
    {
        public IterateListNode(T value, IterateListNode<T> next)
        {
            this.Value = value;
            this.Next = next;
        }

        public IterateListNode(T value) : this(value, null)
        { }

        public T Value { get; private set; }
        public IterateListNode<T> Next { get; set; }

        public override string ToString()
        {
            return (this.Value == null ? "null" : this.Value.ToString()) + " -> " + (this.Next == null ? "null" : "next");
        }
    }
}
