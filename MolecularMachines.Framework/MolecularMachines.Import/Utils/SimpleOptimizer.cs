using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MolecularMachines.Import.Utils
{
    class SimpleOptimizer
    {
        /// <summary>
        /// O(steps^dimensions)
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <param name="steps"></param>
        /// <param name="costFunction"></param>
        public SimpleOptimizer(double[] min, double[] max, int steps, Func<double[], double> costFunction)
        {
            // asserts
            if (min == null) { throw new ArgumentNullException(nameof(min)); }
            if (max == null) { throw new ArgumentNullException(nameof(max)); }
            if (min.Length != max.Length) { throw new ArgumentException($"Length of {nameof(min)} and {nameof(min)} must equal"); }

            // store
            this.min = min;
            this.max = max;
            this.costFunction = costFunction;

            // dimensions
            this.dimensions = this.min.Length;

            // calculate delta
            this.delta = new double[this.dimensions];
            for (int i = 0; i < this.dimensions; i++)
            {
                this.delta[i] = (this.max[i] - this.min[i]) / (double)steps;
            }
        }

        private int dimensions;
        private double[] min;
        private double[] max;
        private double[] delta;
        private Func<double[], double> costFunction;
        private int tests = 0;

        private double[] bestSample = null;
        private double bestSampleCost = double.MaxValue;

        public int Tests { get { return this.tests; } }
        public double[] BestSample { get { return this.bestSample; } }
        public double BestSampleCost { get { return this.bestSampleCost; } }

        public void Optimize()
        {
            var sample = new double[this.dimensions];
            TestDimension(0, sample);
        }

        private void TestDimension(int dimension, double[] sample)
        {
            if (dimension >= this.dimensions)
            {
                var cost = costFunction(sample);
                if (cost < this.bestSampleCost)
                {
                    this.bestSampleCost = cost;
                    this.bestSample = CloneSample(sample);
                }

                this.tests++;
            }
            else
            {
                var nextDimension = dimension + 1;

                for (sample[dimension] = this.min[dimension]; sample[dimension] < this.max[dimension]; sample[dimension] += this.delta[dimension])
                {
                    TestDimension(nextDimension, sample);
                }
            }
        }

        private double[] CloneSample(double[] sample)
        {
            var clone = new double[sample.Length];
            for (int i = 0; i < sample.Length; i++)
            {
                clone[i] = sample[i];
            }
            return clone;
        }
    }
}
