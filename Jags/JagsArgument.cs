namespace SharpJags.Jags
{
	public class JagsArgument : IJagsArgument
	{
		private readonly string _argument;

		public JagsArgument(string argument)
		{
			_argument = argument;
		}

		public string ToFormattedString()
		{
			return "\"" + _argument + "\"";
		}
	}
}