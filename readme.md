# SharpJags
### A C\# Wrapper for JAGS (Just Another Gibbs Sampler)

SharpJags is a wrapper for JAGS (http://mcmc-jags.sourceforge.net) and enables it's users to perform MCMC-simulations and Bayesian learning in C\#.
The wrapper is fully functional and comes with handy utilities for calculating the descriptive statistics of the posterior samples. It is also able to run
several chains in parallel. The wrapper takes care of formatting the input data (R-format), running JAGS and parsing the resulting samples. Beware however this
is not yet production quality code.

#### 0.1.1 Release

* Improved Vector<T> and Matrix<T> classes so to appear less verbose
* General refactoring
* Created nuget package

#### 0.1.0 Release

The library is ready to be experimented with, but bugs must be accounted for. Changes since initial commit:

* All models are now defined inline to make configuration of paths easier
* Added random number generators - Gaussian (Box-Mueller alg.) and Uniform
* Minor improvements in error handling and general code quality
* Updated model example (linear regression)
* The monitor objects themselves are now used to get the associated samples

#### Howto: Perform sampling

``` csharp
JagsConfig.BinPath = @"C:\Program Files\JAGS\JAGS-3.3.0\x64\bin";

Matrix<double> fooMatrix = new double[,]
{
	{ 1, 2, 3 },
	{ 4, 5, 6 },
	{ 7, 8, 9 }
};

var n = 1000;

Vector<double> x = Enumerable.Range(0, n)
	.ToList().ConvertAll(i => (double) i).ToArray();

// SharpJags contains utilities for generating sets of random numbers -
// either normally or uniformly distributed. This is nice for simulating the
// uncertainty of the input data.
// new Gaussian(0, 1).Generate(n) - Generate n numbers drawn from the normal distribution with mean=0 and standard deviation=1 (standard normal)
// new Uniform(-1, 1).Generate(n) - Generate n numbers drawn from the uniform distribution that is defined by [min, max]
Vector<double> epsilon = new Gaussian(0, 1)
	.Generate(n).ToArray();

Vector<double> delta = new Uniform(-1, 1)
	.Generate(n).ToArray();

var data = new JagsData
{
	{ "N", n },
	{ "x", x },
	{ "epsilon", epsilon },
	{ "delta", delta }
};

var modelDefintion = new ModelDefinition
{
	Name = "Gaussians",
	Definition =
	@"
		model {
			for (i in 1:N) {
				y[i] ~ dnorm(y.hat[i], tau)
				y.hat[i] <- a + b * x[i]
			}

			a ~ dnorm(0, .0001)
			b ~ dnorm(0, .0001)
			tau <- pow(sigma, -2)
			sigma ~ dunif(0, 100)
		}
	"
};

var yMonitor = new JagsMonitor()
{
	ParameterName = "y",
	Thin = 1
};

var run = new JagsRun
{
	WorkingDirectory = new FileInfo(Directory.GetCurrentDirectory()),
	ModelDefinition = modelDefintion,
	ModelData = data,
	Monitors = new List<JagsMonitor>
	{
		yMonitor
	},
	Parameters = new MCMCParameters
	{
		BurnIn = 1000,
		SampleCount = 2000,
		Chains = 3
	}
};

var samples = JagsWrapper.Run(run);

// We can also use parameterSamples = samples.All()
// Which returns an IList<IModelParameter>
// We need to use .Get<ModelParameter|ModelParameterVector|ModelParameterMatrix> to
// access each sample individually
var parameterSamples = new List<IModelParameter>
{
	samples.Get<ModelParameterVector>(yMonitor)
};

parameterSamples.ForEach(p =>
	Debug.WriteLine("{0}: Mean: {1}, Std: {2}", 
		p.ParameterName, p.Statistics.Mean, p.Statistics.StandardDeviation));
```

Output

```
y: Mean: -274.045177465289, Std: 58547.0020677397
```
