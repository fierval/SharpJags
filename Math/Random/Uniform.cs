namespace SharpJags.Math.Random
{
	public class Uniform : IDistribution
	{
		private readonly System.Random _rng = new System.Random();
		
		private readonly double _lower;
		private readonly double _upper;

		public Uniform(double lower, double upper)
		{
			_lower = lower;
			_upper = upper;
		}

		public double NextDouble()
		{
			return _lower + ((_upper - _lower) * _rng.NextDouble());
		}
	}
}
