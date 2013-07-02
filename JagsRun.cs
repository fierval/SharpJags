using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpJags
{
	public class JagsRun
	{
		public MCMCParameters Parameters { get; set; }

		public FileSystemInfo ModelPath { get; set; }
		public JagsData ModelData { get; set; }
		public JagsData ModelPriors { get; set; }

		public List<JagsMonitor> Monitors { get; set; }
	}
}
