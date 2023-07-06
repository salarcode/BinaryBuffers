using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Diagnosers;
using Salar.BinaryBuffers.Compatibility;
using System;
using System.Buffers;
using System.IO;

namespace Salar.BinaryBuffers.Benchmarks;

public abstract class BinaryReaderVsBufferReaderBase
{
	protected const int Loops = 5_000_000;

	protected readonly MemoryStream _memoryStream;
	protected readonly BinaryReader _binaryReader;
	protected readonly BinaryBufferReader _bufferReader;
	protected readonly BinaryBufferMemoryReader _binaryBufferMemoryReader;
	protected readonly StreamBufferReader _streamBufferReader;
	protected readonly SequenceBufferReader _sequenceBufferReader;

	protected BinaryReaderVsBufferReaderBase()
	{
		var buffer = new byte[1024];
		_memoryStream = new MemoryStream(buffer);
		_binaryReader = new BinaryReader(_memoryStream);
		_bufferReader = new BinaryBufferReader(buffer);
		_streamBufferReader = new StreamBufferReader(_memoryStream);
		_binaryBufferMemoryReader = new BinaryBufferMemoryReader(new ReadOnlyMemory<byte>(buffer));
		_sequenceBufferReader = new SequenceBufferReader(new ReadOnlySequence<byte>(buffer));
	}
}

public class ReadPerformanceTest
{
	[BenchmarkCategory("ReadInt")]
	public class BinaryReaderVsBufferReader_Int : BinaryReaderVsBufferReaderBase
	{
		[Benchmark(Baseline = true)]
		public void BinaryReader_ReadInt()
		{
			for (int i = 0; i < Loops; i++)
			{
				_memoryStream.Position = 0;

				_binaryReader.ReadInt32();
				_binaryReader.ReadInt64();
			}
		}

		[Benchmark]
		public void BufferReader_ReadInt()
		{
			for (int i = 0; i < Loops; i++)
			{
				_bufferReader.Position = 0;

				_bufferReader.ReadInt32();
				_bufferReader.ReadInt64();
			}
		}

		[Benchmark]
		public void StreamBufferReader_ReadInt()
		{
			for (int i = 0; i < Loops; i++)
			{
				_memoryStream.Position = 0;

				_streamBufferReader.ReadInt32();
				_streamBufferReader.ReadInt64();
			}
		}

		[Benchmark]
		public void BinaryBufferMemoryReader_ReadInt()
		{
			for (int i = 0; i < Loops; i++)
			{
				_binaryBufferMemoryReader.Position = 0;

				_binaryBufferMemoryReader.ReadInt32();
				_binaryBufferMemoryReader.ReadInt64();
			}
		}

		[Benchmark]
		public void SequenceBufferReader_ReadInt()
		{
			for (int i = 0; i < Loops; i++)
			{
				_sequenceBufferReader.Position = 0;

				_sequenceBufferReader.ReadInt32();
				_sequenceBufferReader.ReadInt64();
			}
		}
	}

	[BenchmarkCategory("ReadDecimal")]
	public class BinaryReaderVsBufferReader_Decimal : BinaryReaderVsBufferReaderBase
	{
		[Benchmark(Baseline = true)]
		public void BinaryReader_ReadDecimal()
		{
			for (int i = 0; i < Loops; i++)
			{
				_memoryStream.Position = 0;

				_binaryReader.ReadDecimal();
			}
		}

		[Benchmark]
		public void BufferReader_ReadDecimal()
		{
			for (int i = 0; i < Loops; i++)
			{
				_bufferReader.Position = 0;

				_bufferReader.ReadDecimal();
			}
		}

		[Benchmark]
		public void StreamBufferReader_ReadDecimal()
		{
			for (int i = 0; i < Loops; i++)
			{
				_memoryStream.Position = 0;

				_streamBufferReader.ReadDecimal();
			}
		}

		[Benchmark]
		public void BinaryBufferMemoryReader_ReadDecimal()
		{
			for (int i = 0; i < Loops; i++)
			{
				_binaryBufferMemoryReader.Position = 0;

				_binaryBufferMemoryReader.ReadDecimal();
			}
		}

		[Benchmark]
		public void SequenceBufferReader_ReadDecimal()
		{
			for (int i = 0; i < Loops; i++)
			{
				_sequenceBufferReader.Position = 0;

				_sequenceBufferReader.ReadDecimal();
			}
		}
	}

	[BenchmarkCategory("ReadFloat")]
	public class BinaryReaderVsBufferReader_Float : BinaryReaderVsBufferReaderBase
	{
		[Benchmark(Baseline = true)]
		public void BinaryReader_ReadFloat()
		{
			for (int i = 0; i < Loops; i++)
			{
				_memoryStream.Position = 0;

				_binaryReader.ReadSingle();
			}
		}

		[Benchmark]
		public void BufferReader_ReadFloat()
		{
			for (int i = 0; i < Loops; i++)
			{
				_bufferReader.Position = 0;

				_bufferReader.ReadSingle();
			}
		}

		[Benchmark]
		public void StreamBufferReader_ReadFloat()
		{
			for (int i = 0; i < Loops; i++)
			{
				_memoryStream.Position = 0;

				_streamBufferReader.ReadSingle();
			}
		}

		[Benchmark]
		public void BinaryBufferMemoryReader_ReadFloat()
		{
			for (int i = 0; i < Loops; i++)
			{
				_binaryBufferMemoryReader.Position = 0;

				_binaryBufferMemoryReader.ReadSingle();
			}
		}

		[Benchmark]
		public void SequenceBufferReader_ReadFloat()
		{
			for (int i = 0; i < Loops; i++)
			{
				_sequenceBufferReader.Position = 0;

				_sequenceBufferReader.ReadSingle();
			}
		}
	}
}

public class ReadMemoryTests
{
	[MemoryDiagnoser]
	[BenchmarkCategory("Mem_ReadInt")]
	public class BinaryReaderVsBufferReader_Int : BinaryReaderVsBufferReaderBase
	{
		[IterationSetup]
		public void IterationSetup()
		{
			_memoryStream.Position = 0;
			_bufferReader.Position = 0;
			_binaryBufferMemoryReader.Position = 0;
			_sequenceBufferReader.Position = 0;
		}

		[Benchmark(Baseline = true)]
		public void BinaryReader_ReadInt()
		{
			_binaryReader.ReadInt32();
			_binaryReader.ReadInt64();
		}

		[Benchmark]
		public void BufferReader_ReadInt()
		{
			_bufferReader.ReadInt32();
			_bufferReader.ReadInt64();
		}

		[Benchmark]
		public void StreamBufferReader_ReadInt()
		{
			_streamBufferReader.ReadInt32();
			_streamBufferReader.ReadInt64();
		}

		[Benchmark]
		public void BinaryBufferMemoryReader_ReadInt()
		{
			_binaryBufferMemoryReader.ReadInt32();
			_binaryBufferMemoryReader.ReadInt64();
		}

		[Benchmark]
		public void SequenceBufferReader_ReadInt()
		{
			_sequenceBufferReader.ReadInt32();
			_sequenceBufferReader.ReadInt64();
		}
	}

	[MemoryDiagnoser]
	[BenchmarkCategory("Mem_ReadDecimal")]
	public class BinaryReaderVsBufferReader_Decimal : BinaryReaderVsBufferReaderBase
	{
		[IterationSetup]
		public void IterationSetup()
		{
			_memoryStream.Position = 0;
			_bufferReader.Position = 0;
			_binaryBufferMemoryReader.Position = 0;
			_sequenceBufferReader.Position = 0;
		}

		[Benchmark(Baseline = true)]
		public void BinaryReader_ReadDecimal()
		{
			_binaryReader.ReadDecimal();
		}

		[Benchmark]
		public void BufferReader_ReadDecimal()
		{
			_bufferReader.ReadDecimal();
		}

		[Benchmark]
		public void StreamBufferReader_ReadDecimal()
		{
			_streamBufferReader.ReadDecimal();
		}

		[Benchmark]
		public void BinaryBufferMemoryReader_ReadDecimal()
		{
			_binaryBufferMemoryReader.ReadDecimal();
		}

		[Benchmark]
		public void SequenceBufferReader_ReadDecimal()
		{
			_sequenceBufferReader.ReadDecimal();
		}
	}

	[MemoryDiagnoser]
	[BenchmarkCategory("Mem_ReadFloat")]
	public class BinaryReaderVsBufferReader_Float : BinaryReaderVsBufferReaderBase
	{
		[IterationSetup]
		public void IterationSetup()
		{
			_memoryStream.Position = 0;
			_bufferReader.Position = 0;
			_binaryBufferMemoryReader.Position = 0;
			_sequenceBufferReader.Position = 0;
		}

		[Benchmark(Baseline = true)]
		public void BinaryReader_ReadFloat()
		{
			_binaryReader.ReadSingle();
		}

		[Benchmark]
		public void BufferReader_ReadFloat()
		{
			_bufferReader.ReadSingle();
		}

		[Benchmark]
		public void StreamBufferReader_ReadFloat()
		{
			_streamBufferReader.ReadSingle();
		}

		[Benchmark]
		public void BinaryBufferMemoryReader_ReadFloat()
		{
			_binaryBufferMemoryReader.ReadSingle();
		}

		[Benchmark]
		public void SequenceBufferReader_ReadFloat()
		{
			_sequenceBufferReader.ReadSingle();
		}
	}
}