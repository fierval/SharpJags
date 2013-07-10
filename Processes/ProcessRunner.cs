using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace SharpJags.Processes
{
	public class ProcessRunner : IDisposable
	{
		private readonly Process _process;
		private readonly FileInfo _fileArgument;
		private readonly Dictionary<String, String> _arguments;

		public ProcessRunner(FileSystemInfo processPath, FileInfo fileArgument, Dictionary<String, String> arguments = null)
		{
			_arguments = arguments;
			_fileArgument = fileArgument;
			_process = new Process
			{
				StartInfo = new ProcessStartInfo
				{
					FileName = processPath.FullName,
					Arguments = ExtractArguments(),
					UseShellExecute = false,
					RedirectStandardOutput = true,
					RedirectStandardError = true,
					CreateNoWindow = true
				}
			};
		}

		private String ExtractArguments()
		{
			var args = new StringBuilder();
			if (_fileArgument != null)
				args.Append(_fileArgument.Name).Append(" ");

			if (_arguments != null)
			{
				var c = 0;
				foreach (var kv in _arguments)
				{
					var val = kv.Value;
					if (val.IndexOf(" ", StringComparison.Ordinal) != -1) val = "\"" + val + "\"";
					args.AppendFormat("--{0}={1}", kv.Key, val);
					if (c != (_arguments.Count - 1))
						args.Append(" ");
					c++;
				}
			}

			return args.ToString();
		}

		public ProcessResult Run()
		{
			if (_fileArgument.Directory != null) Directory.SetCurrentDirectory(_fileArgument.Directory.FullName);

			_process.Start();

			var result = _process.StandardOutput.ReadToEnd();
			var errors = _process.StandardError.ReadToEnd();

			_process.WaitForExit();

			return new ProcessResult
			{
				Output = result,
				Errors = errors
			};
		}

		~ProcessRunner()
		{
			Dispose(false);
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		private void Dispose(bool disposing)
		{
			if (!disposing) return;
			_process.Close();
			_process.Kill();
			_process.Dispose();
		}
	}
}
