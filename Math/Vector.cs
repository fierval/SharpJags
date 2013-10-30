using System.Collections.Generic;

namespace SharpJags.Math
{
	public class Vector<T>
	{
		private T[] _storage;

		public int Length { get; private set; }

		public Vector(int length)
		{
			Initialize(new T[length]);
			Fill(default(T));
		}
			
		public Vector(T[] arr)
		{
			Initialize(arr);
		}

		private void Initialize(T[] storage)
		{
			_storage = storage;

			Length = _storage.GetLength(0);
		}

		private void Fill(T defaultValue)
		{
			for (var i = 0; i < Length; i++)
			{
				_storage[i] = defaultValue;
			}
		}

		public T Get(int x)
		{
			return _storage[x];
		}

		public void Set(int x, T value)
		{
			_storage[x] = value;
		}

		public T this[int x]
		{
			get
			{
				return Get(x);
			}

			set
			{
				Set(x, value);
			}
		}

		public IEnumerable<T> AsEnumerable()
		{
			return _storage;
		}

		public static implicit operator Vector<T>(T[] arr)
		{
			return arr == null ? null : new Vector<T>(arr);
		}
	}
}
