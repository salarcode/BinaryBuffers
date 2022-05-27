namespace BinaryBuffers.Tests.Fixtures
{
    using System;
    using System.IO;

    public abstract class SetupReaderFixture<TReaderFixture> : IDisposable
        where TReaderFixture : ReaderFixture<TReaderFixture>
    {
        internal const int DefaultDataLength = ReaderFixture<TReaderFixture>.DefaultDataLength;

        protected internal TReaderFixture Fixture { get; set; }


        public void Dispose()
        {
            Fixture?.Dispose();
        }
    }

    public abstract class ReaderFixture<TReaderFixture> : IDisposable
        where TReaderFixture : ReaderFixture<TReaderFixture>
    {
        internal const int DefaultDataLength = 1_024;

        protected internal static byte[] CreateDefaultData => new byte[DefaultDataLength];

        internal byte[] Data { get; }
        internal BinaryWriter NativeWriter { get; }
        internal BinaryBufferReader BufferReader { get; }
        
        
        protected static (byte[] Data, BinaryBufferReader BufferReader, MemoryStream dataStream) GetConstructorArgs(Func<byte[]> getData = null, (int Offset, int Length)? offsetAndLengthArgs = null)
        {
            var data = getData != null ? getData() : CreateDefaultData;

            if (offsetAndLengthArgs.HasValue)
            {
                return (data, new BinaryBufferReader(data, offsetAndLengthArgs.Value.Offset, offsetAndLengthArgs.Value.Length), new MemoryStream(data, offsetAndLengthArgs.Value.Offset, offsetAndLengthArgs.Value.Length));
            }

            return (data, new BinaryBufferReader(data), new MemoryStream(data));
        }

        protected internal ReaderFixture((byte[] Data, BinaryBufferReader BufferReader, MemoryStream dataStream) args)
        {
            Data = args.Data;
            BufferReader = args.BufferReader;
            NativeWriter = new BinaryWriter(args.dataStream);
        }

        public void Dispose()
        {
            NativeWriter?.Dispose();
        }
    }

    public sealed class ReaderFixtureByteArray : ReaderFixture<ReaderFixtureByteArray>
    {
        internal ReaderFixtureByteArray() : base(GetConstructorArgs())
        {
        }

        internal ReaderFixtureByteArray(int offset, int length) : base(GetConstructorArgs(offsetAndLengthArgs: (offset, length)))
        {
        }

        internal ReaderFixtureByteArray(Func<byte[]> getData) : base(GetConstructorArgs(getData))
        {
        }

        internal ReaderFixtureByteArray(Func<byte[]> getData, int offset, int length) : base(GetConstructorArgs(getData, (offset, length)))
        {
        }
    }

    public sealed class ReaderFixtureArraySegment : ReaderFixture<ReaderFixtureArraySegment>
    {
        private static (byte[] Data, BinaryBufferReader BufferReader, MemoryStream dataStream) GetConstructorArgs(ArraySegment<byte>? data = null, (int Offset, int Length)? offsetAndLengthArgs = null)
        {
            byte[] dataArray;
            BinaryBufferReader bufferReader;
            MemoryStream dataStream;

            if (!data.HasValue)
            {
                dataArray = CreateDefaultData;

                if (offsetAndLengthArgs.HasValue)
                {
                    bufferReader = new BinaryBufferReader(new ArraySegment<byte>(dataArray, offsetAndLengthArgs.Value.Offset, offsetAndLengthArgs.Value.Length));
                    dataStream = new MemoryStream(dataArray, offsetAndLengthArgs.Value.Offset, offsetAndLengthArgs.Value.Length);
                }
                else
                {
                    bufferReader = new BinaryBufferReader(new ArraySegment<byte>(dataArray));
                    dataStream = new MemoryStream(dataArray);
                }
            }
            else
            {
                if (offsetAndLengthArgs.HasValue) throw new InvalidOperationException("Redundant offset and length arguments provided; array segment is already initialized.");

                dataArray = data.Value.Array;
                bufferReader = new BinaryBufferReader(data.Value);
                dataStream = new MemoryStream(dataArray);
            }

            return (dataArray, bufferReader, dataStream);
        }

        internal ReaderFixtureArraySegment() : base(GetConstructorArgs())
        {
        }

        internal ReaderFixtureArraySegment(int offset, int length) : base(GetConstructorArgs(offsetAndLengthArgs: (offset, length)))
        {
        }

        internal ReaderFixtureArraySegment(in ArraySegment<byte> data) : base((data.Array, new BinaryBufferReader(data), new MemoryStream(data.Array)))
        {
        }
    }
}
