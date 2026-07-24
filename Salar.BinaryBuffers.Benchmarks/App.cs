using BenchmarkDotNet.Running;
using Salar.BinaryBuffers.Benchmarks.Configurations;
using System;

namespace Salar.BinaryBuffers.Benchmarks;

public static class App
{
	public static void Main(string[] args)
	{
		if (args.Length > 0)
		{
			BenchmarkSwitcher.FromAssembly(typeof(App).Assembly).Run(args);
			return;
		}

		Console.WriteLine("Welcome to BinaryBuffers benchmark!");
#if DEBUG
		Console.WriteLine("**********************************************");		
		Console.WriteLine("You are in DEBUG mode please use Release mode.");		
		Console.WriteLine("**********************************************");		
#endif
		var choice = Menu();
		if (choice.Key == ConsoleKey.Q)
		{
			return;
		}
		if (choice.KeyChar is < '0' or > '3')
		{
			Console.WriteLine("Invalid choice!");
			return;
		}

		var config = new PerformanceConfig(PromptIterationCount());
		switch (choice.KeyChar)
		{
			case '0':
				BenchmarkSwitcher.FromAssembly(typeof(App).Assembly).RunAllJoined(config: config);
				break;
			case '1':
				BenchmarkSwitcher.FromTypes(new[] {
					typeof(ReadPerformanceTest.BinaryReaderVsBufferReader_Int),
					typeof(ReadPerformanceTest.BinaryReaderVsBufferReader_Float),
					typeof(ReadPerformanceTest.BinaryReaderVsBufferReader_Decimal),

					typeof(WritePerformanceTest.BinaryWriterVsBufferWriter_Int),
					typeof(WritePerformanceTest.BinaryWriterVsBufferWriter_Float),
					typeof(WritePerformanceTest.BinaryWriterVsBufferWriter_Decimal)
				}).RunAllJoined(config: config);
				break;
			case '2':
				BenchmarkSwitcher.FromTypes(new[] {
					typeof(ReadMemoryTests.MemoryBinaryReaderVsBufferReader_Int),
					typeof(ReadMemoryTests.MemoryBinaryReaderVsBufferReader_Float),
					typeof(ReadMemoryTests.MemoryBinaryReaderVsBufferReader_Decimal),

					typeof(WriteMemoryTest.MemoryTestBinaryWriterVsBufferWriter_Int),
					typeof(WriteMemoryTest.MemoryTestBinaryWriterVsBufferWriter_Float),
					typeof(WriteMemoryTest.MemoryTestBinaryWriterVsBufferWriter_Decimal)
				}).RunAllJoined(config: config);
				break;
			case '3':
				BenchmarkSwitcher.FromAssembly(typeof(App).Assembly).Run(config: config);
				break;
		}
	}

	static int? PromptIterationCount()
	{
		Console.Write("Number of runs (press Enter to use the default): ");
		var input = Console.ReadLine();
		return int.TryParse(input, out var iterationCount) && iterationCount > 0
			? iterationCount
			: null;
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
