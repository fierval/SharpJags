using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpJags.Math
{
	public class Vector<T> : IEnumerable
	{
		private List<T> _backingStorage;

		public int Length { get; private set; }

		public Vector()
		{
			Initialize(0);
		}

		public Vector(int length)
		{
			Initialize(length);
			Fill(default(T));
		}
			
		public Vector(int length, T defaultValue)
		{
			Initialize(length);
			Fill(defaultValue);
		}

		public Vector(List<T> list)
		{
			Length = list.Count;
			_backingStorage = list.ToList();
		}

		private void Initialize(int length)
		{
			Length = length;
			_backingStorage = new List<T>();
		}
        
		private void Fill(T defaultValue)
		{
			for (var i = 0; i < Length; i++)
			{
				_backingStorage.Add(defaultValue);
			}
		}

		public List<T> ToList()
		{
			return _backingStorage.ToList();
		}

		public T this[int column]
		{
			get { return _backingStorage[column]; }
			set { _backingStorage[column] = value; }
		}

		public void Add(T data)
		{
			_backingStorage.Add(data);
			Length++;
		}

		public IEnumerator GetEnumerator()
		{
			return _backingStorage.GetEnumerator();
		}
	}
}
