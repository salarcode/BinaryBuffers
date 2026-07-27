using System;
using System.IO;
using Salar.BinaryBuffers.Compatibility;
using Xunit;

namespace Salar.BinaryBuffers.Tests;

public class StreamBufferReaderTests
{
	[Fact]
	public void ReadSpan_should_respect_MemoryStream_origin()
	{
		var data = new byte[] { 9, 9, 1, 2, 3, 9 };
		using var stream = new MemoryStream(data, 2, 3, writable: false, publiclyVisible: true);
		using var reader = new StreamBufferReader(stream);

		var actual = reader.ReadSpan(3).ToArray();

		Assert.Equal(new byte[] { 1, 2, 3 }, actual);
	}

	[Fact]
	public void ReadSpan_should_support_non_public_MemoryStream_buffers()
	{
		var data = new byte[32];
		Array.Fill(data, (byte)42);
		using var stream = new MemoryStream(data, writable: false);
		using var reader = new StreamBufferReader(stream);

		var actual = reader.ReadSpan(data.Length).ToArray();

		Assert.Equal(data, actual);
	}

	[Fact]
	public void ReadSpan_should_complete_partial_stream_reads()
	{
		var data = new byte[32];
		Array.Fill(data, (byte)42);
		using var stream = new PartialReadStream(data);
		using var reader = new StreamBufferReader(stream);

		var actual = reader.ReadSpan(data.Length).ToArray();

		Assert.Equal(data, actual);
	}

	[Fact]
	public void ReadSpan_should_throw_when_MemoryStream_position_is_past_the_end()
	{
		using var stream = new MemoryStream(new byte[4], 0, 4, writable: false, publiclyVisible: true);
		stream.Position = 5;
		using var reader = new StreamBufferReader(stream);

		Assert.Throws<EndOfStreamException>(() => reader.ReadSpan(1));
	}

	private sealed class PartialReadStream : MemoryStream
	{
		public PartialReadStream(byte[] buffer)
			: base(buffer, writable: false)
		{
		}

		public override int Read(byte[] buffer, int offset, int count) => base.Read(buffer, offset, Math.Min(count, 3));
	}
}
