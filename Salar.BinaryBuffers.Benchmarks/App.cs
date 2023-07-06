using BenchmarkDotNet.Running;
using Salar.BinaryBuffers.Benchmarks.Configurations;
using System;

namespace Salar.BinaryBuffers.Benchmarks;

public static class App
{
	public static void Main()
	{
		Console.WriteLine("Welcome to BinaryBuffers benchmark!");
#if DEBUG
		Console.WriteLine("**********************************************");		
		Console.WriteLine("You are in DEBUG mode please use Release mode.");		
		Console.WriteLine("**********************************************");		
#endif
		var choice = Menu();
		if (choice.KeyChar == '0')
		{
			BenchmarkSwitcher.FromAssembly(typeof(App).Assembly).RunAllJoined(config: new PerformanceConfig());
		}
		if (choice.KeyChar == '1')
		{
			BenchmarkSwitcher.FromTypes(new[] {
				typeof(ReadPerformanceTest.BinaryReaderVsBufferReader_Int),
				typeof(ReadPerformanceTest.BinaryReaderVsBufferReader_Float),
				typeof(ReadPerformanceTest.BinaryReaderVsBufferReader_Decimal),

				typeof(WritePerformanceTest.BinaryWriterVsBufferWriter_Int),
				typeof(WritePerformanceTest.BinaryWriterVsBufferWriter_Float),
				typeof(WritePerformanceTest.BinaryWriterVsBufferWriter_Decimal)
			}).RunAllJoined(config: new PerformanceConfig());
		}
		if (choice.KeyChar == '2')
		{
			BenchmarkSwitcher.FromTypes(new[] {
				typeof(ReadMemoryTests.BinaryReaderVsBufferReader_Int),
				typeof(ReadMemoryTests.BinaryReaderVsBufferReader_Float),
				typeof(ReadMemoryTests.BinaryReaderVsBufferReader_Decimal),

				typeof(WriteMemoryTest.BinaryWriterVsBufferWriter_Int),
				typeof(WriteMemoryTest.BinaryWriterVsBufferWriter_Float),
				typeof(WriteMemoryTest.BinaryWriterVsBufferWriter_Decimal)
			}).RunAllJoined(config: new PerformanceConfig());
		}
		else if (choice.KeyChar == '3')
		{
			BenchmarkSwitcher.FromAssembly(typeof(App).Assembly).Run(config: new PerformanceConfig());
		}
		else if (choice.Key == ConsoleKey.Q)
		{
			return;
		}
		else
		{
			Console.WriteLine("Invalid choice!");
		}
	}

	static ConsoleKeyInfo Menu()
	{
		Console.WriteLine("0 - Run all benchmarks");
		Console.WriteLine("1 - Run performance benchmarks");
		Console.WriteLine("2 - Run memory benchmarks");
		Console.WriteLine("3 - Select what to run");
#if DEBUG
		Console.WriteLine("d - Debug");
#endif
		Console.WriteLine("q - Quit");
		Console.Write("Please enter your choice: ");
		try
		{
			return Console.ReadKey();
		}
		finally
		{
			Console.WriteLine();
		}
	}
}
