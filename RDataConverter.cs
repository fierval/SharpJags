using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using SharpJags.Math;

namespace SharpJags
{
	public static class RDataConverter
	{
		public static String Dump(Dictionary<String, Object> data)
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

			return sb.ToString();
		}

		private static String Comment(String comment)
		{
			return comment;
		}
		
		private static String Declare(String identifier, Object value)
		{
			var sb = new StringBuilder(identifier + " <- ");

			if (value is Matrix<double>) sb.Append(ConvertMatrix(value as Matrix<double>));
			else if (value is Vector<double>) sb.Append(ConvertVector(value as Vector<double>));
			else sb.Append(ConvertSimpleValue(value));

			return sb.ToString();
		}
		
		private static String ConvertSimpleValue(Object value) {
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
		
		private static String ConvertVector(Vector<double> vector)
		{
			var sb = new StringBuilder("c(");

			for (var i = 0; i < vector.Length; i++)
			{
				sb.Append(ConvertSimpleValue(vector[i]));
				if (i != (vector.Length - 1))
					sb.Append(",");
			}

			sb.Append(")");

			return sb.ToString();
		}

		private static String ConvertMatrix(Matrix<double> matrix)
		{
			var sb = new StringBuilder("structure(")
			
			.Append(
				ConvertVector(
						matrix.ToColumnVector()
				)
			)

			.Append(", ")
			.Append(".Dim=c(" + matrix.Rows + "," + matrix.Cols + ")")
			.Append(")");

			return sb.ToString();
		}
	}
}
