namespace BinaryBuffers.Benchmark.Configurations
{
    using BenchmarkDotNet.Columns;
    using BenchmarkDotNet.Configs;
    using BenchmarkDotNet.Environments;
    using BenchmarkDotNet.Jobs;
    using BenchmarkDotNet.Loggers;
    using BenchmarkDotNet.Reports;

    public class PerformanceConfig : ManualConfig
    {
        public PerformanceConfig()
        {
            Options = ConfigOptions.DisableLogFile;

            AddColumnProvider(DefaultColumnProviders.Instance);
            AddLogger(ConsoleLogger.Default);

            AddJob(Job.Default.WithPowerPlan(PowerPlan.UserPowerPlan));
            //AddJob(Job.Default.WithPowerPlan(Guid.Parse("9935e61f-1661-40c5-ae2f-8495027d5d5d")));          // AMD Ryzen High Performance
            WithSummaryStyle(SummaryStyle.Default.WithRatioStyle(RatioStyle.Percentage));
        }
    }
}
