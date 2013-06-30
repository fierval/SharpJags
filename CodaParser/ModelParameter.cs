using System;
using System.Collections.Generic;
using System.Linq;
using SharpJags.Math;

namespace SharpJags.CodaParser
{
    public class ModelParameter : IModelParameter
    {
        public String ParameterName { get; set; }
        public readonly List<ModelChain> Chains = new List<ModelChain>();

        public List<Double> Samples
        {
            get 
            { 
                return Chains.SelectMany(m => m.Samples).ToList(); 
            }
        }

        private CompositeSampleStatistics _statistics;
        public CompositeSampleStatistics Statistics
        {
            get 
            { 
                return _statistics ?? (_statistics = new CompositeSampleStatistics(Chains)); 
            }
        }

        public ModelChain this[int key]
        {
            get
            {
                return Chains[key];
            }
        }
    }
}
