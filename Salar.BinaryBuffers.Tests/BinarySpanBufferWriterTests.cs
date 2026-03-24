using Xunit;
using System;
using System.IO;
using System.Linq;
using Salar.BinaryBuffers.Tests.Fixtures;

namespace Salar.BinaryBuffers.Tests;

/// <summary>
/// Tests for BinarySpanBufferWriter - a ref struct that writes to Span&lt;byte&gt;.
/// Note: Ref structs cannot be captured in lambdas, so we use try-catch for exception tests.
/// </summary>
public class BinarySpanBufferWriterTests
{
	private const int DefaultDataLength = SpanWriterFixture.DefaultDataLength;

	public class BasicFunctionalityTests : IDisposable
	{
		private readonly SpanWriterFixture _fixture;

		public BasicFunctionalityTests()
		{
			_fixture = new SpanWriterFixture();
		}

		[Fact]
		public void Constructor_should_initialize_correctly()
		{
			Span<byte> buffer = stackalloc byte[1024];
			var writer = new BinarySpanBufferWriter(buffer);

			Assert.Equal(1024, writer.Length);
			Assert.Equal(0, writer.Position);
			Assert.Equal(0, writer.WrittenLength);
			Assert.Equal(0, writer.Offset);
		}

		[Fact]
		public void WrittenLength_should_track_written_bytes()
		{
			var writer = _fixture.CreateWriter();
			Assert.Equal(0, writer.WrittenLength);

			writer.Write(42); // 4 bytes
			Assert.Equal(4, writer.WrittenLength);

			writer.Write((short)100); // 2 bytes
			Assert.Equal(6, writer.WrittenLength);
		}

		[Fact]
		public void Position_should_advance_after_writing()
		{
			var writer = _fixture.CreateWriter();
			writer.Write(42);
			Assert.Equal(4, writer.Position);

			writer.Write((short)100);
			Assert.Equal(6, writer.Position);
		}

		[Fact]
		public void ResetBuffer_should_reset_state()
		{
			var writer = _fixture.CreateWriter();
			writer.Write(42);
			writer.Write(100L);

			writer.ResetBuffer();

			Assert.Equal(0, writer.Position);
			Assert.Equal(0, writer.WrittenLength);
		}

		public void Dispose()
		{
			_fixture?.Dispose();
		}
	}

	public class PrimitiveTypeWritingTests : IDisposable
	{
		private readonly SpanWriterFixture _fixture;

		public PrimitiveTypeWritingTests()
		{
			_fixture = new SpanWriterFixture();
		}

		[Theory]
		[InlineData(true)]
		[InlineData(false)]
		public void WriteBoolean(bool input)
		{
			var writer = _fixture.CreateWriter();
			writer.Write(input);
			Assert.Equal(input, _fixture.NativeReader.ReadBoolean());
		}

		[Theory]
		[InlineData(0)]
		[InlineData(byte.MaxValue)]
		public void WriteByte(byte input)
		{
			var writer = _fixture.CreateWriter();
			writer.Write(input);
			Assert.Equal(input, _fixture.NativeReader.ReadByte());
		}

		[Theory]
		[InlineData(0)]
		[InlineData(sbyte.MinValue)]
		[InlineData(sbyte.MaxValue)]
		public void WriteSByte(sbyte input)
		{
			var writer = _fixture.CreateWriter();
			writer.Write(input);
			Assert.Equal(input, _fixture.NativeReader.ReadSByte());
		}

		[Theory]
		[InlineData(0)]
		[InlineData(short.MinValue)]
		[InlineData(short.MaxValue)]
		public void WriteInt16(short input)
		{
			var writer = _fixture.CreateWriter();
			writer.Write(input);
			Assert.Equal(input, _fixture.NativeReader.ReadInt16());
		}

		[Theory]
		[InlineData(0)]
		[InlineData(ushort.MaxValue)]
		public void WriteUInt16(ushort input)
		{
			var writer = _fixture.CreateWriter();
			writer.Write(input);
			Assert.Equal(input, _fixture.NativeReader.ReadUInt16());
		}

		[Theory]
		[InlineData(0)]
		[InlineData(int.MinValue)]
		[InlineData(int.MaxValue)]
		public void WriteInt32(int input)
		{
			var writer = _fixture.CreateWriter();
			writer.Write(input);
			Assert.Equal(input, _fixture.NativeReader.ReadInt32());
		}

		[Theory]
		[InlineData(0)]
		[InlineData(uint.MaxValue)]
		public void WriteUInt32(uint input)
		{
			var writer = _fixture.CreateWriter();
			writer.Write(input);
			Assert.Equal(input, _fixture.NativeReader.ReadUInt32());
		}

		[Theory]
		[InlineData(0)]
		[InlineData(long.MinValue)]
		[InlineData(long.MaxValue)]
		public void WriteInt64(long input)
		{
			var writer = _fixture.CreateWriter();
			writer.Write(input);
			Assert.Equal(input, _fixture.NativeReader.ReadInt64());
		}

		[Theory]
		[InlineData(0)]
		[InlineData(ulong.MaxValue)]
		public void WriteUInt64(ulong input)
		{
			var writer = _fixture.CreateWriter();
			writer.Write(input);
			Assert.Equal(input, _fixture.NativeReader.ReadUInt64());
		}

		[Theory]
		[InlineData(0)]
		[InlineData(3.14f)]
		[InlineData(float.MinValue / 1.5)]
		[InlineData(float.MaxValue / 1.5)]
		public void WriteSingle(float input)
		{
			var writer = _fixture.CreateWriter();
			writer.Write(input);
			Assert.Equal(input, _fixture.NativeReader.ReadSingle());
		}

		[Theory]
		[InlineData(0)]
		[InlineData(3.14)]
		[InlineData(double.MinValue / 1.5)]
		[InlineData(double.MaxValue / 1.5)]
		public void WriteDouble(double input)
		{
			var writer = _fixture.CreateWriter();
			writer.Write(input);
			Assert.Equal(input, _fixture.NativeReader.ReadDouble());
		}

		[Theory]
		[InlineData(0)]
		[InlineData(123.45)]
		[InlineData(-123.45)]
		public void WriteDecimal(decimal input)
		{
			var writer = _fixture.CreateWriter();
			writer.Write(input);
			Assert.Equal(input, _fixture.NativeReader.ReadDecimal());
		}

		public void Dispose()
		{
			_fixture?.Dispose();
		}
	}

	public class BufferWritingTests : IDisposable
	{
		private readonly SpanWriterFixture _fixture;

		public BufferWritingTests()
		{
			_fixture = new SpanWriterFixture();
		}

		[Theory]
		[InlineData(0)]
		[InlineData(1)]
		[InlineData(100)]
		[InlineData(DefaultDataLength)]
		public void WriteByteArray(int dataLength)
		{
			var writer = _fixture.CreateWriter();
			var buff = new byte[dataLength];
			Array.Fill(buff, (byte)0xFF);

			writer.Write(buff);
			var val = _fixture.NativeReader.ReadBytes(dataLength);

			Assert.True(val.SequenceEqual(buff));
		}

		[Theory]
		[InlineData(10, 0, 10)]
		[InlineData(10, 5, 5)]
		public void WriteByteArraySlice(int originalLength, int offset, int length)
		{
			var writer = _fixture.CreateWriter();
			var buff = new byte[originalLength];
			Array.Fill(buff, (byte)0xAB);

			writer.Write(buff, offset, length);
			var val = _fixture.NativeReader.ReadBytes(length);

			Assert.True(val.SequenceEqual(buff[offset..(offset + length)]));
		}

		[Theory]
		[InlineData(0)]
		[InlineData(1)]
		[InlineData(100)]
		public void WriteReadOnlySpan(int dataLength)
		{
			var writer = _fixture.CreateWriter();
			var buff = new byte[dataLength];
			Array.Fill(buff, (byte)0xCD);

			writer.Write(buff.AsSpan());
			var val = _fixture.NativeReader.ReadBytes(dataLength);

			Assert.True(val.SequenceEqual(buff));
		}

		[Fact]
		public void WriteNull_should_throw()
		{
			var writer = _fixture.CreateWriter();
			bool threwException = false;
			
			try { writer.Write(null as byte[]); }
			catch (ArgumentNullException ex) 
			{ 
				threwException = true;
				Assert.Equal("buffer", ex.ParamName);
			}
			
			Assert.True(threwException);
		}

		public void Dispose()
		{
			_fixture?.Dispose();
		}
	}

	public class UtilityMethodsTests : IDisposable
	{
		private readonly SpanWriterFixture _fixture;

		public UtilityMethodsTests()
		{
			_fixture = new SpanWriterFixture();
		}

		[Fact]
		public void ToReadOnlySpan_should_return_written_data()
		{
			var writer = _fixture.CreateWriter();
			writer.Write(42);
			writer.Write((short)100);

			var span = writer.ToReadOnlySpan();
			
			Assert.Equal(6, span.Length);
			_fixture.NativeReader.BaseStream.Position = 0;
			var expected = _fixture.NativeReader.ReadBytes(6);
			Assert.True(span.SequenceEqual(expected));
		}

		[Fact]
		public void ToArray_should_return_written_data_as_array()
		{
			var writer = _fixture.CreateWriter();
			writer.Write(42);
			writer.Write((short)100);

			var array = writer.ToArray();
			
			Assert.Equal(6, array.Length);
			_fixture.NativeReader.BaseStream.Position = 0;
			var expected = _fixture.NativeReader.ReadBytes(6);
			Assert.True(array.SequenceEqual(expected));
		}

		[Fact]
		public void RemainingToReadOnlySpan_should_return_remaining_data()
		{
			var writer = _fixture.CreateWriter();
			writer.Write(42);
			writer.Write((short)100);
			writer.Position = 2;

			var span = writer.RemainingToReadOnlySpan();
			Assert.Equal(4, span.Length);
		}

		public void Dispose()
		{
			_fixture?.Dispose();
		}
	}

	public class StackallocTests
	{
		[Fact]
		public void Should_work_with_stackalloc()
		{
			Span<byte> buffer = stackalloc byte[1024];
			var writer = new BinarySpanBufferWriter(buffer);

			writer.Write(42);
			writer.Write(3.14);
			writer.Write(true);

			Assert.Equal(13, writer.WrittenLength);
			var result = writer.ToReadOnlySpan();
			Assert.Equal(13, result.Length);
		}

		[Fact]
		public void Should_work_with_array_slice()
		{
			var array = new byte[1024];
			var writer = new BinarySpanBufferWriter(array.AsSpan(100, 500));

			Assert.Equal(500, writer.Length);
			Assert.Equal(0, writer.Offset);

			writer.Write(42);
			Assert.Equal(4, writer.WrittenLength);
			Assert.Equal(42, BitConverter.ToInt32(array, 100));
		}
	}

	public class InterfaceImplementationTests
	{
		[Fact]
		public void Should_implement_IBufferWriter_methods()
		{
			Span<byte> buffer = stackalloc byte[100];
			var writer = new BinarySpanBufferWriter(buffer);

			// All IBufferWriter methods should work
			writer.Write(true);
			writer.Write((byte)42);
			writer.Write((sbyte)-1);
			writer.Write((short)100);
			writer.Write((ushort)200);
			writer.Write(300);
			writer.Write(400U);
			writer.Write(500L);
			writer.Write(600UL);
			writer.Write(1.5f);
			writer.Write(2.5);
			writer.Write(3.5m);
			writer.Write(new byte[] { 1, 2, 3 });
			writer.Write(new byte[] { 4, 5, 6 }, 0, 3);
			writer.Write(new byte[] { 7, 8, 9 }.AsSpan());

			Assert.True(writer.WrittenLength > 0);
		}
	}
}
