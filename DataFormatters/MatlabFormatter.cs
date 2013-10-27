using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using SharpJags.Math;

namespace SharpJags.DataFormatters
{
	public class MatlabFormatter : IDataFormatter
	{
		public FormattedData Format(Dictionary<string, object> data)
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

			return new FormattedData
			{
				FileExtension = "m",
				Data = sb.ToString()
			};
		}

		private string Comment(string comment)
		{
			return comment.Replace('#', '%');
		}

		private string Declare(string identifier, object value)
		{
			var sb = new StringBuilder("jagsDataStruct." + identifier + " = ");

			if (value is Matrix<double>) sb.Append(ConvertMatrix(value as Matrix<double>));
			else if (value is Vector<double>) sb.Append(ConvertVector(value as Vector<double>));
			else sb.Append(ConvertSimpleValue(value));

			sb.Append(";");

			return sb.ToString();
		}

		private string ConvertSimpleValue(object value)
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

		private string ConvertVector(Vector<double> vector, bool child = false)
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

		private string ConvertMatrix(Matrix<double> matrix)
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
