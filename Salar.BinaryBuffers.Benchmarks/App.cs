using BenchmarkDotNet.Running;
using Salar.BinaryBuffers.Benchmarks.Configurations;

namespace Salar.BinaryBuffers.Benchmarks;

public static class App
{
	public static void Main()
	{
		//BenchmarkSwitcher.FromAssembly(typeof(App).Assembly).Run(config: new PerformanceConfig());
		BenchmarkRunner.Run(typeof(App).Assembly, new PerformanceConfig());
	}
}
