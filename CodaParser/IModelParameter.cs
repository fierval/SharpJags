using System;
using SharpJags.Math;

namespace SharpJags.CodaParser
{
	public interface IModelParameter
	{
		String ParameterName { get; }
		CompositeSampleStatistics Statistics { get; }
	}
}