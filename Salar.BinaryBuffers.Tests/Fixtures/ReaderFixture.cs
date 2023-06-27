using System;
using System.IO;

namespace Salar.BinaryBuffers.Tests.Fixtures;

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
