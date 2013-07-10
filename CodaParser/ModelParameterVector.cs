using System;
using SharpJags.Math;
using System.Collections.Generic;
using System.Linq;

namespace SharpJags.CodaParser
{
	public class ModelParameterVector : IModelParameter
	{
		public String ParameterName { get; set; }
		 
		public readonly List<ModelParameter> Parameters;

		public ModelParameterVector()
		{
			Parameters = new List<ModelParameter>();
		}

		public ModelParameter this[int key]
		{
			get
			{
				return Parameters[key];
			}
		}

		private CompositeSampleStatistics _statistics;
		public CompositeSampleStatistics Statistics
		{
			get
			{
				return _statistics ?? (
					_statistics = new CompositeSampleStatistics(
						Parameters.SelectMany(p => p.Chains).ToList()));
			}
		}
	}
}
