using System;

namespace SharpJags
{
	[Serializable]
	public class JagsException : Exception
	{
		public JagsException(String message) : base(message) { }
	}
}
