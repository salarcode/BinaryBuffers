using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Toolchains.InProcess;
// ReSharper disable InconsistentNaming

namespace Salar.BinaryBuffers.Benchmark
{
	public abstract class BinaryReaderVsBufferReaderBase
	{
		protected const int Loops = 5_000_000;
		private readonly byte[] _buffer;
		protected readonly MemoryStream _mem;
		protected readonly BinaryReader _binaryReader;
		protected readonly BinaryBufferReader _bufferReader;

		protected BinaryReaderVsBufferReaderBase()
		{
			_buffer = new byte[1024];
			_mem = new MemoryStream(_buffer);
			_binaryReader = new BinaryReader(_mem);
			_bufferReader = new BinaryBufferReader(_buffer);
		}
	}

	public class BinaryReaderVsBufferReader_Int : BinaryReaderVsBufferReaderBase
	{
		[Benchmark(Baseline = true)]
		public void BinaryReader_ReadInt()
		{
			for (int i = 0; i < Loops; i++)
			{
				_mem.Position = 0;

				_binaryReader.ReadInt32();
				_binaryReader.ReadInt64();
			}
		}

		[Benchmark()]
		public void BufferReader_ReadInt()
		{
			for (int i = 0; i < Loops; i++)
			{
				_bufferReader.Position = 0;

				_bufferReader.ReadInt32();
				_bufferReader.ReadInt64();
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
				_mem.Position = 0;

				_binaryReader.ReadDecimal();
			}
		}


		[Benchmark()]
		public void BufferReader_ReadDecimal()
		{
			for (int i = 0; i < Loops; i++)
			{
				_bufferReader.Position = 0;

				_bufferReader.ReadDecimal();
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
				_mem.Position = 0;

				_binaryReader.ReadSingle();
			}
		}


		[Benchmark()]
		public void BufferReader_ReadFloat()
		{
			for (int i = 0; i < Loops; i++)
			{
				_bufferReader.Position = 0;

				_bufferReader.ReadSingle();
			}
		}
	}
}
