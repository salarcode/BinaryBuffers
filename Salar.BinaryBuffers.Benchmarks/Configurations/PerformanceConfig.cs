using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Environments;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Loggers;
using BenchmarkDotNet.Reports;

namespace Salar.BinaryBuffers.Benchmarks.Configurations;

public class PerformanceConfig : ManualConfig
{
	public PerformanceConfig()
	{
		Options = ConfigOptions.DisableLogFile;

		AddColumnProvider(DefaultColumnProviders.Instance);
		AddLogger(ConsoleLogger.Default);

		//AddJob(Job.Default.WithPowerPlan(PowerPlan.UserPowerPlan).WithRuntime(CoreRuntime.Core60));
		AddJob(Job.Default.WithPowerPlan(PowerPlan.UserPowerPlan).WithRuntime(CoreRuntime.Core70));
		//AddJob(Job.Default.WithPowerPlan(Guid.Parse("9935e61f-1661-40c5-ae2f-8495027d5d5d")));          // AMD Ryzen High Performance
		WithSummaryStyle(SummaryStyle.Default.WithRatioStyle(RatioStyle.Percentage));
		//AddExporter(new[] { new HtmlExporter() });
	}
}
