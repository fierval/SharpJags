using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpJags.Math;

namespace SharpJags.CodaParser
{
    public class ModelChain
    {
        public readonly List<Double> Samples = new List<Double>();

        private SampleStatistics _statistics;
        public SampleStatistics Statistics
        {
            get { return _statistics ?? (_statistics = new SampleStatistics(Samples)); }
        }
    }
}
