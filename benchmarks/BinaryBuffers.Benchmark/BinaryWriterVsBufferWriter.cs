namespace BinaryBuffers.Benchmark
{
    using BenchmarkDotNet.Attributes;

    using System.IO;

    public abstract class BinaryWriterVsBufferWriterBase
    {
        protected const int Loops = 5_000_000;

        protected readonly MemoryStream _memoryStream;
        protected readonly BinaryWriter _binaryWriter;
        protected readonly BinaryBufferWriter _bufferWriter;

        protected BinaryWriterVsBufferWriterBase()
        {
            var buffer = new byte[1024];
            _memoryStream = new MemoryStream(buffer);
            _binaryWriter = new BinaryWriter(_memoryStream);
            _bufferWriter = new BinaryBufferWriter(buffer);
        }


    }

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
    }


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
    }

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
    }

}
