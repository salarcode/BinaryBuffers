﻿using BenchmarkDotNet.Attributes;
using Salar.BinaryBuffers.Compatibility;
using System.IO;

namespace Salar.BinaryBuffers.Benchmarks;

public abstract class BinaryWriterVsBufferWriterBase
{
	protected const int Loops = 5_000_000;

	protected readonly MemoryStream _memoryStream;
	protected readonly BinaryWriter _binaryWriter;
	protected readonly BinaryBufferWriter _bufferWriter;
	protected readonly StreamBufferWriter _streamWriter;

	protected BinaryWriterVsBufferWriterBase()
	{
		var buffer = new byte[1024];
		_memoryStream = new MemoryStream(buffer);
		_binaryWriter = new BinaryWriter(_memoryStream);
		_bufferWriter = new BinaryBufferWriter(buffer);
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
	}
}


public class WriteMemoryTest
{
	[BenchmarkCategory("MemWriteInt")]
	public class BinaryWriterVsBufferWriter_Int : BinaryWriterVsBufferWriterBase
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
	}

	[BenchmarkCategory("MemWriteFloat")]
	public class BinaryWriterVsBufferWriter_Float : BinaryWriterVsBufferWriterBase
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
	}

	[BenchmarkCategory("MemWriteDecimal")]
	public class BinaryWriterVsBufferWriter_Decimal : BinaryWriterVsBufferWriterBase
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
	}
}