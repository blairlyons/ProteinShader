using MolecularMachines.Framework.DataStructures;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MolecularMachines.Framework.DataStructures
{
    /// <summary>
    /// Linked list which allows to remove elements during iteration.
    /// </summary>
    public class IterateList<T> : IEnumerable<T>
    {
        public IterateList()
        {

        }

        /// <summary>
        /// Enumerator for <see cref="IterateList{T}"/>.
        /// </summary>
        public class Enumerator : IEnumerator<T>
        {
            /// <summary>
            /// Creates a new enumerator.
            /// </summary>
            /// <param name="list">List that is traversed and manipulated.</param>
            public Enumerator(IterateList<T> list)
            {
                this.list = list;
                this.Reset(); // also sets first=true
            }

            private IterateList<T> list;
            private IterateListNode<T> currentNode;
            private IterateListNode<T> previousNode;
            private bool removed = false;
            private bool first;

            public void Reset()
            {
                this.first = true;
                this.currentNode = null;
            }

            public void Dispose()
            {
                this.list = null;
                this.currentNode = null;
                this.previousNode = null;
            }

            public bool MoveNext()
            {
                if (first)
                {
                    this.currentNode = list.root;
                    this.previousNode = null;
                    this.first = false;
                }
                else
                {
                    if (removed)
                    {
                        // the current item was removed by the controller,
                        // therefore the previousNode for the nextNode stays the same
                        // -> nothing to do here
                    }
                    else
                    {
                        // the current item was NOT removed by the controller,
                        // therefore the previousNode for the nextNode is the old currentNode
                        this.previousNode = this.currentNode;
                    }

                    this.currentNode = this.currentNode.Next;
                }

                this.removed = false;

                return (this.currentNode != null);
            }

            public T Current
            {
                get { return this.currentNode.Value; }
            }

            object IEnumerator.Current { get { return this.Current; } }

            /// <summary>
            /// Removes the current node
            /// </summary>
            public void Remove()
            {
                if (!removed)
                {
                    if (previousNode == null)
                    {
                        // there is no node before -> this is the root node
                        list.root = currentNode.Next;
                    }
                    else
                    {
                        // cut this node out
                        previousNode.Next = currentNode.Next;
                    }

                    if (currentNode == list.last)
                    {
                        list.last = previousNode;
                    }

                    list.Count--;

                    removed = true;
                }
                else
                {
                    throw new Exception("this node was already removed");
                }
            }
        }

        private IterateListNode<T> root = null;
        private IterateListNode<T> last = null;

        public int Count { get; set; }

        public void Clear()
        {
            this.root = null;
            this.last = null;
            Count = 0;
        }

        public void Add(T item)
        {
            if (last == null)
            {
                root = new IterateListNode<T>(item);
                last = root;
            }
            else
            {
                var newNode = new IterateListNode<T>(item);
                last.Next = newNode;
                last = newNode;
            }

            Count++;
        }

        public void Remove(T item)
        {
            using (var enumerator = GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    if (object.Equals(item, enumerator.Current))
                    {
                        enumerator.Remove();
                        return;
                    }
                }
            }
        }

        public Enumerator GetEnumerator()
        {
            return new Enumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
