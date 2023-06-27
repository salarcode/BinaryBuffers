using System;

namespace Salar.BinaryBuffers.Tests.Fixtures;

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
