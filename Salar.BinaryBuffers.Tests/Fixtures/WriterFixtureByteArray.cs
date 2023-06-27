using System;

namespace Salar.BinaryBuffers.Tests.Fixtures;

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
