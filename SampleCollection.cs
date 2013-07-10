using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using SharpJags.CodaParser;

namespace SharpJags
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
			return _sampleDictionary
				.Select(s => s.Value)
					.ToList();
		}

		public T Get<T>(JagsMonitor monitor) where T : class, IModelParameter
		{
			if (!_sampleDictionary.ContainsKey(monitor.ParameterName))
				throw new KeyNotFoundException("Model parameter not found. Be sure to create a monitor for it.");

			var parameter = _sampleDictionary[monitor.ParameterName];
			var returnParameter = parameter as T;

			if(returnParameter == null)
				throw new InvalidDataException("The model parameter is found, but is not of the specified type. Correct type is: " + parameter.GetType());

			return returnParameter;
		}
	}
}