using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpJags.Math.Random
{
	public static class DistributionExtensions
	{
		public static IEnumerable<double> Generate(this IDistribution distribution, int n)
		{
			var list = new List<double>();
			for (var i = 0; i < n; i++)
			{
				list.Add(
					distribution.NextDouble());
			}

			return list;
		}
	}
}
