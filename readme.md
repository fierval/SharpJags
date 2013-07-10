# SharpJags
### A C\# Wrapper for JAGS (Just Another Gibbs Sampler)

SharpJags is a wrapper for JAGS (http://mcmc-jags.sourceforge.net) and enables it's users to perform MCMC-simulations and Bayesian learning in C\#.
The wrapper is fully functional and comes with handy utilities for calculating the descriptive statistics of the posterior samples. It is also able to run
several chains in parallel. The wrapper takes care of formatting the input data (R-format), running JAGS and parsing the resulting samples. Beware however this
is not yet production quality code.

#### 0.1.0 Release

The library is ready to be experimented with, but bugs must be accounted for. Changes since initial commit:

* All models are now defined inline to make configuration of paths easier
* Added random number generators - Gaussian (Box-Mueller alg.) and Uniform
* Minor improvements in error handling and general code quality
* Updated model example (linear regression)
* The monitor objects themselves are now used to get the associated samples

#### Howto: Perform sampling

```
JagsConfig.BinPath = @"C:\Program Files\JAGS\JAGS-3.3.0\x64\bin";

var fooMatrix = new Matrix<double>
{
	new double[]
	{
		1,
		2,
		3
	},
	new double[]
	{
		4,
		5,
		6
	},
	new double[]
	{
		7,
		8,
		9
	}
};

var n = 1000;
var x = new Vector<double>(
	Enumerable.Range(0, n)
	.ToList()
	.ConvertAll(i => (double)i));

// SharpJags contains utilities for generating sets of random numbers -
// either normally or uniformly distributed. This is nice for simulating the
// uncertainty of the input data.
// new Gaussian(0, 1).Generate(n) - Generate n numbers drawn from the normal distribution with mean=0 and standard deviation=1 (standard normal)
// new Uniform(-1, 1).Generate(n) - Generate n numbers drawn from the uniform distribution that is defined by [min, max]
var epsilon = new Vector<double>(
	new Gaussian(0, 1)
		.Generate(n));

var delta = new Vector<double>(
	new Uniform(-1, 1)
		.Generate(n));

var data = new JagsData
{
	{ "N", n },
	{ "x", x },
	{ "epsilon", epsilon },
	{ "delta", delta }
};

// Default simple linear regression example
var modelDefintion = new ModelDefinition
{
	Name = "LinearRegression",
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

var alphaMonitor = new JagsMonitor()
{
	ParameterName = "a",
	Thin = 1
};

var betaMonitor = new JagsMonitor()
{
	ParameterName = "b",
	Thin = 1
};

var run = new JagsRun
{
	WorkingDirectory = new FileInfo(Directory.GetCurrentDirectory()),
	ModelDefinition = modelDefintion,
	ModelData = data,
	Monitors = new List<JagsMonitor>
	{
		alphaMonitor,
		betaMonitor
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
	samples.Get<ModelParameter>(alphaMonitor),
	samples.Get<ModelParameter>(betaMonitor)
};

parameterSamples.ForEach(p => 
	Debug.WriteLine("{0}: Mean: {1}, Std: {2}", p.ParameterName, p.Statistics.Mean, p.Statistics.StandardDeviation));
```

Outputs

```
a: Mean: -1.27493863785, Std: 100.421535452004
b: Mean: 1.189023323, Std: 98.5209869502626
```