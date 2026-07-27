using BenchmarkDotNet.Attributes;
using Salar.BinaryBuffers.Compatibility;
using System.IO;

namespace Salar.BinaryBuffers.Benchmarks;

public abstract class BinaryWriterVsBufferWriterBase
{
	protected const int Loops = 5_000_000;

	protected readonly byte[] _buffer;
	protected readonly MemoryStream _memoryStream;
	protected readonly BinaryWriter _binaryWriter;
	protected readonly BinaryBufferWriter _bufferWriter;
	protected readonly StreamBufferWriter _streamWriter;

	protected BinaryWriterVsBufferWriterBase()
	{
		_buffer = new byte[1024];
		_memoryStream = new MemoryStream(_buffer);
		_binaryWriter = new BinaryWriter(_memoryStream);
		_bufferWriter = new BinaryBufferWriter(_buffer);
		_streamWriter = new StreamBufferWriter(_memoryStream);
	}
}

public class WritePerformanceTest
{
	[BenchmarkCategory("WriteInt")]
	public class BinaryWriterVsBufferWriter_Int : BinaryWriterVsBufferWriterBase
	{
		[Benchmark(Baseline = true)]
		public void BinaryWriter_WriteInt()
		{
			for (int i = 0; i < Loops; i++)
			{
				_memoryStream.Position = 0;

				_binaryWriter.Write(1024);
				_binaryWriter.Write(1024L);
			}
		}

		[Benchmark]
		public void BufferWriter_WriteInt()
		{
			for (int i = 0; i < Loops; i++)
			{
				_bufferWriter.Position = 0;

				_bufferWriter.Write(1024);
				_bufferWriter.Write(1024L);
			}
		}

		[Benchmark]
		public void StreamWriter_WriteInt()
		{
			for (int i = 0; i < Loops; i++)
			{
				_memoryStream.Position = 0;

				_streamWriter.Write(1024);
				_streamWriter.Write(1024L);
			}
		}

		[Benchmark]
		public void SpanBufferWriter_WriteInt()
		{
			var spanWriter = new BinarySpanBufferWriter(_buffer);

			for (int i = 0; i < Loops; i++)
			{
				spanWriter.Position = 0;

				spanWriter.Write(1024);
				spanWriter.Write(1024L);
			}
		}
	}

	[BenchmarkCategory("WriteFloat")]
	public class BinaryWriterVsBufferWriter_Float : BinaryWriterVsBufferWriterBase
	{
		[Benchmark(Baseline = true)]
		public void BinaryWriter_WriteFloat()
		{
			for (int i = 0; i < Loops; i++)
			{
				_memoryStream.Position = 0;

				_binaryWriter.Write(1024.1024F);
			}
		}

		[Benchmark]
		public void BufferWriter_WriteFloat()
		{
			for (int i = 0; i < Loops; i++)
			{
				_bufferWriter.Position = 0;

				_bufferWriter.Write(1024.1024F);
			}
		}

		[Benchmark]
		public void StreamWriter_WriteFloat()
		{
			for (int i = 0; i < Loops; i++)
			{
				_memoryStream.Position = 0;

				_streamWriter.Write(1024.1024F);
			}
		}

		[Benchmark]
		public void SpanBufferWriter_WriteFloat()
		{
			var spanWriter = new BinarySpanBufferWriter(_buffer);

			for (int i = 0; i < Loops; i++)
			{
				spanWriter.Position = 0;

				spanWriter.Write(1024.1024F);
			}
		}
	}

	[BenchmarkCategory("WriteDecimal")]
	public class BinaryWriterVsBufferWriter_Decimal : BinaryWriterVsBufferWriterBase
	{
		[Benchmark(Baseline = true)]
		public void BinaryWriter_WriteDecimal()
		{
			for (int i = 0; i < Loops; i++)
			{
				_memoryStream.Position = 0;

				_binaryWriter.Write(1024.1024M);
			}
		}

		[Benchmark]
		public void BufferWriter_WriteDecimal()
		{
			for (int i = 0; i < Loops; i++)
			{
				_bufferWriter.Position = 0;

				_bufferWriter.Write(1024.1024M);
			}
		}

		[Benchmark]
		public void StreamWriter_WriteDecimal()
		{
			for (int i = 0; i < Loops; i++)
			{
				_memoryStream.Position = 0;

				_streamWriter.Write(1024.1024M);
			}
		}

		[Benchmark]
		public void SpanBufferWriter_WriteDecimal()
		{
			var spanWriter = new BinarySpanBufferWriter(_buffer);

			for (int i = 0; i < Loops; i++)
			{
				spanWriter.Position = 0;

				spanWriter.Write(1024.1024M);
			}
		}
	}
}


public class WriteMemoryTest
{
	[BenchmarkCategory("MemWriteInt")]
	public class MemoryTestBinaryWriterVsBufferWriter_Int : BinaryWriterVsBufferWriterBase
	{
		[IterationSetup]
		public void IterationSetup()
		{
			_memoryStream.Position = 0;
			_bufferWriter.Position = 0;
			_streamWriter.Position = 0;
		}

		[Benchmark(Baseline = true)]
		public void BinaryWriter_WriteInt()
		{
			_binaryWriter.Write(1024);
			_binaryWriter.Write(1024L);
		}

		[Benchmark]
		public void BufferWriter_WriteInt()
		{
			_bufferWriter.Write(1024);
			_bufferWriter.Write(1024L);
		}

		[Benchmark]
		public void StreamWriter_WriteInt()
		{
			_streamWriter.Write(1024);
			_streamWriter.Write(1024L);
		}

		[Benchmark]
		public void SpanBufferWriter_WriteInt()
		{
			var spanWriter = new BinarySpanBufferWriter(_buffer);
			spanWriter.Write(1024);
			spanWriter.Write(1024L);
		}
	}

	[BenchmarkCategory("MemWriteFloat")]
	public class MemoryTestBinaryWriterVsBufferWriter_Float : BinaryWriterVsBufferWriterBase
	{
		[IterationSetup]
		public void IterationSetup()
		{
			_memoryStream.Position = 0;
			_bufferWriter.Position = 0;
			_streamWriter.Position = 0;
		}

		[Benchmark(Baseline = true)]
		public void BinaryWriter_WriteFloat()
		{
			_binaryWriter.Write(1024.1024F);
		}

		[Benchmark]
		public void BufferWriter_WriteFloat()
		{
			_bufferWriter.Write(1024.1024F);
		}

		[Benchmark]
		public void StreamWriter_WriteFloat()
		{
			_streamWriter.Write(1024.1024F);
		}

		[Benchmark]
		public void SpanBufferWriter_WriteFloat()
		{
			var spanWriter = new BinarySpanBufferWriter(_buffer);
			spanWriter.Write(1024.1024F);
		}
	}

	[BenchmarkCategory("MemWriteDecimal")]
	public class MemoryTestBinaryWriterVsBufferWriter_Decimal : BinaryWriterVsBufferWriterBase
	{
		[IterationSetup]
		public void IterationSetup()
		{
			_memoryStream.Position = 0;
			_bufferWriter.Position = 0;
			_streamWriter.Position = 0;
		}

		[Benchmark(Baseline = true)]
		public void BinaryWriter_WriteDecimal()
		{
			_binaryWriter.Write(1024.1024M);
		}

		[Benchmark]
		public void BufferWriter_WriteDecimal()
		{
			_bufferWriter.Write(1024.1024M);
		}

		[Benchmark]
		public void StreamWriter_WriteDecimal()
		{
			_streamWriter.Write(1024.1024M);
		}

		[Benchmark]
		public void SpanBufferWriter_WriteDecimal()
		{
			var spanWriter = new BinarySpanBufferWriter(_buffer);
			spanWriter.Write(1024.1024M);
		}
	}
}

/*
// * Summary *

BenchmarkDotNet v0.15.8, Windows 11 (10.0.26200.8655/25H2/2025Update/HudsonValley2)
13th Gen Intel Core i9-13900H 2.60GHz, 1 CPU, 20 logical and 14 physical cores
.NET SDK 10.0.302
  [Host]     : .NET 10.0.10 (10.0.10, 10.0.1026.32716), X64 RyuJIT x86-64-v3
  Job-CNOHWM : .NET 10.0.10 (10.0.10, 10.0.1026.32716), X64 RyuJIT x86-64-v3

PowerPlanMode=67b4a053-3646-4532-affd-0535c9ea82a7  Runtime=.NET 10.0

| Type                               | Method                               | Mean      | Error     | StdDev    | Median    | Ratio    | RatioSD |
|----------------------------------- |------------------------------------- |----------:|----------:|----------:|----------:|---------:|--------:|
| BinaryReaderVsBufferReader_Decimal | BinaryReader_ReadDecimal             | 14.604 ms | 0.2630 ms | 0.2331 ms | 14.612 ms | baseline |         |
| BinaryReaderVsBufferReader_Decimal | BufferReader_ReadDecimal             | 16.283 ms | 0.2861 ms | 0.3513 ms | 16.289 ms |     +12% |    2.6% |
| BinaryReaderVsBufferReader_Decimal | StreamBufferReader_ReadDecimal       | 45.714 ms | 0.8920 ms | 0.8761 ms | 45.463 ms |    +213% |    2.4% |
| BinaryReaderVsBufferReader_Decimal | BinaryBufferMemoryReader_ReadDecimal | 19.774 ms | 0.3882 ms | 0.4909 ms | 19.730 ms |     +35% |    2.9% |
| BinaryReaderVsBufferReader_Decimal | SequenceBufferReader_ReadDecimal     | 30.572 ms | 0.5799 ms | 0.9199 ms | 30.299 ms |    +109% |    3.3% |
| BinaryReaderVsBufferReader_Float   | BinaryReader_ReadFloat               | 11.492 ms | 0.2278 ms | 0.5544 ms | 11.366 ms | baseline |         |
| BinaryReaderVsBufferReader_Float   | BufferReader_ReadFloat               |  3.263 ms | 0.0652 ms | 0.1485 ms |  3.208 ms |     -78% |    4.8% |
| BinaryReaderVsBufferReader_Float   | StreamBufferReader_ReadFloat         |  7.753 ms | 0.1539 ms | 0.1440 ms |  7.681 ms |     -47% |    2.4% |
| BinaryReaderVsBufferReader_Float   | BinaryBufferMemoryReader_ReadFloat   |  5.925 ms | 0.0860 ms | 0.0718 ms |  5.929 ms |     -59% |    1.9% |
| BinaryReaderVsBufferReader_Float   | SequenceBufferReader_ReadFloat       | 15.740 ms | 0.2266 ms | 0.2009 ms | 15.779 ms |      +8% |    2.0% |
| BinaryReaderVsBufferReader_Int     | BinaryReader_ReadInt                 | 18.045 ms | 0.3465 ms | 0.4970 ms | 17.888 ms | baseline |         |
| BinaryReaderVsBufferReader_Int     | BufferReader_ReadInt                 |  5.166 ms | 0.1031 ms | 0.1779 ms |  5.125 ms |     -65% |    3.7% |
| BinaryReaderVsBufferReader_Int     | StreamBufferReader_ReadInt           | 14.612 ms | 0.2877 ms | 0.3078 ms | 14.593 ms |      +0% |    2.6% |
| BinaryReaderVsBufferReader_Int     | BinaryBufferMemoryReader_ReadInt     | 11.096 ms | 0.1869 ms | 0.1657 ms | 11.088 ms |     -24% |    2.1% |
| BinaryReaderVsBufferReader_Int     | SequenceBufferReader_ReadInt         | 30.856 ms | 0.5794 ms | 0.7328 ms | 30.652 ms |    +111% |    2.8% |
| BinaryWriterVsBufferWriter_Decimal | BinaryWriter_WriteDecimal            | 33.536 ms | 0.6633 ms | 1.3399 ms | 33.211 ms | baseline |         |
| BinaryWriterVsBufferWriter_Decimal | BufferWriter_WriteDecimal            |  6.641 ms | 0.1278 ms | 0.0998 ms |  6.629 ms |     -55% |    2.1% |
| BinaryWriterVsBufferWriter_Decimal | StreamWriter_WriteDecimal            | 29.551 ms | 0.8233 ms | 2.2813 ms | 28.735 ms |    +102% |    7.8% |
| BinaryWriterVsBufferWriter_Decimal | SpanBufferWriter_WriteDecimal        |  2.582 ms | 0.0484 ms | 0.0452 ms |  2.563 ms |     -82% |    2.3% |
| BinaryWriterVsBufferWriter_Float   | BinaryWriter_WriteFloat              | 20.648 ms | 0.3863 ms | 0.4133 ms | 20.521 ms | baseline |         |
| BinaryWriterVsBufferWriter_Float   | BufferWriter_WriteFloat              |  5.306 ms | 0.1016 ms | 0.1356 ms |  5.276 ms |     -64% |    2.9% |
| BinaryWriterVsBufferWriter_Float   | StreamWriter_WriteFloat              | 21.345 ms | 0.4164 ms | 0.4795 ms | 21.250 ms |     +46% |    2.7% |
| BinaryWriterVsBufferWriter_Float   | SpanBufferWriter_WriteFloat          |  1.638 ms | 0.0494 ms | 0.1417 ms |  1.601 ms |     -89% |    8.7% |
| BinaryWriterVsBufferWriter_Int     | BinaryWriter_WriteInt                | 43.426 ms | 0.7984 ms | 1.3983 ms | 43.128 ms | baseline |         |
| BinaryWriterVsBufferWriter_Int     | BufferWriter_WriteInt                |  9.460 ms | 0.1829 ms | 0.1711 ms |  9.398 ms |     -35% |    2.3% |
| BinaryWriterVsBufferWriter_Int     | StreamWriter_WriteInt                | 42.479 ms | 0.6983 ms | 0.6190 ms | 42.434 ms |    +191% |    2.1% |
| BinaryWriterVsBufferWriter_Int     | SpanBufferWriter_WriteInt            |  2.658 ms | 0.0531 ms | 0.1242 ms |  2.615 ms |     -82% |    4.9% | 
*/