using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Text.RegularExpressions;

namespace SharpJags.CodaParser
{
	public class Parser
	{
		private List<String> _parameterIndex; 
		private List<List<String>> _codaChains;
		
		private const String IsMatrix = @"([A-Za-z0-9_]+)\[([0-9]+),([0-9]+)\]$";
		private const String IsVector = @"([A-Za-z0-9_]+)\[([0-9]+)\]$";

		public SampleCollection Parse(CodaData data)
		{
			_parameterIndex = data.Index;
			_codaChains = data.Chains;

			var parameters = new Dictionary<String, IModelParameter>();

			foreach (var t in _parameterIndex)
			{
				var chunks = t.Split(' ');

				var parameterIdentifier = chunks[0];
				var startIndex = Int32.Parse(chunks[1]) - 1;
				var endIndex = Int32.Parse(chunks[2]);

				var isMatrix = Regex.Match(parameterIdentifier, IsMatrix);
				var isVector = Regex.Match(parameterIdentifier, IsVector);

				String parameterName;
				int x = 0, y = 0;

				if (isMatrix.Success)
				{
					parameterName = isMatrix.Groups[1].Value;
					x = Int32.Parse(isMatrix.Groups[2].Value) - 1;
					y = Int32.Parse(isMatrix.Groups[3].Value) - 1;
					if (!parameters.ContainsKey(parameterName))
					{
						var mat = new ModelParameterMatrix();
						var vec = new ModelParameterVector();
						var param = new ModelParameter();
						vec.Parameters.Insert(y, param);
						mat.Vectors.Insert(x, vec);
						mat.ParameterName = parameterName;
						parameters.Add(parameterName, mat);
					}
					else
					{
						var mat = (parameters[parameterName] as ModelParameterMatrix);
						if (mat != null)
						{
							var vec = mat.Vectors.ElementAtOrDefault(x);
							if (vec == null)
							{
								var param = new ModelParameter();
								vec = new ModelParameterVector();
								vec.Parameters.Insert(y, param);
								mat.Vectors.Insert(x, vec);
							}
							else
							{
								if (vec.Parameters.ElementAtOrDefault(y) == null)
								{
									vec.Parameters.Insert(y, new ModelParameter());
								}
							}
						}
					}

				}
				else if (isVector.Success)
				{
					parameterName = isVector.Groups[1].Value;
					x = Int32.Parse(isVector.Groups[2].Value) - 1;
					if (!parameters.ContainsKey(parameterName))
					{
						var vec = new ModelParameterVector();
						var param = new ModelParameter();
						vec.Parameters.Insert(x, param);
						vec.ParameterName = parameterName;
						parameters.Add(parameterName, vec);
					}
					else
					{
						var vec = (parameters[parameterName] as ModelParameterVector);
						if (vec != null && vec.Parameters.ElementAtOrDefault(x) == null)
						{
							vec.Parameters.Insert(x, new ModelParameter());
						}
					}
				}
				else
				{
					parameterName = parameterIdentifier;
					if (!parameters.ContainsKey(parameterName))
					{
						var param = new ModelParameter
						{
							ParameterName = parameterName
						};
						
						parameters.Add(parameterName, param);
					}
				}

				ModelParameter parameter = null;
				if (isMatrix.Success)
				{
					var modelParameterMatrix = parameters[parameterName] as ModelParameterMatrix;
					if (modelParameterMatrix != null)
						parameter = modelParameterMatrix.Vectors[x][y];
				}
				else if (isVector.Success)
				{
					var modelParameterVector = parameters[parameterName] as ModelParameterVector;
					if (modelParameterVector != null)
						parameter = modelParameterVector.Parameters[x];
				}
				else
				{
					parameter = (parameters[parameterName] as ModelParameter);
				}

				foreach (var t1 in _codaChains)
				{
					var chain = new ModelChain();
					var chainLines = t1;

					for (var k = startIndex; k < endIndex; k++)
					{
						var line = chainLines[k];
						
						var sampleChunk = line.Split(
							new[]{ ' ' }, 5, StringSplitOptions.RemoveEmptyEntries);
						
						try
						{
							var s = sampleChunk[1];
							var sample = Double.Parse(s, CultureInfo.InvariantCulture);
							chain.Samples.Add(sample);
						}
						catch (Exception)
						{
							Debug.WriteLine("Parser failed to parse sample. Skipping.");
						}
					}

					if (parameter != null) parameter.Chains.Add(chain);
				}
			}

			return new SampleCollection(parameters);
		}
	}
}
