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
		else if (choice.KeyChar == '1')
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
		Console.WriteLine("1 - Select what to run");
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
