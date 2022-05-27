namespace BinaryBuffers.Tests.Fixtures
{
    using System;
    using System.IO;

    public abstract class SetupWriterFixture<TWriterFixture> : IDisposable
        where TWriterFixture : WriterFixture<TWriterFixture>
    {
        internal const int DefaultDataLength = WriterFixture<TWriterFixture>.DefaultDataLength;

        protected internal TWriterFixture Fixture { get; set; }


        public void Dispose()
        {
            Fixture?.Dispose();
        }
    }

    public abstract class WriterFixture<TWriterFixture> : IDisposable
        where TWriterFixture : WriterFixture<TWriterFixture>
    {
        internal const int DefaultDataLength = 1_024;

        protected internal static byte[] CreateDefaultData => new byte[DefaultDataLength];


        internal byte[] Data { get; }
        internal BinaryReader NativeReader { get; }
        internal BinaryBufferWriter BufferWriter { get; }
        
        
        protected static (byte[] Data, BinaryBufferWriter BufferWriter, MemoryStream dataStream) GetConstructorArgs(Func<byte[]> getData = null, (int Offset, int Length)? offsetAndLengthArgs = null)
        {
            var data = getData != null ? getData() : CreateDefaultData;

            if (offsetAndLengthArgs.HasValue)
            {
                return (data, new BinaryBufferWriter(data, offsetAndLengthArgs.Value.Offset, offsetAndLengthArgs.Value.Length), new MemoryStream(data, offsetAndLengthArgs.Value.Offset, offsetAndLengthArgs.Value.Length));
            }

            return (data, new BinaryBufferWriter(data), new MemoryStream(data));
        }

        protected internal WriterFixture((byte[] Data, BinaryBufferWriter BufferWriter, MemoryStream dataStream) args)
        {
            Data = args.Data;
            BufferWriter = args.BufferWriter;
            NativeReader = new BinaryReader(args.dataStream);
        }

        public void Dispose()
        {
            NativeReader?.Dispose();
        }
    }

    public sealed class WriterFixtureByteArray : WriterFixture<WriterFixtureByteArray>
    {
        internal WriterFixtureByteArray() : base(GetConstructorArgs())
        {
        }

        internal WriterFixtureByteArray(int offset, int length) : base(GetConstructorArgs(offsetAndLengthArgs: (offset, length)))
        {
        }

        internal WriterFixtureByteArray(Func<byte[]> getData) : base(GetConstructorArgs(getData))
        {
        }

        internal WriterFixtureByteArray(Func<byte[]> getData, int offset, int length) : base(GetConstructorArgs(getData, (offset, length)))
        {
        }
    }
}
