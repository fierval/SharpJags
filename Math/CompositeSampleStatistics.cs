using System;
using System.Collections.Generic;
using System.Linq;
using SharpJags.CodaParser;

namespace SharpJags.Math
{
    public class CompositeSampleStatistics
    {
        public int Count { get; private set; }
        public Double Min { get; private set; }
        public Double Max { get; private set; }
        public Double Mean { get; private set; }
        public Double Variance { get; private set; }
        public Double StandardDeviation { get; private set; }

        public CompositeSampleStatistics(List<ModelChain> chains)
        {
            CalculateCompositeStatistics(chains);
        }

        private void CalculateCompositeStatistics(List<ModelChain> chains)
        {
            var meanSum = 0d;
            var sampleSum = 0;

            var mins = new List<Double>();
            var maxs = new List<Double>();

            foreach (var chain in chains)
            {
                mins.Add(chain.Statistics.Min);
                maxs.Add(chain.Statistics.Max);
                sampleSum += chain.Statistics.Count;
                meanSum += chain.Statistics.Count * chain.Statistics.Mean;
            }

            var grandMean = meanSum / sampleSum;
            var ess = 0d;
            var tgss = 0d;

            foreach (var chain in chains)
            {
                ess += chain.Statistics.Variance * (chain.Statistics.Count - 1);
                tgss += System.Math.Pow((chain.Statistics.Mean - grandMean), 2) * chain.Statistics.Count;
            }

            var grandVariance = (ess + tgss) / (sampleSum - 1);

            Count = sampleSum;
            Min = mins.Min();
            Max = maxs.Max();
            Mean = grandMean;
            Variance = grandVariance;
            StandardDeviation = System.Math.Sqrt(grandVariance);
        }
    }
}
