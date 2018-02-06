using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MolecularMachines.Framework.DataStructures
{
    /// <summary>
    /// Immutable List. Similar to an array, but the items can not be changed.
    /// </summary>
    /// <typeparam name="T">Type of the items</typeparam>
    public class ImmutableList<T> : IEnumerable<T>
    {
        public ImmutableList(IEnumerable<T> source)
        {
            if (source == null)
            { this.items = new T[0]; }
            else
            { this.items = source.ToArray(); }
        }

        private T[] items;

        /// <summary>
        /// Access the item at a specified index.
        /// </summary>
        /// <param name="index">index</param>
        /// <returns>item at index</returns>
        public T this[int index]
        {
            get { return items[index]; }
        }

        /// <summary>
        /// Number of items in this <see cref="ImmutableList{T}"/>.
        /// </summary>
        public int Count { get { return items.Length; } }

        public IEnumerator<T> GetEnumerator()
        {
            return ((IEnumerable<T>)items).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
