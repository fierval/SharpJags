using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SharpJags.Parsing.Coda
{
	public class CodaDataReader : ISampleReader
	{
		private List<String> ReadFileLineByLine(String path)
		{
			return File.ReadAllLines(path).ToList();
		}

		public SampleData Read(string basePath, string indexFileName, string chainFileNameTemplate, int numChains)
		{
			var indexPath = Path.Combine(basePath, indexFileName);
			var index = ReadFileLineByLine(indexPath);
			var chains = new List<List<String>>();
			for (var i = 0; i < numChains; i++)
			{
				var chainPath = String.Format(chainFileNameTemplate, i);
				chains.Add(ReadFileLineByLine(chainPath));
			}

			return new SampleData
			{
				Index = index,
				Chains = chains
			};
		}
	}
}
