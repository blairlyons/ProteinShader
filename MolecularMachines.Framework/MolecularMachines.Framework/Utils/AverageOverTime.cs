using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MolecularMachines.Framework.Utils
{
    /// <summary>
    /// Average over a float stream over a specific time period
    /// </summary>
    public class AverageOverTime
    {
        /// <summary>
        /// Create instance.
        /// </summary>
        /// <param name="timeSpan">Maximum time span the values are stored</param>
        /// <param name="capacity">Maximum number of stored values</param>
        public AverageOverTime(TimeSpan timeSpan, int capacity)
        {
            this.timeSpan = timeSpan;
            this.samples = new CircularBuffer<Sample>(capacity);
        }

        private TimeSpan timeSpan;
        private CircularBuffer<Sample> samples;

        private void Clean()
        {
            Sample sample;
            DateTime oldThreshold = DateTime.Now.Add(-timeSpan);

            while (samples.PopPreview(out sample))
            {
                if (sample.dateTime < oldThreshold)
                {
                    // remove old sample
                    samples.Pop();
                }
                else
                {
                    // all old samples removed -> break
                    break;
                }
            }
        }

        /// <summary>
        /// Add a new value now.
        /// </summary>
        /// <param name="value">value</param>
        public void Push(float value)
        {
            samples.Push(new Sample()
            {
                dateTime = DateTime.Now,
                value = value
            });
        }

        /// <summary>
        /// Calculate the average over the samples in the defined timespan.
        /// If the samples exceed the capacity, only the lastest are considered (which is a shorter than the defined timespan)
        /// </summary>
        /// <returns>Avg. If no samples, 0 is returned</returns>
        public float GetAvg()
        {
            float sum = 0;
            int count = 0;

            Clean();

            foreach (Sample s in this.samples)
            {
                sum += s.value;
                count++;
            }

            if (count == 0) { return 0; }
            else
            {
                return sum / count;
            }
        }

        private struct Sample
        {
            public DateTime dateTime;
            public float value;
        }
    }
}
