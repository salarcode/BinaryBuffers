using BenchmarkDotNet.Running;

namespace Salar.BinaryBuffers.Benchmark
{
	public class Program
	{
		public static void Main(string[] args)
		{
			BenchmarkRunner.Run(typeof(Program).Assembly);
		}
	}
}
