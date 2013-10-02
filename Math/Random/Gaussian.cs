namespace SharpJags.Math.Random
{
	public class Gaussian : IDistribution
	{
		private readonly System.Random _rng = new System.Random();
		double? _spareValue;

		private readonly double _mu;
		private readonly double _sigma;

		public Gaussian(double mu, double sigma)
		{
			_mu = mu;
			_sigma = sigma;
		}

		private double GenNextDouble()
		{
			if (null != _spareValue)
			{
				var tmp = _spareValue.Value;
				_spareValue = null;
				return tmp;
			}

			double x, y, sqr;

			do
			{
				x = 2.0 * _rng.NextDouble() - 1.0;
				y = 2.0 * _rng.NextDouble() - 1.0;
				sqr = x * x + y * y;
			}
			while (sqr > 1.0 || System.Math.Abs(sqr) < 0.0001);

			var fac = System.Math.Sqrt(-2.0 * System.Math.Log(sqr) / sqr);

			_spareValue = x * fac;
			return y * fac;
		}

		public double NextDouble()
		{
			return _mu + (GenNextDouble() * _sigma);
		}
	}
}
