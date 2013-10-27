using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SharpJags.DataFormatters;

namespace SharpJags.Jags
{
	public class JagsData : IEnumerable
	{
		private readonly Dictionary<String, Object> _storage;
		private readonly List<IDataFormatter> _outputFormatters;

		public JagsData(IEnumerable<IDataFormatter> outputFormatters = null)
		{
			_storage = new Dictionary<String, Object>();
			
			_outputFormatters = new List<IDataFormatter>
			{
				new RFormatter()
			};

			if (outputFormatters != null)
			{
				_outputFormatters = _outputFormatters.Union(outputFormatters).ToList();
			}
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

		public void Add(string key, object data)
		{
			_storage.Add(key, data);
		}

		public IEnumerable<FormattedData> GetFormattedData()
		{
			return _outputFormatters.Select(formatter => formatter.Format(_storage));
		}
	}
}
