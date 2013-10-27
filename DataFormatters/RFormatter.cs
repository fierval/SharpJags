using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using SharpJags.Math;

namespace SharpJags.DataFormatters
{
	public class RFormatter : IDataFormatter
	{
		public FormattedData Format(Dictionary<string, object> data)
		{
			var sb = new StringBuilder();
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
				FileExtension = "r",
				Data = sb.ToString()
			};
		}

		private string Comment(string comment)
		{
			return comment;
		}
		
		private string Declare(string identifier, object value)
		{
			var sb = new StringBuilder(identifier + " <- ");

			if (value is Matrix<double>) sb.Append(ConvertMatrix(value as Matrix<double>));
			else if (value is Vector<double>) sb.Append(ConvertVector(value as Vector<double>));
			else sb.Append(ConvertSimpleValue(value));

			return sb.ToString();
		}
		
		private string ConvertSimpleValue(object value) {
			if(value is Double) 
			{
				var val = (double)value;
				return Double.IsNaN(val) ? "NA" : val.ToString(CultureInfo.InvariantCulture);
			}
			if (value is Int32)
			{
				return value.ToString();
			}

			return "'" + (value as String) + "'";
		}
		
		private string ConvertVector(Vector<double> vector)
		{
			var sb = new StringBuilder("c(");

			for (var i = 0; i < vector.Length; i++)
			{
				sb.Append(ConvertSimpleValue(vector.Get(i)));
				if (i != (vector.Length - 1))
					sb.Append(",");
			}

			sb.Append(")");

			return sb.ToString();
		}

		private string ConvertMatrix(Matrix<double> matrix)
		{
			var sb = new StringBuilder("structure(")
			
			.Append(ConvertVector(
						matrix.ToColumnVector()))

			.Append(", ")
			.Append(".Dim=c(" + matrix.Rows + "," + matrix.Cols + ")")
			.Append(")");

			return sb.ToString();
		}
	}
}
