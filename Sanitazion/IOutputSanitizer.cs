namespace SharpJags.Sanitazion
{
	public interface IOutputSanitizer
	{
		string Sanitize(string definition);
	}
}