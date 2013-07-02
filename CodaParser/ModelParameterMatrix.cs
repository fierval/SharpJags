using SharpJags.Math;
using System.Collections.Generic;
using System.Linq;

namespace SharpJags.CodaParser
{
	public class ModelParameterMatrix : IModelParameter
	{
		public readonly List<ModelParameterVector> Vectors;

		public ModelParameterMatrix()
		{
			Vectors = new List<ModelParameterVector>();
		}

		public ModelParameterVector this[int key]
		{
			get
			{
				return Vectors[key];
			}
		}

		private CompositeSampleStatistics _statistics;
		public CompositeSampleStatistics Statistics
		{
			get
			{
				return _statistics ?? (
					_statistics = new CompositeSampleStatistics(
						Vectors.SelectMany(v => 
							v.Parameters.SelectMany(p => p.Chains)).ToList()));
			}
		}
	}
}
