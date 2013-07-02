using System;
using System.Collections.Generic;
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

		public T Get<T>(String key) where T : class, IModelParameter
		{
			if(!_sampleDictionary.ContainsKey(key))
				throw new KeyNotFoundException("Model parameter not found. Be sure to create a monitor for it.");
			
			return _sampleDictionary[key] as T;
		}
	}
}