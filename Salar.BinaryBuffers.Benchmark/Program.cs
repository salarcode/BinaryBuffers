using System;
using System.Reflection;
using BenchmarkDotNet.Running;

namespace Salar.BinaryBuffers.Benchmark
{
	class Program
	{
		static void Main(string[] args)
		{
			BenchmarkRunner.Run(typeof(Program).Assembly);
		}
	}
}