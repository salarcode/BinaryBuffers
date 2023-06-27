using System;

namespace Salar.BinaryBuffers.Tests.Fixtures;

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
