using System.IO;
using BenchmarkDotNet.Attributes;

namespace Salar.BinaryBuffers.Benchmark
{
	public abstract class BinaryWriterVsBufferWriterBase
	{
		protected const int Loops = 5_000_000;
		private readonly byte[] _buffer;
		protected readonly MemoryStream _mem;
		protected readonly BinaryWriter _binaryWriter;
		protected readonly BinaryBufferWriter _bufferWriter;

		protected BinaryWriterVsBufferWriterBase()
		{
			_buffer = new byte[1024];
			_mem = new MemoryStream(_buffer);
			_binaryWriter = new BinaryWriter(_mem);
			_bufferWriter = new BinaryBufferWriter(_buffer);
		}
		
		
	}
	
	public class BinaryWriterVsBufferWriter_Int : BinaryWriterVsBufferWriterBase
	{
		[Benchmark(Baseline = true)]
		public void BinaryReader_WriteInt()
		{
			for (int i = 0; i < Loops; i++)
			{
				_mem.Position = 0;

				_binaryWriter.Write(1024);
				_binaryWriter.Write(1024L);
			}
		}

		[Benchmark()]
		public void BufferReader_WriteInt()
		{
			for (int i = 0; i < Loops; i++)
			{
				_bufferWriter.Position = 0;

				_bufferWriter.Write(1024);
				_bufferWriter.Write(1024L);
			}
		}
	}
	
	public class BinaryWriterVsBufferWriter_Decimal : BinaryWriterVsBufferWriterBase
	{
		[Benchmark(Baseline = true)]
		public void BinaryReader_WriteDecimal()
		{
			for (int i = 0; i < Loops; i++)
			{
				_mem.Position = 0;

				_binaryWriter.Write((decimal)1024.1024);
				_binaryWriter.Write((decimal)1024.1024);
			}
		}

		[Benchmark()]
		public void BufferReader_WriteDecimal()
		{
			for (int i = 0; i < Loops; i++)
			{
				_bufferWriter.Position = 0;

				_bufferWriter.Write((decimal)1024.1024);
				_bufferWriter.Write((decimal)1024.1024);
			}
		}
	}

}