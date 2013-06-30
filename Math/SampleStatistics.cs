using System.Collections.Generic;
using System.Linq;

namespace SharpJags.Math
{
    public class SampleStatistics
    {
        public double Mean { get; private set; }
        public double Variance { get; private set; }
        public double StandardDeviation {
            get { return System.Math.Sqrt(Variance); }
        }

        public int Count { get; private set; }
        public double Min { get; private set; }
        public double Max { get; private set; }

        public SampleStatistics(ICollection<double> samples)
        {
            Count   = samples.Count;
            Min     = samples.Min();
            Max     = samples.Max();
            
            CalculateMean(samples);
            CalculateVariance(samples);
        }

        private void CalculateMean(ICollection<double> samples)
        {
            Mean = samples.Sum() / Count;
        }

        private void CalculateVariance(ICollection<double> samples)
        {
            Variance = samples.Sum(s => System.Math.Pow((s - Mean), 2)) / (Count - 1);
        }
    }
}