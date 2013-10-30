using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SharpJags.Jags;
using SharpJags.Parsing;

namespace SharpJags.Collections
{
	public class SampleCollection
	{
		private readonly Dictionary<String, IModelParameter> _sampleDictionary;

		public SampleCollection(Dictionary<String, IModelParameter> sampleDictionary)
		{
			_sampleDictionary = sampleDictionary;
		}

		public IList<IModelParameter> All()
		{
			return _sampleDictionary.Select(s => s.Value).ToList();
		}

		public T Get<T>(string parameterName) where T : class, IModelParameter
		{
			if (!_sampleDictionary.ContainsKey(parameterName))
				throw new KeyNotFoundException("Model parameter not found. Be sure to create a monitor for it.");

			var parameter = _sampleDictionary[parameterName];
			var returnParameter = parameter as T;

			if (returnParameter == null)
			{
				throw new InvalidDataException(
					"The model parameter is found, but is not of the specified type. Correct type is: " + parameter.GetType());
			}

			return returnParameter;
		}

		public T Get<T>(JagsMonitor monitor) where T : class, IModelParameter
		{
			return Get<T>(monitor.ParameterName);
		}
	}
}