using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MolecularMachines.Framework.Utils
{
    public class CircularBuffer<T> : IEnumerable<T>
    {
        public CircularBuffer(int capacity)
        {
            this.buffer = new T[capacity];

            this.writeIndex = 0;
            this.readIndex = 0;
            this.count = 0;
        }

        private T[] buffer;

        private int writeIndex;
        private int readIndex;
        private int count;

        public void Push(T item)
        {
            this.count++;
            if (this.count > this.buffer.Length)
            {
                // write index caught up with read index
                OnWriteOverflow();
            }

            this.buffer[this.writeIndex] = item;

            this.writeIndex = (this.writeIndex + 1) % this.buffer.Length;
        }

        public bool PopPreview(out T item)
        {
            if (this.count == 0)
            {
                item = default(T);
                return false;
            }
            else
            {
                item = this.buffer[this.readIndex];
                return true;
            }
        }

        public bool Pop(out T item)
        {
            if (this.PopPreview(out item))
            {
                this.readIndex = (this.readIndex + 1) % buffer.Length;
                this.count--;

                return true;
            }
            else
            { return false; }
        }

        public T Pop()
        {
            T item;
            if (!this.Pop(out item))
            {
                throw new Exception("buffer underflow");
            }

            return item;
        }

        protected virtual void OnWriteOverflow()
        {
            // just read one out to keep the buffer working.
            // one could also throw an exception instead
            Pop();
        }

        public IEnumerator<T> GetEnumerator()
        {
            for (int i = 0; i < this.count; i++)
            {
                var bufferIndex = (this.readIndex + i) % this.buffer.Length;
                yield return this.buffer[bufferIndex];
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<T>)this).GetEnumerator();
        }
    }
}
