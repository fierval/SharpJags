using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpJags
{
	public class MCMCParameters
	{
		public int Chains { get; set; }
		public int BurnIn { get; set; }
		public int SampleCount { get; set; }
	}
}
