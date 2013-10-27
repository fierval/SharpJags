namespace SharpJags.Jags
{
	public class MCMCParameters
	{
		public int Chains { get; set; }
		public int BurnIn { get; set; }
		public int SampleCount { get; set; }
	}
}
