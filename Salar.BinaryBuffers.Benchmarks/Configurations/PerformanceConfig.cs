using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Environments;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Loggers;
using BenchmarkDotNet.Reports;

namespace Salar.BinaryBuffers.Benchmarks.Configurations;

public class PerformanceConfig : ManualConfig
{
	public PerformanceConfig(int? iterationCount = null)
	{
		Options = ConfigOptions.DisableLogFile;

		AddColumnProvider(DefaultColumnProviders.Instance);
		AddLogger(ConsoleLogger.Default);

		//AddJob(Job.Default.WithPowerPlan(PowerPlan.UserPowerPlan).WithRuntime(CoreRuntime.Core60));
		var job = Job.Default.WithPowerPlan(PowerPlan.UserPowerPlan).WithRuntime(CoreRuntime.Latest);
		if (iterationCount is > 0)
		{
			job = job
				.WithLaunchCount(1)
				.WithWarmupCount(System.Math.Max(1, iterationCount.Value / 10))
				.WithIterationCount(iterationCount.Value);
		}

		AddJob(job);
		//AddJob(Job.Default.WithPowerPlan(Guid.Parse("9935e61f-1661-40c5-ae2f-8495027d5d5d")));          // AMD Ryzen High Performance
		WithSummaryStyle(SummaryStyle.Default.WithRatioStyle(RatioStyle.Percentage));
		//AddExporter(new[] { new HtmlExporter() });
	}
}
