using System;
using System.Text.RegularExpressions;

namespace SharpJags.Sanitazion
{
	public class OutputSanitizer : IOutputSanitizer
	{
		public string Sanitize(string definition)
		{
			var numberOfLeadingTabs = 0;

			foreach (var t in definition)
			{
				if (t == 9)
				{
					numberOfLeadingTabs++;
					continue;
				}

				if (t == 13 || t == 10)
				{
					continue;
				}

				break;
			}

			return Regex
				.Replace(definition, @"\t{" + numberOfLeadingTabs + "}", String.Empty)
				.Trim();
		}
	}
}