namespace BinaryBuffers.Tests
{
    using Xunit;

    using System;
    using System.IO;
    using System.Text;

    public class BinaryBufferWriterTests : IDisposable
    {
        private readonly MemoryStream _memoryStream;
        private readonly BinaryReader _reader;
        private readonly BinaryBufferWriter _bufferWriter;

        public BinaryBufferWriterTests()
        {
            var data = new byte[1024];
            _memoryStream = new MemoryStream(data);
            _reader = new BinaryReader(_memoryStream, Encoding.UTF8, true);
            _bufferWriter = new BinaryBufferWriter(data);
        }

        private void Reset()
        {
            _memoryStream.Position = 0;
            _bufferWriter.Position = 0;
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(byte.MaxValue)]
        [InlineData(short.MinValue)]
        [InlineData(short.MinValue / 2)]
        [InlineData(short.MaxValue)]
        [InlineData(short.MaxValue / 2)]
        public void ReadInt16(short input)
        {
            Reset();
            _bufferWriter.Write(input);

            var val = _reader.ReadInt16();

            Assert.Equal(input, val);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(byte.MaxValue)]
        [InlineData(ushort.MaxValue)]
        [InlineData(ushort.MaxValue / 2)]
        public void ReadUInt16(ushort input)
        {
            Reset();
            _bufferWriter.Write(input);

            var val = _reader.ReadUInt16();

            Assert.Equal(input, val);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(byte.MaxValue)]
        [InlineData(short.MaxValue)]
        [InlineData(short.MinValue)]
        [InlineData(int.MinValue)]
        [InlineData(int.MinValue / 2)]
        [InlineData(int.MaxValue)]
        [InlineData(int.MaxValue / 2)]
        public void ReadInt32(int input)
        {
            Reset();
            _bufferWriter.Write(input);

            var val = _reader.ReadInt32();

            Assert.Equal(input, val);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(byte.MaxValue)]
        [InlineData(ushort.MaxValue)]
        [InlineData(uint.MaxValue)]
        [InlineData(uint.MaxValue / 2)]
        public void ReadUInt32(uint input)
        {
            Reset();
            _bufferWriter.Write(input);

            var val = _reader.ReadUInt32();

            Assert.Equal(input, val);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(byte.MaxValue)]
        [InlineData(short.MaxValue)]
        [InlineData(short.MinValue)]
        [InlineData(int.MinValue)]
        [InlineData(int.MinValue / 2)]
        [InlineData(int.MaxValue)]
        [InlineData(int.MaxValue / 2)]
        [InlineData(long.MinValue)]
        [InlineData(long.MinValue / 2)]
        [InlineData(long.MaxValue)]
        [InlineData(long.MaxValue / 2)]
        public void ReadInt64(long input)
        {
            Reset();
            _bufferWriter.Write(input);

            var val = _reader.ReadInt64();

            Assert.Equal(input, val);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(byte.MaxValue)]
        [InlineData(ushort.MaxValue)]
        [InlineData(uint.MaxValue)]
        [InlineData(uint.MaxValue / 2)]
        [InlineData(ulong.MaxValue)]
        [InlineData(ulong.MaxValue / 2)]
        public void ReadUInt64(ulong input)
        {
            Reset();
            _bufferWriter.Write(input);

            var val = _reader.ReadUInt64();

            Assert.Equal(input, val);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(0.1)]
        [InlineData(-1)]
        [InlineData(-1.1)]
        [InlineData(byte.MaxValue)]
        [InlineData(short.MaxValue)]
        [InlineData(short.MinValue)]
        [InlineData(int.MinValue)]
        [InlineData(int.MinValue / 1.5)]
        [InlineData(int.MaxValue)]
        [InlineData(int.MaxValue / 1.5)]
        [InlineData(float.MinValue)]
        [InlineData(float.MinValue / 1.5)]
        [InlineData(float.MaxValue)]
        [InlineData(float.MaxValue / 1.5)]
        public void ReadSingle(float input)
        {
            Reset();
            _bufferWriter.Write(input);

            var val = _reader.ReadSingle();

            Assert.Equal(input, val);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(0.1)]
        [InlineData(-1)]
        [InlineData(-1.1)]
        [InlineData(byte.MaxValue)]
        [InlineData(short.MaxValue)]
        [InlineData(short.MinValue)]
        [InlineData(int.MinValue)]
        [InlineData(int.MinValue / 1.5)]
        [InlineData(int.MaxValue)]
        [InlineData(int.MaxValue / 1.5)]
        [InlineData(double.MinValue)]
        [InlineData(double.MinValue / 1.5)]
        [InlineData(double.MaxValue)]
        [InlineData(double.MaxValue / 1.5)]
        public void ReadDouble(double input)
        {
            Reset();
            _bufferWriter.Write(input);

            var val = _reader.ReadDouble();

            Assert.Equal(input, val);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(0.1)]
        [InlineData(-1)]
        [InlineData(-1.1)]
        [InlineData(byte.MaxValue)]
        [InlineData(short.MaxValue)]
        [InlineData(short.MinValue)]
        [InlineData(int.MinValue)]
        [InlineData((float) (int.MinValue / 2.5))]
        [InlineData(int.MaxValue)]
        [InlineData((float) (int.MaxValue / 2.5))]
        public void ReadDecimal(object inputObj)
        {
            Reset();
            var input = Convert.ToDecimal(inputObj);
            _bufferWriter.Write(input);

            var val = _reader.ReadDecimal();

            Assert.Equal(input, val);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(byte.MaxValue)]
        [InlineData(byte.MaxValue / 2)]
        public void ReadByte(byte input)
        {
            Reset();
            _bufferWriter.Write(input);

            var val = _reader.ReadByte();

            Assert.Equal(input, val);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(100)]
        public void ReadBytes(byte input)
        {
            Reset();
            var buff = new byte[input];
            var element = (byte) (input / 2);
            Array.Fill(buff, element);

            _bufferWriter.Write(buff);

            var val = _reader.ReadBytes(input);

            Assert.Equal(input, val.Length);
            Assert.Equal(buff, val);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(-1)]
        [InlineData(sbyte.MinValue)]
        [InlineData(sbyte.MinValue / 2)]
        [InlineData(sbyte.MaxValue)]
        [InlineData(sbyte.MaxValue / 2)]
        public void ReadSByte(sbyte input)
        {
            Reset();
            _bufferWriter.Write(input);

            var val = _reader.ReadSByte();

            Assert.Equal(input, val);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void ReadBoolean(bool input)
        {
            Reset();
            _bufferWriter.Write(input);

            var val = _reader.ReadBoolean();

            Assert.Equal(input, val);
        }


        public void Dispose()
        {
            _memoryStream?.Dispose();
        }
    }
}
