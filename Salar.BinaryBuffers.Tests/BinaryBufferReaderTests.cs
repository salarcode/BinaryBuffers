using System;
using System.IO;
using System.Text;
using Xunit;

namespace Salar.BinaryBuffers.Tests
{
	public class BinaryBufferReaderTests : IDisposable
	{
        private readonly MemoryStream _memoryStream;
		private readonly BinaryWriter _writer;
		private readonly BinaryBufferReader _bufferReader;

		public BinaryBufferReaderTests()
		{
            var data = new byte[1024];
			_memoryStream = new MemoryStream(data);
			_writer = new BinaryWriter(_memoryStream, Encoding.UTF8, true);
			_bufferReader = new BinaryBufferReader(data);
		}

		private void Reset()
		{
			_memoryStream.Position = 0;
			_memoryStream.SetLength(0);
			_bufferReader.Position = 0;
		}

		[Theory]
		[InlineData(0)]
		[InlineData(-1)]
		[InlineData(byte.MaxValue)]
		[InlineData(Int16.MinValue)]
		[InlineData(Int16.MinValue / 2)]
		[InlineData(Int16.MaxValue)]
		[InlineData(Int16.MaxValue / 2)]
		public void ReadInt16(Int16 input)
		{
			Reset();
			_writer.Write(input);

			var val = _bufferReader.ReadInt16();

			Assert.Equal(input, val);
		}

		[Theory]
		[InlineData(0)]
		[InlineData(byte.MaxValue)]
		[InlineData(UInt16.MaxValue)]
		[InlineData(UInt16.MaxValue / 2)]
		public void ReadUInt16(UInt16 input)
		{
			Reset();
			_writer.Write(input);

			var val = _bufferReader.ReadUInt16();

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
			_writer.Write(input);

			var val = _bufferReader.ReadInt32();

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
			_writer.Write(input);

			var val = _bufferReader.ReadUInt32();

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
			_writer.Write(input);

			var val = _bufferReader.ReadInt64();

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
			_writer.Write(input);

			var val = _bufferReader.ReadUInt64();

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
			_writer.Write(input);

			var val = _bufferReader.ReadSingle();

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
			_writer.Write(input);

			var val = _bufferReader.ReadDouble();

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
			_writer.Write(input);

			var val = _bufferReader.ReadDecimal();

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
			_writer.Write(input);

			var val = _bufferReader.ReadByte();

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

			_writer.Write(buff);

			var val = _bufferReader.ReadBytes(input);

			Assert.Equal(input, val.Length);
			Assert.Equal(buff, val);
		}


		[Theory]
		[InlineData(0)]
		[InlineData(1)]
		[InlineData(150)]
		public void ReadSpan(int input)
		{
			Reset();
			var buff = new byte[input];
			var element = (byte) (input / 2);
			var elementSpan = new ReadOnlySpan<byte>(buff);
			Array.Fill(buff, element);

			_writer.Write(buff);

			var val = _bufferReader.ReadSpan(input);

			Assert.Equal(input, val.Length);

			for (int i = 0; i < buff.Length; i++)
			{
				Assert.Equal(buff[i], val[i]);
			}
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
			_writer.Write(input);

			var val = _bufferReader.ReadSByte();

			Assert.Equal(input, val);
		}

		[Theory]
		[InlineData(true)]
		[InlineData(false)]
		public void ReadBoolean(bool input)
		{
			Reset();
			_writer.Write(input);

			var val = _bufferReader.ReadBoolean();

			Assert.Equal(input, val);
		}


		public void Dispose()
		{
			_memoryStream?.Dispose();
		}
	}
}
