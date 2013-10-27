using SharpJags.Jags;

namespace SharpJags.Processes
{
	public class ProcessArgument : IProcessArgument
	{
		private readonly string _formattedArgument;

		public ProcessArgument(string formattedArgument)
		{
			_formattedArgument = formattedArgument;
		}

		public string ToFormattedString()
		{
			return _formattedArgument;
		}
	}
}