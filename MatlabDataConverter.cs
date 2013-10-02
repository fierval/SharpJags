using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using SharpJags.Math;

namespace SharpJags
{
	public static class MatlabDataConverter
	{
		public static String Dump(Dictionary<String, Object> data)
		{
			var sb = new StringBuilder();

			sb.AppendLine("jagsDataStruct = struct();");

			foreach (var pair in data)
			{
				if (pair.Key.StartsWith("#") && pair.Value == null)
				{
					sb.AppendLine(Comment(pair.Key));
				}
				else
				{
					sb.AppendLine(Declare(pair.Key, pair.Value));
				}
			}

			return sb.ToString();
		}

		private static String Comment(String comment)
		{
			return comment.Replace('#', '%');
		}

		private static String Declare(String identifier, Object value)
		{
			var sb = new StringBuilder("jagsDataStruct." + identifier + " = ");

			if (value is Matrix<double>) sb.Append(ConvertMatrix(value as Matrix<double>));
			else if (value is Vector<double>) sb.Append(ConvertVector(value as Vector<double>));
			else sb.Append(ConvertSimpleValue(value));

			sb.Append(";");

			return sb.ToString();
		}

		private static String ConvertSimpleValue(Object value)
		{
			if (value is Double)
			{
				var val = (double)value;
				return Double.IsNaN(val) ? "NaN" : val.ToString(CultureInfo.InvariantCulture);
			}
			if (value is Int32)
			{
				return value.ToString();
			}

			return "'" + (value as String) + "'";
		}

		private static String ConvertVector(Vector<double> vector, Boolean child = false)
		{
			var sb = new StringBuilder("");

			if (!child) sb.Append("[");

			for (var i = 0; i < vector.Length; i++)
			{
				sb.Append(ConvertSimpleValue(vector.Get(i)));
				if (i != (vector.Length - 1))
					sb.Append(",");
			}

			if (!child) sb.Append("]");

			return sb.ToString();
		}

		private static String ConvertMatrix(Matrix<double> matrix)
		{
			var sb = new StringBuilder("[");

			for (var i = 0; i < matrix.Rows; i++)
			{
				sb.Append(ConvertVector(matrix.Row(i), true));
				if (i != (matrix.Rows - 1))
					sb.Append(";");
			}

			sb.Append("]");

			return sb.ToString();
		}
	}
}
