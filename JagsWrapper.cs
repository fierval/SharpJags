using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using SharpJags.CodaParser;
using SharpJags.Processes;

namespace SharpJags
{
	public static class JagsWrapper
	{
		private const String ScriptFilenameTemplate = "run{0}.cmd";
		private const String ModelFilenameTemplate = "{0}.jags";
		private const String DataFilename = "jagsData";
		private const String PriorFilename = "jagsPriors.r";

		private static void WriteFile(FileSystemInfo path, String data)
		{
			File.WriteAllText(path.FullName, data, Encoding.ASCII);
		}
		
		private static void SaveDataFile(FileSystemInfo modelDirectory, String data, String extension = ".r")
		{
			var dataPath = new FileInfo(modelDirectory.FullName + "/" + DataFilename + extension);
			WriteFile(dataPath, data);
		}

		private static void SavePriorsDataFiles(FileSystemInfo modelDirectory, String priorsData)
		{
			var priorPath = new FileInfo(modelDirectory.FullName + "/" + PriorFilename);
			WriteFile(priorPath, priorsData);
		}

		private static void SaveJagsScript(FileSystemInfo modelDirectory, int currentChain, String script)
		{
			var scriptPath = new FileInfo(modelDirectory.FullName + "/" + String.Format(ScriptFilenameTemplate, currentChain));
			WriteFile(scriptPath, script);
		}

		private static void SaveModelDefinition(FileSystemInfo modelDirectory, ModelDefinition modelDefinition)
		{
			var dataPath = new FileInfo(modelDirectory.FullName + "/" + modelDefinition.Name + ".jags");
			WriteFile(dataPath, SanitizeModelOutput(modelDefinition.Definition));
		}

		private static string SanitizeModelOutput(string definition)
		{
			var numberOfLeadingTabs = 0;

			foreach (var t in definition)
			{
				if (t == 9)
				{
					numberOfLeadingTabs++;
					continue;
				}
				
				if(t == 13 || t == 10) continue;

				break;
			}

			return Regex
				.Replace(definition, @"\t{" + numberOfLeadingTabs + "}", String.Empty)
				.Trim();
		}

		private static void GenerateJagsDataFiles(JagsRun run)
		{
			SaveModelDefinition(run.WorkingDirectory, run.ModelDefinition);

			if (run.ModelData != null)
			{
				SaveDataFile(run.WorkingDirectory, run.ModelData.DumpR());
				SaveDataFile(run.WorkingDirectory, run.ModelData.DumpMatlab(), ".m");
			}

			if (run.ModelPriors != null)
			{
				SavePriorsDataFiles(run.WorkingDirectory, run.ModelPriors.DumpR());
			}
		}

		private static void GenerateJagsScript(JagsRun run, int currentChain)
		{
			var script = new StringBuilder();
			
			script.AppendLine(
				String.Format("model in {0}", 
				String.Format(ModelFilenameTemplate, run.ModelDefinition.Name)));

			script.AppendLine(
				String.Format("data in {0}.r", DataFilename));

			script.AppendLine("compile");

			if(run.ModelPriors != null)
				script.AppendLine(
					String.Format("parameters in {0}", PriorFilename));

			script.AppendLine("initialize");
			script.AppendLine(String.Format("update {0}", run.Parameters.BurnIn));

			foreach(var monitor in run.Monitors)
			{
				var mStr = new StringBuilder(
					String.Format("monitor {0}", monitor.ParameterName));
				
				if (monitor.Thin > 0)
					mStr.Append(String.Format(", thin({0})", monitor.Thin));
				
				script.AppendLine(mStr.ToString());
			}

			script.AppendLine(String.Format("update {0}", run.Parameters.SampleCount));
			script.AppendLine(String.Format("coda *, stem(CODA{0})", currentChain));
			script.AppendLine();

			SaveJagsScript(run.WorkingDirectory, currentChain, script.ToString());
		}

		private static void JagsTask(JagsRun run, int currentChain)
		{
			GenerateJagsScript(run, currentChain);

			try
			{
				if (run.WorkingDirectory == null) return;
				
				var result = new ProcessRunner(
					new FileInfo(
						String.Format("{0}/{1}", JagsConfig.BinPath, "jags.bat")),
					new FileInfo(
						String.Format("{0}/{1}", run.WorkingDirectory.FullName,
						String.Format(ScriptFilenameTemplate, currentChain))))
							.Run();

				var isRealErrorSituation =
					!String.IsNullOrWhiteSpace(result.Errors)
					&& (new[]
						{
							"error", 
							"failed",
							"unable"
						}).Any(s => result.Errors.IndexOf(s, StringComparison.InvariantCultureIgnoreCase) != -1);

				if (isRealErrorSituation)
					throw new OperationCanceledException("JAGS returned with error: " + result.Errors);
			}
			catch (Exception e)
			{
				throw new JagsException("JAGS exception. Message: " + e.Message);
			}
		}

		public static SampleCollection Run(JagsRun run)
		{
			GenerateJagsDataFiles(run);

			var tasks = new List<Task>();
			for (var i = 0; i < run.Parameters.Chains; i++)
			{
				new Action<int>(currentChain => tasks.Add(Task.Factory.StartNew(() => JagsTask(run, currentChain))))(i);
			}

			Task.WaitAll(tasks.ToArray());

			var data = CodaDataReader.Read(run.WorkingDirectory, "CODA0index.txt", "CODA{0}chain1.txt", run.Parameters.Chains);
			
			return new Parser().Parse(data);
		}
	}
}
