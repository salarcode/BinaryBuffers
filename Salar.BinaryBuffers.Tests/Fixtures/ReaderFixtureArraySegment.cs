using System;
using System.IO;

namespace Salar.BinaryBuffers.Tests.Fixtures;

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
