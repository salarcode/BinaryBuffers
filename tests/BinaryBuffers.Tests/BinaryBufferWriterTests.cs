namespace BinaryBuffers.Tests
{
    using Fixtures;

    using Xunit;

    using System;
    using System.IO;
    using System.Linq;

    public class BinaryBufferWriterTests
    {
        public abstract class Constructor<TWriterFixture> : SetupWriterFixture<TWriterFixture>
            where TWriterFixture : WriterFixture<TWriterFixture>
        {
            public abstract TWriterFixture CreateFixtureWithNullUnderlyingByteArray();
            public abstract TWriterFixture CreateFixture();


            [Fact]
            public virtual void Providing_a_null_underlying_byte_array_should_throw()
            {
                var actualException = Assert.Throws<ArgumentNullException>(() => { Fixture = CreateFixtureWithNullUnderlyingByteArray(); });

                Assert.Equal("buffer", actualException.ParamName);
            }

            [Fact]
            public virtual void Creating_a_writer_should_succeed()
            {
                Fixture = CreateFixture();
            }
        }

        public class Constructor
        {
            public class ByteArray : Constructor<WriterFixtureByteArray>
            {
                public override WriterFixtureByteArray CreateFixtureWithNullUnderlyingByteArray() => new WriterFixtureByteArray(() => null);

                public override WriterFixtureByteArray CreateFixture() => new WriterFixtureByteArray();
            }

            public class ByteArrayWithOffsetAndLength : Constructor<WriterFixtureByteArray>
            {
                public override WriterFixtureByteArray CreateFixtureWithNullUnderlyingByteArray() => new WriterFixtureByteArray(() => null, 1, DefaultDataLength - 1);

                public override WriterFixtureByteArray CreateFixture() => new WriterFixtureByteArray(1, DefaultDataLength - 1);

                [Theory]
                [InlineData(0)]
                [InlineData(DefaultDataLength / 2)]
                [InlineData(DefaultDataLength)]
                public void Providing_a_zero_length_should_succeed(int offset)
                {
                    Fixture = new WriterFixtureByteArray(offset, 0);
                }

                [Fact]
                public void Providing_a_negative_offset_should_throw()
                {
                    var expectedException = ExceptionHelper.OffsetLessThanZeroException("offset");
                    var actualException = Assert.Throws<ArgumentOutOfRangeException>(() => { Fixture = new WriterFixtureByteArray(-1, 0); });

                    Assert.Equal(expectedException.ParamName, actualException.ParamName);
                    Assert.Equal(expectedException.Message, actualException.Message);
                }

                [Fact]
                public void Providing_a_negative_length_should_throw()
                {
                    var expectedException = ExceptionHelper.LengthLessThanZeroException("length");
                    var actualException = Assert.Throws<ArgumentOutOfRangeException>(() => { Fixture = new WriterFixtureByteArray(0, -1); });

                    Assert.Equal(expectedException.ParamName, actualException.ParamName);
                    Assert.Equal(expectedException.Message, actualException.Message);
                }

                [Fact]
                public void Providing_an_offset_equal_to_the_length_should_succeed()
                {
                    const int length = 1;

                    Fixture = new WriterFixtureByteArray(length, length);
                }

                [Theory]
                [InlineData(0, 0)]
                [InlineData(DefaultDataLength, 0)]
                [InlineData(0, DefaultDataLength)]
                [InlineData(DefaultDataLength / 2, DefaultDataLength / 2)]
                public void Providing_a_length_within_the_bounds_of_the_effective_length_of_the_underlying_byte_array_should_succeed(int offset, int length)
                {
                    Fixture = new WriterFixtureByteArray(offset, length);
                }

                [Theory]
                [InlineData(0, DefaultDataLength + 1)]
                [InlineData(DefaultDataLength, 1)]
                public void Providing_a_length_greater_than_the_effective_length_of_the_underlying_byte_array_should_throw(int offset, int length)
                {
                    var expectedException = ExceptionHelper.LengthGreaterThanEffectiveLengthOfByteArrayException();
                    var actualException = Assert.Throws<ArgumentException>(() => { Fixture = new WriterFixtureByteArray(offset, length); });

                    Assert.Equal(expectedException.Message, actualException.Message);
                }
            }
        }


        public abstract class Position<TWriterFixture> : SetupWriterFixture<TWriterFixture>
            where TWriterFixture : WriterFixture<TWriterFixture>
        {
            public abstract TWriterFixture CreateFixture();


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
                var actualException = Assert.Throws<ArgumentOutOfRangeException>(() => { Fixture.BufferWriter.Position = -1; });

                Assert.Equal(expectedException.ParamName, actualException.ParamName);
                Assert.Equal(expectedException.Message, actualException.Message);
            }

            [Fact]
            public void Setting_a_position_greater_than_the_length_of_the_underlying_byte_array_should_throw()
            {
                var expectedException = ExceptionHelper.PositionGreaterThanLengthOfByteArrayException("value");
                var actualException = Assert.Throws<ArgumentOutOfRangeException>(() => { Fixture.BufferWriter.Position = Fixture.BufferWriter.Length + 1; });

                Assert.Equal(expectedException.ParamName, actualException.ParamName);
                Assert.Equal(expectedException.Message, actualException.Message);
            }

            [Fact]
            public void Setting_a_position_equal_to_the_length_of_the_underlying_byte_array_should_succeed()
            {
                Fixture.BufferWriter.Position = Fixture.BufferWriter.Length;
            }

            [Fact]
            public void Should_set_expected_position()
            {
                var positions = new[] { 0, 1, Fixture.BufferWriter.Length };

                foreach (var position in positions)
                {
                    Fixture.BufferWriter.Position = position;

                    Assert.Equal(position, Fixture.BufferWriter.Position);
                }
            }
        }

        public class Position
        {
            public class ByteArray : Position<WriterFixtureByteArray>
            {
                public override WriterFixtureByteArray CreateFixture() => new WriterFixtureByteArray();
            }

            public class ByteArrayWithOffsetAndLength : Position<WriterFixtureByteArray>
            {
                public override WriterFixtureByteArray CreateFixture() => new WriterFixtureByteArray(1, DefaultDataLength - 1);
            }
        }


        public abstract class Writing<TWriterFixture> : SetupWriterFixture<TWriterFixture>
            where TWriterFixture : WriterFixture<TWriterFixture>
        {
            public abstract TWriterFixture CreateFixture();


            internal Writing()
            {
                Init();
            }

            private void Init()
            {
                Fixture = CreateFixture();
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
                Fixture.BufferWriter.Write(input);

                var val = Fixture.NativeReader.ReadInt16();

                Assert.Equal(input, val);
            }

            [Theory]
            [InlineData(true)]
            [InlineData(false)]
            public void WriteBoolean(bool input)
            {
                Fixture.BufferWriter.Write(input);

                var val = Fixture.NativeReader.ReadBoolean();

                Assert.Equal(input, val);
            }

            [Theory]
            [InlineData(0)]
            [InlineData(1)]
            [InlineData(byte.MaxValue)]
            [InlineData(byte.MaxValue / 2)]
            public void WriteByte(byte input)
            {
                Fixture.BufferWriter.Write(input);

                var val = Fixture.NativeReader.ReadByte();

                Assert.Equal(input, val);
            }

            protected void WriteBytesBase(int dataLength, byte data)
            {
                var buff = new byte[dataLength];
                Array.Fill(buff, data);

                Fixture.BufferWriter.Write(buff);

                var val = Fixture.NativeReader.ReadBytes(dataLength);

                Assert.True(val.SequenceEqual(buff));
            }

            protected void WriteBytesSliceBase(int originalDataLength, byte data, int dataOffset, int dataLength)
            {
                var buff = new byte[originalDataLength];
                Array.Fill(buff, data);

                Fixture.BufferWriter.Write(buff, dataOffset, dataLength);

                var val = Fixture.NativeReader.ReadBytes(dataLength);

                Assert.True(val.SequenceEqual(buff[dataOffset..(dataOffset + dataLength)]));
            }

            protected void WriteSpanBase(int dataLength, byte data)
            {
                var buff = new byte[dataLength];
                Array.Fill(buff, data);

                Fixture.BufferWriter.Write(buff.AsSpan());

                var val = Fixture.NativeReader.ReadBytes(dataLength);

                Assert.True(val.SequenceEqual(buff));
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
            public void WriteDecimal(object inputObj)
            {
                var input = Convert.ToDecimal(inputObj);
                Fixture.BufferWriter.Write(input);

                var val = Fixture.NativeReader.ReadDecimal();

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
            public void WriteDouble(double input)
            {
                Fixture.BufferWriter.Write(input);

                var val = Fixture.NativeReader.ReadDouble();

                Assert.Equal(input, val);
            }

            [Theory]
            [InlineData(0)]
            [InlineData(-1)]
            [InlineData(byte.MaxValue)]
            [InlineData(short.MinValue)]
            [InlineData(short.MinValue / 2)]
            [InlineData(short.MaxValue)]
            [InlineData(short.MaxValue / 2)]
            public void WriteInt16(short input)
            {
                Fixture.BufferWriter.Write(input);

                var val = Fixture.NativeReader.ReadInt16();

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
            public void WriteInt32(int input)
            {
                Fixture.BufferWriter.Write(input);

                var val = Fixture.NativeReader.ReadInt32();

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
            public void WriteInt64(long input)
            {
                Fixture.BufferWriter.Write(input);

                var val = Fixture.NativeReader.ReadInt64();

                Assert.Equal(input, val);
            }

            [Theory]
            [InlineData(0)]
            [InlineData(1)]
            [InlineData(-1)]
            [InlineData(sbyte.MinValue)]
            [InlineData(sbyte.MinValue / 2)]
            [InlineData(sbyte.MaxValue)]
            [InlineData(sbyte.MaxValue / 2)]
            public void WriteSByte(sbyte input)
            {
                Fixture.BufferWriter.Write(input);

                var val = Fixture.NativeReader.ReadSByte();

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
            public void WriteSingle(float input)
            {
                Fixture.BufferWriter.Write(input);

                var val = Fixture.NativeReader.ReadSingle();

                Assert.Equal(input, val);
            }

            [Theory]
            [InlineData(0)]
            [InlineData(byte.MaxValue)]
            [InlineData(ushort.MaxValue)]
            [InlineData(ushort.MaxValue / 2)]
            public void WriteUInt16(ushort input)
            {
                Fixture.BufferWriter.Write(input);

                var val = Fixture.NativeReader.ReadUInt16();

                Assert.Equal(input, val);
            }

            [Theory]
            [InlineData(0)]
            [InlineData(byte.MaxValue)]
            [InlineData(ushort.MaxValue)]
            [InlineData(uint.MaxValue)]
            [InlineData(uint.MaxValue / 2)]
            public void WriteUInt32(uint input)
            {
                Fixture.BufferWriter.Write(input);

                var val = Fixture.NativeReader.ReadUInt32();

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
            public void WriteUInt64(ulong input)
            {
                Fixture.BufferWriter.Write(input);

                var val = Fixture.NativeReader.ReadUInt64();

                Assert.Equal(input, val);
            }

            [Fact]
            public void Attempting_to_write_a_null_byte_array_should_throw()
            {
                var actualException = Assert.Throws<ArgumentNullException>(() => { Fixture.BufferWriter.Write(null as byte[]); });

                Assert.Equal("buffer", actualException.ParamName);
            }

            [Fact]
            public void Attempting_to_write_a_slice_of_a_null_byte_array_should_throw()
            {
                var actualException = Assert.Throws<ArgumentNullException>(() => { Fixture.BufferWriter.Write(null as byte[], 0, 0); });

                Assert.Equal("buffer", actualException.ParamName);
            }

            [Theory]
            [InlineData(0, 1)]
            [InlineData(-10, 20)]
            public void Attempting_to_write_outside_the_writable_region_of_the_underlying_byte_array_should_throw(int offsetFromEndOfUnderlyingByteArray, int writeLength)
            {
                Fixture.BufferWriter.Position = Fixture.BufferWriter.Length + offsetFromEndOfUnderlyingByteArray;

                var expectedException = ExceptionHelper.EndOfDataException();
                var actualException = Assert.Throws<EndOfStreamException>(() => Fixture.BufferWriter.Write(new byte[writeLength]));

                Assert.Equal(expectedException.Message, actualException.Message);
            }

            private int WriteData_AdvanceToPosition_WriteData(int startingPosition, int dataLength1, int advanceToPosition, int dataLength2)
            {
                var data1 = Enumerable.Repeat((byte)0xFF, dataLength1).ToArray();
                var data2 = Enumerable.Repeat((byte)(0xFF - 1), dataLength2).ToArray();

                Fixture.BufferWriter.Position = startingPosition;
                Fixture.BufferWriter.Write(data1);
                Fixture.BufferWriter.Position = advanceToPosition;
                Fixture.BufferWriter.Write(data2);

                return Math.Max(Convert.ToByte(dataLength1 > 0) * startingPosition + dataLength1, Convert.ToByte(dataLength2 > 0) * advanceToPosition + dataLength2);
            }

            private int WriteData_AdvanceToPosition_WriteData()
            {
                const int dataLength1 = 10;
                const int dataLength2 = dataLength1;
                const int advanceToPosition = dataLength1 + 10;

                return WriteData_AdvanceToPosition_WriteData(0, dataLength1, advanceToPosition, dataLength2);
            }

            [Fact]
            public void ToReadOnlySpan_should_return_the_written_data()
            {
                var writtenLength = WriteData_AdvanceToPosition_WriteData();

                var expectedVal = Fixture.NativeReader.ReadBytes(writtenLength).AsSpan();

                Assert.True(Fixture.BufferWriter.ToReadOnlySpan().SequenceEqual(expectedVal));
            }

            [Fact]
            public void ToArray_should_return_the_written_data()
            {
                var writtenLength = WriteData_AdvanceToPosition_WriteData();

                var expectedVal = Fixture.NativeReader.ReadBytes(writtenLength);

                Assert.True(Fixture.BufferWriter.ToArray().SequenceEqual(expectedVal));
            }

            [Theory]
            [InlineData(  0,  0, 10,  0)]
            [InlineData(  0, 10, 20, 10)]
            [InlineData(  0, 20, 30,  0)]
            [InlineData(  0, 10,  0, 10)]
            [InlineData(100, 10,  0, 10)]
            public void WrittenLength_should_return_the_length_of_the_written_data(int startingPosition, int dataLength1, int advanceToPosition, int dataLength2)
            {
                var expectedWrittenLength = WriteData_AdvanceToPosition_WriteData(startingPosition, dataLength1, advanceToPosition, dataLength2);

                Assert.Equal(expectedWrittenLength, Fixture.BufferWriter.WrittenLength);
            }
        }

        public class Writing
        {
            public class ByteArray : Writing<WriterFixtureByteArray>
            {
                public override WriterFixtureByteArray CreateFixture() => new WriterFixtureByteArray();

                [Theory]
                [InlineData(0,                 0xFF)]
                [InlineData(1,                 0xFF - 1)]
                [InlineData(DefaultDataLength, 0xFF - 2)]
                public void WriteBytes(int dataLength, byte data) => WriteBytesBase(dataLength, data);

                [Theory]
                [InlineData(                0, 0xFF,                         0,                 0)]
                [InlineData(                1, 0xFF - 1,                     0,                 0)]
                [InlineData(                1, 0xFF - 1,                     0,                 1)]
                [InlineData(DefaultDataLength, 0xFF - 2,                     0, DefaultDataLength)]
                [InlineData(DefaultDataLength, 0xFF - 2,     DefaultDataLength,                 0)]
                [InlineData(DefaultDataLength, 0xFF - 2, DefaultDataLength - 1,                 1)]
                public void WriteBytesSlice(int originalDataLength, byte data, int dataOffset, int dataLength) => WriteBytesSliceBase(originalDataLength, data, dataOffset, dataLength);

                [Theory]
                [InlineData(0,                 0xFF)]
                [InlineData(1,                 0xFF - 1)]
                [InlineData(DefaultDataLength, 0xFF - 2)]
                public void WriteSpan(int dataLength, byte data) => WriteSpanBase(dataLength, data);
            }

            public class ByteArrayWithOffsetAndLength : Writing<WriterFixtureByteArray>
            {
                public override WriterFixtureByteArray CreateFixture() => new WriterFixtureByteArray(1, DefaultDataLength - 1);

                [Theory]
                [InlineData(0,                     0xFF)]
                [InlineData(1,                     0xFF - 1)]
                [InlineData(DefaultDataLength - 1, 0xFF - 2)]
                public void WriteBytes(int dataLength, byte data) => WriteBytesBase(dataLength, data);

                [Theory]
                [InlineData(                0, 0xFF,                         0,                     0)]
                [InlineData(DefaultDataLength, 0xFF - 2,                     0, DefaultDataLength - 1)]
                [InlineData(DefaultDataLength, 0xFF - 2, DefaultDataLength,                         0)]
                [InlineData(DefaultDataLength, 0xFF - 2, DefaultDataLength - 1,                     1)]
                public void WriteBytesSlice(int originalDataLength, byte data, int dataOffset, int dataLength) => WriteBytesSliceBase(originalDataLength, data, dataOffset, dataLength);

                [Theory]
                [InlineData(0,                     0xFF)]
                [InlineData(1,                     0xFF - 1)]
                [InlineData(DefaultDataLength - 1, 0xFF - 2)]
                public void WriteSpan(int dataLength, byte data) => WriteSpanBase(dataLength, data);
            }
        }
    }
}
