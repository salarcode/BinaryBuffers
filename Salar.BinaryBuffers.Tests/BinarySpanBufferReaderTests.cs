using System;
using System.IO;
using Xunit;

namespace Salar.BinaryBuffers.Tests;

public class BinarySpanBufferReaderTests
{
	[Fact]
	public void ConstructorShouldInitializeState()
	{
		ReadOnlySpan<byte> buffer = stackalloc byte[16];
		var reader = new BinarySpanBufferReader(buffer);

		Assert.Equal((0, 16, 0, 16), (reader.Offset, reader.Length, reader.Position, reader.Remaining));
	}

	[Fact]
	public void ResetBufferShouldReplaceInputAndResetPosition()
	{
		var reader = new BinarySpanBufferReader(new byte[8]);
		reader.Position = 4;

		reader.ResetBuffer(new byte[16]);

		Assert.Equal((16, 0, 16), (reader.Length, reader.Position, reader.Remaining));
	}

	[Fact]
	public void ResetBufferShouldResetPosition()
	{
		var reader = new BinarySpanBufferReader(new byte[8]);
		reader.Position = 4;

		reader.ResetBuffer();

		Assert.Equal(0, reader.Position);
	}

	[Theory]
	[InlineData(true)]
	[InlineData(false)]
	public void ReadBooleanShouldReturnWrittenValue(bool value)
	{
		Span<byte> buffer = stackalloc byte[1];
		var writer = new BinarySpanBufferWriter(buffer);
		writer.Write(value);
		var reader = new BinarySpanBufferReader(buffer);

		Assert.Equal(value, reader.ReadBoolean());
	}

	[Fact]
	public void ReadByteShouldReturnWrittenValue()
	{
		ReadOnlySpan<byte> buffer = stackalloc byte[] { byte.MaxValue };
		var reader = new BinarySpanBufferReader(buffer);

		Assert.Equal(byte.MaxValue, reader.ReadByte());
	}

	[Fact]
	public void ReadSByteShouldReturnWrittenValue()
	{
		ReadOnlySpan<byte> buffer = stackalloc byte[] { unchecked((byte)sbyte.MinValue) };
		var reader = new BinarySpanBufferReader(buffer);

		Assert.Equal(sbyte.MinValue, reader.ReadSByte());
	}

	[Fact]
	public void ReadDecimalShouldReturnWrittenValue()
	{
		const decimal value = -12345.6789m;
		Span<byte> buffer = stackalloc byte[16];
		var writer = new BinarySpanBufferWriter(buffer);
		writer.Write(value);
		var reader = new BinarySpanBufferReader(buffer);

		Assert.Equal(value, reader.ReadDecimal());
	}

	[Fact]
	public void ReadSingleShouldReturnWrittenValue()
	{
		const float value = 123.5f;
		Span<byte> buffer = stackalloc byte[sizeof(float)];
		var writer = new BinarySpanBufferWriter(buffer);
		writer.Write(value);
		var reader = new BinarySpanBufferReader(buffer);

		Assert.Equal(value, reader.ReadSingle());
	}

	[Fact]
	public void ReadDoubleShouldReturnWrittenValue()
	{
		const double value = -123.5;
		Span<byte> buffer = stackalloc byte[sizeof(double)];
		var writer = new BinarySpanBufferWriter(buffer);
		writer.Write(value);
		var reader = new BinarySpanBufferReader(buffer);

		Assert.Equal(value, reader.ReadDouble());
	}

	[Fact]
	public void ReadInt16ShouldReturnWrittenValue()
	{
		const short value = short.MinValue;
		Span<byte> buffer = stackalloc byte[sizeof(short)];
		var writer = new BinarySpanBufferWriter(buffer);
		writer.Write(value);
		var reader = new BinarySpanBufferReader(buffer);

		Assert.Equal(value, reader.ReadInt16());
	}

	[Fact]
	public void ReadUInt16ShouldReturnWrittenValue()
	{
		const ushort value = ushort.MaxValue;
		Span<byte> buffer = stackalloc byte[sizeof(ushort)];
		var writer = new BinarySpanBufferWriter(buffer);
		writer.Write(value);
		var reader = new BinarySpanBufferReader(buffer);

		Assert.Equal(value, reader.ReadUInt16());
	}

	[Fact]
	public void ReadInt32ShouldReturnWrittenValue()
	{
		const int value = int.MinValue;
		Span<byte> buffer = stackalloc byte[sizeof(int)];
		var writer = new BinarySpanBufferWriter(buffer);
		writer.Write(value);
		var reader = new BinarySpanBufferReader(buffer);

		Assert.Equal(value, reader.ReadInt32());
	}

	[Fact]
	public void ReadUInt32ShouldReturnWrittenValue()
	{
		const uint value = uint.MaxValue;
		Span<byte> buffer = stackalloc byte[sizeof(uint)];
		var writer = new BinarySpanBufferWriter(buffer);
		writer.Write(value);
		var reader = new BinarySpanBufferReader(buffer);

		Assert.Equal(value, reader.ReadUInt32());
	}

	[Fact]
	public void ReadInt64ShouldReturnWrittenValue()
	{
		const long value = long.MinValue;
		Span<byte> buffer = stackalloc byte[sizeof(long)];
		var writer = new BinarySpanBufferWriter(buffer);
		writer.Write(value);
		var reader = new BinarySpanBufferReader(buffer);

		Assert.Equal(value, reader.ReadInt64());
	}

	[Fact]
	public void ReadUInt64ShouldReturnWrittenValue()
	{
		const ulong value = ulong.MaxValue;
		Span<byte> buffer = stackalloc byte[sizeof(ulong)];
		var writer = new BinarySpanBufferWriter(buffer);
		writer.Write(value);
		var reader = new BinarySpanBufferReader(buffer);

		Assert.Equal(value, reader.ReadUInt64());
	}

	[Fact]
	public void ReadSpanShouldReturnRequestedBytesAndAdvancePosition()
	{
		var reader = new BinarySpanBufferReader(new byte[] { 1, 2, 3, 4 });

		var result = reader.ReadSpan(3);

		Assert.True(result.SequenceEqual(new byte[] { 1, 2, 3 }) && reader.Position == 3);
	}

	[Fact]
	public void ReadBytesShouldReturnRequestedBytes()
	{
		var reader = new BinarySpanBufferReader(new byte[] { 1, 2, 3, 4 });

		var result = reader.ReadBytes(3);

		Assert.Equal(new byte[] { 1, 2, 3 }, result);
	}

	[Fact]
	public void ReadShouldReturnAvailableBytesAtEndOfSpan()
	{
		var reader = new BinarySpanBufferReader(new byte[] { 1, 2, 3 });
		reader.Position = 1;
		var destination = new byte[4];

		var count = reader.Read(destination, 1, 3);

		Assert.True(count == 2 && destination.AsSpan(1, 2).SequenceEqual(new byte[] { 2, 3 }));
	}

	[Fact]
	public void PositionLessThanZeroShouldThrow()
	{
		var reader = new BinarySpanBufferReader(new byte[1]);
		Exception exception = null;

		try
		{
			reader.Position = -1;
		}
		catch (Exception caught)
		{
			exception = caught;
		}

		Assert.IsType<ArgumentOutOfRangeException>(exception);
	}

	[Fact]
	public void PositionGreaterThanLengthShouldThrow()
	{
		var reader = new BinarySpanBufferReader(new byte[1]);
		Exception exception = null;

		try
		{
			reader.Position = 2;
		}
		catch (Exception caught)
		{
			exception = caught;
		}

		Assert.IsType<ArgumentOutOfRangeException>(exception);
	}

	[Fact]
	public void ReadingPastEndShouldThrowEndOfStreamException()
	{
		var reader = new BinarySpanBufferReader(ReadOnlySpan<byte>.Empty);
		Exception exception = null;

		try
		{
			reader.ReadByte();
		}
		catch (Exception caught)
		{
			exception = caught;
		}

		Assert.IsType<EndOfStreamException>(exception);
	}

	[Fact]
	public void ReadingPastEndShouldMovePositionToEnd()
	{
		var reader = new BinarySpanBufferReader(new byte[2]);

		try
		{
			reader.ReadInt32();
		}
		catch (EndOfStreamException)
		{
		}

		Assert.Equal(reader.Length, reader.Position);
	}
}
