using SharpJags.Collections;

namespace SharpJags.Parsing
{
	public interface ISampleParser
	{
		SampleCollection Parse(string rootPath, string indexFileName, string chainFileNameTemplate, int numChains);
	}
}