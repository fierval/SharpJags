using System.Collections.Generic;
using System.IO;
using SharpJags.Models;
using SharpJags.Sanitazion;

namespace SharpJags.Jags
{
	public class JagsRun
	{
		public string WorkingDirectory { get; set; }
		public MCMCParameters Parameters { get; set; }
		public ModelDefinition ModelDefinition { get; set; }
		public JagsData ModelData { get; set; }
		public JagsData ModelPriors { get; set; }
		public IEnumerable<JagsMonitor> Monitors { get; set; }
		public IOutputSanitizer OutputSanitizer { get; set; }
	}
}
