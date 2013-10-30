using System.Collections.Generic;

namespace SharpJags.Math
{
	public class Matrix<T>
	{
		public int Rows { get; private set; }
		public int Cols { get; private set; }

		private T[,] _storage;

		public Matrix(int rows, int cols)
		{
			Initialize(new T[rows, cols]);
			Fill(default(T));
		}

		public Matrix(T[,] arr)
		{
			Initialize(arr);
		} 

		private void Initialize(T[,] storage)
		{
			_storage = storage;
			
			Rows = _storage.GetLength(0);
			Cols = _storage.GetLength(1);
		}

		private void Fill(T defaultValue)
		{
			for (var i = 0; i < Rows; i++)
			{
				for (var j = 0; j < Cols; j++)
				{
					_storage[i, j] = defaultValue;
				}
			}
		}

		public T Get(int y, int x)
		{
			return _storage[y, x];
		}

		public void Set(int y, int x, T value)
		{
			_storage[y, x] = value;
		}

		public T this[int y, int x]
		{
			get
			{
				return Get(y, x);
			}

			set
			{
				Set(y, x, value);
			}
		}

		public Vector<T> Row(int index)
		{
			var vector = new T[Cols];
			for (var i = 0; i < Cols; i++)
			{
				vector[i] = _storage[index, i];
			}

			return vector;
		}

		public Vector<T> Col(int index)
		{
			var vector = new T[Rows];
			for (var i = 0; i < Rows; i++)
			{
				vector[i] = _storage[i, index];
			}

			return vector;
		}

		public Vector<T> ToColumnVector()
		{
			var vector = new List<T>();
			for (var i = 0; i < Rows; i++)
			{
				vector.AddRange(Row(i).AsEnumerable());
			}

			return vector.ToArray();
		}

		public static implicit operator Matrix<T>(T[,] arr)
		{
			return arr == null ? null : new Matrix<T>(arr);
		}
	}
}
