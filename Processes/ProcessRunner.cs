using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using SharpJags.Jags;

namespace SharpJags.Processes
{
	public class ProcessRunner : IProcessRunner
	{
		private readonly FileInfo _processPath;

		public ProcessRunner(FileInfo processPath)
		{
			_processPath = processPath;
		}

		private string BuildArgumentString(IEnumerable<IProcessArgument> arguments)
		{
			return arguments == null ? String.Empty : String.Join(" ", arguments.Select(a => a.ToFormattedString())).Trim();
		}

		public ProcessResult Run(IEnumerable<IProcessArgument> arguments, string workingDirectory = null)
		{
			if (!String.IsNullOrEmpty(workingDirectory))
			{
				Directory.SetCurrentDirectory(workingDirectory);	
			}
			
			var process = new Process
			{
				StartInfo = new ProcessStartInfo
				{
					FileName = _processPath.FullName,
					Arguments = BuildArgumentString(arguments),
					CreateNoWindow = true,
					UseShellExecute = false,
					RedirectStandardError = true,
					RedirectStandardOutput = true
				}
			};

			string result;
			string errors;

			try
			{
				process.Start();

				result = process.StandardOutput.ReadToEnd();
				errors = process.StandardError.ReadToEnd();

				process.WaitForExit();
			}
			catch (Exception e)
			{
				throw new ProcessRunnerException(
					String.Format("The process exited unexpectedely with message: {0}", e.Message));
			}
			finally
			{
				process.Dispose();
			}

			return new ProcessResult
			{
				Output = result,
				Errors = errors
			};
		}
	}
}