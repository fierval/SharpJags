using System.Collections.Generic;
using System.IO;
using SharpJags.Jags;

namespace SharpJags.Processes
{
	public interface IProcessRunner
	{
		ProcessResult Run(IEnumerable<IProcessArgument> arguments, string workingDirectory = null);
	}
}