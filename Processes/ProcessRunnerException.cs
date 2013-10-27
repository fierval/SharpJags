using System;

namespace SharpJags.Processes
{
	public class ProcessRunnerException : Exception
	{
		public ProcessRunnerException(string message) : base(message) { }
	}
}