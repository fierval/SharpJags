using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SharpJags.Math
{
	public class Matrix<T> : IEnumerable
	{
		private List<Vector<T>> _backingStorage;

		public int Rows { get; private set; }
		public int Cols { get; private set; }

		public Matrix()
		{
			Initialize(0, 0);
		}

		public Matrix(int rows, int cols)
		{
			Initialize(rows, cols);
			Fill(default(T));
		}

		public Matrix(int rows, int cols, T defaultValue)
		{
			Initialize(rows, cols);
			Fill(defaultValue);
		}

		private void Initialize(int rows, int cols)
		{
			Rows = rows;
			Cols = cols;
			_backingStorage = new List<Vector<T>>(rows);
		}

		private void Fill(T defaultValue)
		{
			for (var i = 0; i < Rows; i++)
			{
				_backingStorage.Add(new Vector<T>(Cols, defaultValue));
			}
		}

		public Vector<T> this[int row]
		{
			get { return _backingStorage[row]; }
		}

		public Vector<T> ToColumnVector()
		{
			return new Vector<T>(_backingStorage
				.SelectMany(v => v.ToList())
				.ToList());
		}

		public void Add(Vector<T> vector)
		{
			if (Cols == 0)
				Cols = vector.Length;
			if(vector.Length != Cols)
				throw new ArgumentException("Dimension mismatch.");
			
			_backingStorage.Add(vector);
			Rows++;
		}

		public IEnumerator GetEnumerator()
		{
			return _backingStorage.GetEnumerator();
		}
	}
}
