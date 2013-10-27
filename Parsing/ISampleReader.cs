namespace SharpJags.Parsing
{
	public interface ISampleReader
	{
		SampleData Read(string basePath, string indexFileName, string chainFileNameTemplate, int numChains);
	}
}