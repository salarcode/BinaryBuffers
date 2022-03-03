namespace BinaryBuffers.Benchmark
{
    using BenchmarkDotNet.Attributes;

    using System.IO;

    public abstract class BinaryReaderVsBufferReaderBase
    {
        protected const int Loops = 5_000_000;

        protected readonly MemoryStream _memoryStream;
        protected readonly BinaryReader _binaryReader;
        protected readonly BinaryBufferReader _bufferReader;

        protected BinaryReaderVsBufferReaderBase()
        {
            var buffer = new byte[1024];
            _memoryStream = new MemoryStream(buffer);
            _binaryReader = new BinaryReader(_memoryStream);
            _bufferReader = new BinaryBufferReader(buffer);
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
    }
}
