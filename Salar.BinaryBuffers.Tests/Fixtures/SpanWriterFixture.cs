using System;
using System.IO;

namespace Salar.BinaryBuffers.Tests.Fixtures;

public sealed class SpanWriterFixture : IDisposable
{
	internal const int DefaultDataLength = 1_024;

	internal byte[] Data { get; }
	internal BinaryReader NativeReader { get; }

	public SpanWriterFixture()
	{
		Data = new byte[DefaultDataLength];
		NativeReader = new BinaryReader(new MemoryStream(Data));
	}

	public SpanWriterFixture(int length)
	{
		if (length < 0) throw new ArgumentOutOfRangeException(nameof(length));
		Data = new byte[length];
		NativeReader = new BinaryReader(new MemoryStream(Data));
	}

	public BinarySpanBufferWriter CreateWriter() => new BinarySpanBufferWriter(Data.AsSpan());

	public void Dispose()
	{
		NativeReader?.Dispose();
	}
}
