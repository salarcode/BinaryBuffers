using System;

namespace Salar.BinaryBuffers.Tests.Fixtures;

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