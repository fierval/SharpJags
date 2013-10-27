using System;
using SharpJags.Math;

namespace SharpJags.Parsing
{
	public interface IModelParameter
	{
		String ParameterName { get; }
		CompositeSampleStatistics Statistics { get; }
	}
}