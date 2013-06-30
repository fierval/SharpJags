using System;
using System.Collections;
using System.Collections.Generic;

namespace SharpJags
{
	public class JagsData : IEnumerable
	{
		private readonly Dictionary<String, Object> _storage;

		public JagsData()
		{
			_storage = new Dictionary<String, Object>();
		}

		public String DumpR()
		{
			return RDataConverter.Dump(_storage);
		}

		public String DumpMatlab() 
		{
			return MatlabDataConverter.Dump(_storage);
		}

		public Object this[String key]
		{
			get { return Get(key); }
			set { Set(key, value); }
		}

		private void Set(string key, object value)
		{
			_storage[key] = value;
		}

		private object Get(string key)
		{
			return _storage[key];
		}

		public IEnumerator GetEnumerator()
		{
			return _storage.GetEnumerator();
		}

		public void Add(String key, Object data)
		{
			_storage.Add(key, data);
		}
	}
}
