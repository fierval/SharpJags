using System;
using System.Collections.Generic;
using SharpJags.Math;

namespace SharpJags.Parsing
{
	public class ModelChain
	{
		public readonly List<double> Samples = new List<double>();

		private SampleStatistics _statistics;
		public SampleStatistics Statistics
		{
			get { return _statistics ?? (_statistics = new SampleStatistics(Samples)); }
		}
	}
}
