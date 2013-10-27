using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using SharpJags.Collections;
using SharpJags.Models;
using SharpJags.Parsing;
using SharpJags.Parsing.Coda;
using SharpJags.Processes;
using SharpJags.Sanitazion;

namespace SharpJags.Jags
{
	public static class JagsWrapper
	{
		private const string ScriptFilenameTemplate = "run{0}.cmd";
		private const string ModelFilenameTemplate = "{0}.jags";
		private const string DataFilename = "jagsData.{0}";
		private const string PriorFilename = "jagsPriors.{0}";

		private static void WriteFile(string rootPath, JagsResourceFile file)
		{
			File.WriteAllText(
				Path.Combine(rootPath, file.Path), file.Data, Encoding.ASCII);
		}

		private static void WriteFiles(string rootPath, IEnumerable<JagsResourceFile> files)
		{
			foreach (var file in files)
			{
				WriteFile(rootPath, file);
			}
		}

		private static IEnumerable<JagsResourceFile> GetDataFiles(JagsData data, bool arePriors = false)
		{
			return data.GetFormattedData().Select(
				formattedData => new JagsResourceFile
				{
					Path = String.Format(
						arePriors 
							? PriorFilename 
							: DataFilename, 
						formattedData.FileExtension),
					
					Data = formattedData.Data
				});
		}

		private static IEnumerable<JagsResourceFile> GetModelDefinition(ModelDefinition modelDefinition, IOutputSanitizer sanitizer = null)
		{
			yield return new JagsResourceFile
			{
				Path = String.Format(ModelFilenameTemplate, modelDefinition.Name),
				
				Data = sanitizer != null 
					? sanitizer.Sanitize(modelDefinition.Definition)
					: modelDefinition.Definition
			};
		}

		private static IEnumerable<JagsResourceFile> GetJagsCommandScript(JagsRun jagsRun, int currentChain)
		{
			yield return new JagsResourceFile
			{
				Path = String.Format(ScriptFilenameTemplate, currentChain),
				Data = BuildJagsCommandScript(jagsRun, currentChain)
			};
		} 

		private static string BuildJagsCommandScript(JagsRun run, int currentChain)
		{
			var script = new StringBuilder()

			.AppendLine(
				String.Format("model in {0}",
					String.Format(ModelFilenameTemplate, run.ModelDefinition.Name)))

			.AppendLine(
				String.Format("data in {0}", 
					String.Format(DataFilename, "r")))

			.AppendLine("compile");

			if (run.ModelPriors != null)
			{
				script.AppendLine(
					String.Format("parameters in {0}",
						String.Format(PriorFilename, "r")));
			}

			script.AppendLine("initialize");
			
			script.AppendLine(
				String.Format("update {0}", run.Parameters.BurnIn));

			foreach (var monitor in run.Monitors)
			{
				var mStr = new StringBuilder(
					String.Format("monitor {0}", monitor.ParameterName));

				if (monitor.Thin > 0)
				{
					mStr.Append(String.Format(", thin({0})", monitor.Thin));
				}

				script.AppendLine(mStr.ToString());
			}

			script.AppendLine(String.Format("update {0}", run.Parameters.SampleCount));
			script.AppendLine(String.Format("coda *, stem(CODA{0})", currentChain));
			script.AppendLine();

			return script.ToString();
		}

		private static void GenerateJagsCommandScript(JagsRun jagsRun, int currentChain)
		{
			WriteFiles(jagsRun.WorkingDirectory, GetJagsCommandScript(jagsRun, currentChain));
		}

		private static void GenerateOutputFiles(JagsRun run)
		{
			IEnumerable<JagsResourceFile> files = new List<JagsResourceFile>();

			if (run.ModelData != null)
			{
				files = files.Union(GetDataFiles(run.ModelData));
			}

			if (run.ModelPriors != null)
			{
				files = files.Union(GetDataFiles(run.ModelPriors, true));
			}

			files = files.Union(GetModelDefinition(run.ModelDefinition, run.OutputSanitizer));

			WriteFiles(run.WorkingDirectory, files);
		}

		private static void JagsTask(JagsRun run, int currentChain)
		{
			GenerateJagsCommandScript(run, currentChain);

			try
			{
				var processRunner = new ProcessRunner(
					new FileInfo(Path.Combine(JagsConfig.BinPath, "jags.bat")));

				var result = processRunner.Run(
					new List<IProcessArgument>
					{
						new JagsArgument(
							Path.Combine(run.WorkingDirectory, String.Format(ScriptFilenameTemplate, currentChain)))
					}, run.WorkingDirectory);

				var isJagsError =
					!String.IsNullOrWhiteSpace(result.Errors)
					&& (new[] { "error", "failed", "unable" })
						.Any(s => result.Errors.IndexOf(s, StringComparison.InvariantCultureIgnoreCase) != -1);

				if (isJagsError)
				{
					throw new Exception("JAGS returned with error: " + result.Errors);
				}
			}
			catch (Exception e)
			{
				throw new JagsException("JAGS exception. Message: " + e.Message);
			}
		}

		public static SampleCollection Run(JagsRun run, ISampleParser parser)
		{
			if (String.IsNullOrEmpty(run.WorkingDirectory))
			{
				throw new ArgumentException("A working directory must be specified.");
			}
			
			GenerateOutputFiles(run);

			var tasks = new List<Task>();
			for (var i = 0; i < run.Parameters.Chains; i++)
			{
				new Action<int>(currentChain => tasks.Add(Task.Factory.StartNew(() => JagsTask(run, currentChain))))(i);
			}

			Task.WaitAll(tasks.ToArray());

			return parser.Parse(run.WorkingDirectory, "CODA0index.txt", "CODA{0}chain1.txt", run.Parameters.Chains);
		}
	}
}
