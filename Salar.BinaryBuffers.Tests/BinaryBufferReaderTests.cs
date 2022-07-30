using Xunit;
using System;
using System.IO;
using System.Linq;
using Salar.BinaryBuffers.Tests.Fixtures;

namespace Salar.BinaryBuffers.Tests;

public class BinaryBufferReaderTests
{
	public abstract class Constructor<TReaderFixture> : SetupReaderFixture<TReaderFixture>
		where TReaderFixture : ReaderFixture<TReaderFixture>
	{
		public abstract TReaderFixture CreateFixtureWithNullUnderlyingByteArray();
		public abstract TReaderFixture CreateFixture();


		[Fact]
		public virtual void Providing_a_null_underlying_byte_array_should_throw()
		{
			var actualException = Assert.Throws<ArgumentNullException>(() => { Fixture = CreateFixtureWithNullUnderlyingByteArray(); });

			Assert.Equal("data", actualException.ParamName);
		}

		[Fact]
		public virtual void Creating_a_reader_should_succeed()
		{
			Fixture = CreateFixture();
		}
	}

	public class Constructor
	{
		public class ByteArray : Constructor<ReaderFixtureByteArray>
		{
			public override ReaderFixtureByteArray CreateFixtureWithNullUnderlyingByteArray() => new ReaderFixtureByteArray(() => null);

			public override ReaderFixtureByteArray CreateFixture() => new ReaderFixtureByteArray();
		}

		public class ByteArrayWithOffsetAndLength : Constructor<ReaderFixtureByteArray>
		{
			public override ReaderFixtureByteArray CreateFixtureWithNullUnderlyingByteArray() => new ReaderFixtureByteArray(() => null, 1, DefaultDataLength - 1);

			public override ReaderFixtureByteArray CreateFixture() => new ReaderFixtureByteArray(1, DefaultDataLength - 1);

			[Theory]
			[InlineData(0)]
			[InlineData(DefaultDataLength / 2)]
			[InlineData(DefaultDataLength)]
			public void Providing_a_zero_length_should_succeed(int offset)
			{
				Fixture = new ReaderFixtureByteArray(offset, 0);
			}

			[Fact]
			public void Providing_a_negative_offset_should_throw()
			{
				var expectedException = ExceptionHelper.OffsetLessThanZeroException("offset");
				var actualException = Assert.Throws<ArgumentOutOfRangeException>(() => { Fixture = new ReaderFixtureByteArray(-1, 0); });

				Assert.Equal(expectedException.ParamName, actualException.ParamName);
				Assert.Equal(expectedException.Message, actualException.Message);
			}

			[Fact]
			public void Providing_a_negative_length_should_throw()
			{
				var expectedException = ExceptionHelper.LengthLessThanZeroException("length");
				var actualException = Assert.Throws<ArgumentOutOfRangeException>(() => { Fixture = new ReaderFixtureByteArray(0, -1); });

				Assert.Equal(expectedException.ParamName, actualException.ParamName);
				Assert.Equal(expectedException.Message, actualException.Message);
			}

			[Fact]
			public void Providing_an_offset_equal_to_the_length_should_succeed()
			{
				const int length = 1;

				Fixture = new ReaderFixtureByteArray(length, length);
			}

			[Theory]
			[InlineData(0, 0)]
			[InlineData(DefaultDataLength, 0)]
			[InlineData(0, DefaultDataLength)]
			[InlineData(DefaultDataLength / 2, DefaultDataLength / 2)]
			public void Providing_a_length_within_the_bounds_of_the_effective_length_of_the_underlying_byte_array_should_succeed(int offset, int length)
			{
				Fixture = new ReaderFixtureByteArray(offset, length);
			}

			[Theory]
			[InlineData(0, DefaultDataLength + 1)]
			[InlineData(DefaultDataLength, 1)]
			public void Providing_a_length_greater_than_the_effective_length_of_the_underlying_byte_array_should_throw(int offset, int length)
			{
				var expectedException = ExceptionHelper.LengthGreaterThanEffectiveLengthOfByteArrayException();
				var actualException = Assert.Throws<ArgumentException>(() => { Fixture = new ReaderFixtureByteArray(offset, length); });

				Assert.Equal(expectedException.Message, actualException.Message);
			}
		}

		public class ArraySegment : Constructor<ReaderFixtureArraySegment>
		{
			public override ReaderFixtureArraySegment CreateFixtureWithNullUnderlyingByteArray() => new ReaderFixtureArraySegment(new ArraySegment<byte>());

			public override ReaderFixtureArraySegment CreateFixture() => new ReaderFixtureArraySegment();
		}

		public class ArraySegmentWithOffsetAndLength : Constructor<ReaderFixtureArraySegment>
		{
			public override ReaderFixtureArraySegment CreateFixtureWithNullUnderlyingByteArray() => throw new NotImplementedException();

			public override ReaderFixtureArraySegment CreateFixture() => new ReaderFixtureArraySegment(1, DefaultDataLength - 1);

			public override void Providing_a_null_underlying_byte_array_should_throw() { }
		}
	}


	public abstract class Position<TReaderFixture> : SetupReaderFixture<TReaderFixture>
		where TReaderFixture : ReaderFixture<TReaderFixture>
	{
		public abstract TReaderFixture CreateFixture();


		internal Position()
		{
			Init();
		}

		private void Init()
		{
			Fixture = CreateFixture();
		}

		[Fact]
		public void Setting_a_negative_position_should_throw()
		{
			var expectedException = ExceptionHelper.PositionLessThanZeroException("value");
			var actualException = Assert.Throws<ArgumentOutOfRangeException>(() => { Fixture.BufferReader.Position = -1; });

			Assert.Equal(expectedException.ParamName, actualException.ParamName);
			Assert.Equal(expectedException.Message, actualException.Message);
		}

		[Fact]
		public void Setting_a_position_greater_than_the_length_of_the_underlying_byte_array_should_throw()
		{
			var expectedException = ExceptionHelper.PositionGreaterThanLengthOfByteArrayException("value");
			var actualException = Assert.Throws<ArgumentOutOfRangeException>(() => { Fixture.BufferReader.Position = Fixture.BufferReader.Length + 1; });

			Assert.Equal(expectedException.ParamName, actualException.ParamName);
			Assert.Equal(expectedException.Message, actualException.Message);
		}

		[Fact]
		public void Setting_a_position_equal_to_the_length_of_the_underlying_byte_array_should_succeed()
		{
			Fixture.BufferReader.Position = Fixture.BufferReader.Length;
		}

		[Fact]
		public void Should_set_expected_position()
		{
			var positions = new[] { 0, 1, Fixture.BufferReader.Length };

			foreach (var position in positions)
			{
				Fixture.BufferReader.Position = position;

				Assert.Equal(position, Fixture.BufferReader.Position);
			}
		}
	}

	public class Position
	{
		public class ByteArray : Position<ReaderFixtureByteArray>
		{
			public override ReaderFixtureByteArray CreateFixture() => new ReaderFixtureByteArray();
		}

		public class ByteArrayWithOffsetAndLength : Position<ReaderFixtureByteArray>
		{
			public override ReaderFixtureByteArray CreateFixture() => new ReaderFixtureByteArray(1, DefaultDataLength - 1);
		}

		public class ArraySegment : Position<ReaderFixtureArraySegment>
		{
			public override ReaderFixtureArraySegment CreateFixture() => new ReaderFixtureArraySegment();
		}

		public class ArraySegmentWithOffsetAndLength : Position<ReaderFixtureArraySegment>
		{
			public override ReaderFixtureArraySegment CreateFixture() => new ReaderFixtureArraySegment(1, DefaultDataLength - 1);
		}
	}


	public abstract class Reading<TReaderFixture> : SetupReaderFixture<TReaderFixture>
		where TReaderFixture : ReaderFixture<TReaderFixture>
	{
		public abstract TReaderFixture CreateFixture();


		internal Reading()
		{
			Init();
		}

		private void Init()
		{
			Fixture = CreateFixture();
		}


		[Theory]
		[InlineData(true)]
		[InlineData(false)]
		public void ReadBoolean(bool input)
		{
			Fixture.NativeWriter.Write(input);

			var val = Fixture.BufferReader.ReadBoolean();

			Assert.Equal(input, val);
			Assert.Equal(1, Fixture.BufferReader.Position);
		}

		[Theory]
		[InlineData(0)]
		[InlineData(1)]
		[InlineData(byte.MaxValue)]
		[InlineData(byte.MaxValue / 2)]
		public void ReadByte(byte input)
		{
			Fixture.NativeWriter.Write(input);

			var val = Fixture.BufferReader.ReadByte();

			Assert.Equal(input, val);
			Assert.Equal(1, Fixture.BufferReader.Position);
		}

		[Theory]
		[InlineData(0)]
		[InlineData(1)]
		[InlineData(100)]
		public void ReadBytes(byte input)
		{
			var buff = new byte[input];
			var element = (byte)(input / 2);
			Array.Fill(buff, element);

			Fixture.NativeWriter.Write(buff);

			var val = Fixture.BufferReader.ReadBytes(input);

			Assert.Equal(input, val.Length);
			Assert.Equal(buff, val);
			Assert.Equal(input, Fixture.BufferReader.Position);
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
		[InlineData((float)(int.MinValue / 2.5))]
		[InlineData(int.MaxValue)]
		[InlineData((float)(int.MaxValue / 2.5))]
		public void ReadDecimal(object inputObj)
		{
			var input = Convert.ToDecimal(inputObj);
			Fixture.NativeWriter.Write(input);

			var val = Fixture.BufferReader.ReadDecimal();

			Assert.Equal(input, val);
			Assert.Equal(16, Fixture.BufferReader.Position);
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
			Fixture.NativeWriter.Write(input);

			var val = Fixture.BufferReader.ReadDouble();

			Assert.Equal(input, val);
			Assert.Equal(8, Fixture.BufferReader.Position);
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
			Fixture.NativeWriter.Write(input);

			var val = Fixture.BufferReader.ReadInt16();

			Assert.Equal(input, val);
			Assert.Equal(2, Fixture.BufferReader.Position);
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
			Fixture.NativeWriter.Write(input);

			var val = Fixture.BufferReader.ReadInt32();

			Assert.Equal(input, val);
			Assert.Equal(4, Fixture.BufferReader.Position);
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
			Fixture.NativeWriter.Write(input);

			var val = Fixture.BufferReader.ReadInt64();

			Assert.Equal(input, val);
			Assert.Equal(8, Fixture.BufferReader.Position);
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
			Fixture.NativeWriter.Write(input);

			var val = Fixture.BufferReader.ReadSByte();

			Assert.Equal(input, val);
			Assert.Equal(1, Fixture.BufferReader.Position);
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
			Fixture.NativeWriter.Write(input);

			var val = Fixture.BufferReader.ReadSingle();

			Assert.Equal(input, val);
			Assert.Equal(4, Fixture.BufferReader.Position);
		}

		[Theory]
		[InlineData(0)]
		[InlineData(1)]
		[InlineData(150)]
		public void ReadSpan(int input)
		{
			var buff = new byte[input];
			var element = (byte)(input / 2);

			Array.Fill(buff, element);

			Fixture.NativeWriter.Write(buff);

			var val = Fixture.BufferReader.ReadSpan(input);

			Assert.Equal(input, val.Length);

			for (int i = 0; i < buff.Length; i++)
			{
				Assert.Equal(buff[i], val[i]);
			}

			Assert.Equal(input, Fixture.BufferReader.Position);
		}

		[Theory]
		[InlineData(0)]
		[InlineData(byte.MaxValue)]
		[InlineData(ushort.MaxValue)]
		[InlineData(ushort.MaxValue / 2)]
		public void ReadUInt16(ushort input)
		{
			Fixture.NativeWriter.Write(input);

			var val = Fixture.BufferReader.ReadUInt16();

			Assert.Equal(input, val);
			Assert.Equal(2, Fixture.BufferReader.Position);
		}

		[Theory]
		[InlineData(0)]
		[InlineData(byte.MaxValue)]
		[InlineData(ushort.MaxValue)]
		[InlineData(uint.MaxValue)]
		[InlineData(uint.MaxValue / 2)]
		public void ReadUInt32(uint input)
		{
			Fixture.NativeWriter.Write(input);

			var val = Fixture.BufferReader.ReadUInt32();

			Assert.Equal(input, val);
			Assert.Equal(4, Fixture.BufferReader.Position);
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
			Fixture.NativeWriter.Write(input);

			var val = Fixture.BufferReader.ReadUInt64();

			Assert.Equal(input, val);
			Assert.Equal(8, Fixture.BufferReader.Position);
		}

		[Fact]
		public void Attempting_to_read_a_byte_outside_the_readable_region_of_the_underlying_byte_array_should_throw()
		{
			Fixture.BufferReader.Position = Fixture.BufferReader.Length;

			var expectedException = ExceptionHelper.EndOfDataException();
			var actualException = Assert.Throws<EndOfStreamException>(() => _ = Fixture.BufferReader.ReadByte());

			Assert.Equal(expectedException.Message, actualException.Message);
		}

		[Theory]
		[InlineData(0, 1)]
		[InlineData(-10, 20)]
		public void Attempting_to_read_a_span_outside_the_readable_region_of_the_underlying_byte_array_should_throw(int offsetFromEndOfUnderlyingByteArray, int readLength)
		{
			Fixture.BufferReader.Position = Fixture.BufferReader.Length + offsetFromEndOfUnderlyingByteArray;

			var expectedException = ExceptionHelper.EndOfDataException();
			var actualException = Assert.Throws<EndOfStreamException>(() => _ = Fixture.BufferReader.ReadSpan(readLength));

			Assert.Equal(expectedException.Message, actualException.Message);
		}

		[Fact]
		public void Attempting_to_read_a_span_with_a_negative_length_should_return_an_empty_span()
		{
			var actualSpan = Fixture.BufferReader.ReadSpan(-1);

			Assert.True(actualSpan.SequenceEqual(ReadOnlySpan<byte>.Empty));
		}

		[Fact]
		public void Reading_a_decimal_value_with_an_invalid_representation_should_throw()
		{
			var data = Enumerable.Range(0, 16).Select((_, i) => (byte)(i & 1)).ToArray();

			Fixture.NativeWriter.Write(data);

			var expectedException = ExceptionHelper.DecimalReadingException(null);
			var actualException = Assert.Throws<IOException>(() => _ = Fixture.BufferReader.ReadDecimal());

			Assert.Equal(expectedException.Message, actualException.Message);
		}
	}

	public class Reading
	{
		public class ByteArray : Reading<ReaderFixtureByteArray>
		{
			public override ReaderFixtureByteArray CreateFixture() => new ReaderFixtureByteArray();
		}

		public class ByteArrayWithOffsetAndLength : Reading<ReaderFixtureByteArray>
		{
			public override ReaderFixtureByteArray CreateFixture() => new ReaderFixtureByteArray(1, DefaultDataLength - 1);
		}

		public class ArraySegment : Reading<ReaderFixtureArraySegment>
		{
			public override ReaderFixtureArraySegment CreateFixture() => new ReaderFixtureArraySegment();
		}

		public class ArraySegmentWithOffsetAndLength : Reading<ReaderFixtureArraySegment>
		{
			public override ReaderFixtureArraySegment CreateFixture() => new ReaderFixtureArraySegment(1, DefaultDataLength - 1);
		}
	}
}
