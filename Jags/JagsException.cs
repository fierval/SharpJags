using System;

namespace SharpJags.Jags
{
	[Serializable]
	public class JagsException : Exception
	{
		public JagsException(String message) : base(message) { }
	}
}
