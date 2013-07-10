using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SharpJags.CodaParser
{
	public static class CodaDataReader
	{
		private static List<String> ReadFileLineByLine(String path)
		{
			return File.ReadAllLines(path).ToList();
		}

		public static CodaData Read(FileSystemInfo basePath, String indexFileName, String chainFileNameTemplate, int numChains)
		{
			var indexPath = String.Format("{0}/{1}", basePath.FullName, indexFileName);
			var index = ReadFileLineByLine(indexPath);
			var chains = new List<List<String>>();
			for (var i = 0; i < numChains; i++)
			{
				var chainPath = String.Format(chainFileNameTemplate, i);
				chains.Add(ReadFileLineByLine(chainPath));
			}

			return new CodaData
			{
				Index = index,
				Chains = chains
			};
		}
	}

	public class CodaData
	{
		public List<String> Index { get; set; }
		public List<List<String>> Chains { get; set; }
	}
}
