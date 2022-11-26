using BenchmarkDotNet.Attributes;
using Salar.BinaryBuffers.Compatibility;
using System.IO;

namespace Salar.BinaryBuffers.Benchmarks;

public abstract class BinaryReaderVsBufferReaderBase
{
	protected const int Loops = 5_000_000;

	protected readonly MemoryStream _memoryStream;
	protected readonly BinaryReader _binaryReader;
	protected readonly BinaryBufferReader _bufferReader;
	protected readonly StreamBufferReader _streamBufferReader;

	protected BinaryReaderVsBufferReaderBase()
	{
		var buffer = new byte[1024];
		_memoryStream = new MemoryStream(buffer);
		_binaryReader = new BinaryReader(_memoryStream);
		_bufferReader = new BinaryBufferReader(buffer);
		_streamBufferReader = new StreamBufferReader(_memoryStream);
	}
}

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
}

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
}

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
}
