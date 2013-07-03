# SharpJags
### A C\# Wrapper for JAGS (Just Another Gibbs Sampler)

SharpJags is a wrapper for JAGS (http://mcmc-jags.sourceforge.net) and enables it's users to perform MCMC-simulations and Bayesian learning in C\#.
The wrapper is fully functional and comes with handy utilities for calculating the descriptive statistics of the posterior samples. It is also able to run
several chains in parallel. The wrapper takes care of formatting the input data (R-format), running JAGS and parsing the resulting samples. Beware however this
is not yet production quality code.

### Perform sampling

```
var mat = new Matrix<double>
	{
		new Vector<double>
			{
				1, 2, 3
			},
		new Vector<double>
			{
				4, 5, 6
			},
		new Vector<double>
			{
				7, 8, 9
			}
	};

var data = new JagsData()
	{
		{ "TestMat", mat }
	};

var modelDefintion = new ModelDefinition 
	{ 
		Name = "Gaussians",
		Definition = 
			@"
				model {
					y <- (alpha * beta) + gamma
					alpha ~ dnorm(0, 1)
					beta ~ dnorm(0, 1)
					gamma ~ dnorm(0, 1)
				}
			"
	};

JagsConfig.Path = @"C:\Jags\bin\jags.bat";

var run = new JagsRun
	{
		WorkingDirectory = new FileInfo(Directory.GetCurrentDirectory()),
		ModelDefinition = modelDefintion,
		ModelData = data,
		Monitors = new List<JagsMonitor> 
			{ 
				new JagsMonitor { ParameterName = "sigma", Thin = 3 }
			},
		Parameters = new MCMCParameters 
			{ 
				BurnIn = 1000,
				SampleCount = 2000,
				Chains = 3
			}
	};

var samples = JagsWrapper.Run(run);

var sigmaSamples = samples.Get<ModelParameterVector>("sigma");

Debug.WriteLine("Mean: "	+ sigmaSamples.Statistics.Mean);
Debug.WriteLine("Std: "		+ sigmaSamples.Statistics.StandardDeviation);
```