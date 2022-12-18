using System;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Salar.BinaryBuffers.Compatibility;

/// <summary>
/// Implements a <see cref="BufferReaderBase"/> that can read primitive data types from a <see cref="Stream"/>.
/// Normally you should not use this instead of <see cref="BinaryReader"/> but use this for widen support of <see cref="IBufferReader"/>.
/// This has similar performance to the <see cref="BinaryReader"/>.
/// </summary>
public class StreamBufferReader : BufferReaderBase, IDisposable
{
	private delegate ReadOnlySpan<byte> MemoryStreamInternalReadSpan(int count);

	private static readonly MethodInfo _memoryStreamInternalReadSpanMethodInfo = typeof(MemoryStream)
		.GetMethod("InternalReadSpan", BindingFlags.Instance | BindingFlags.NonPublic);

	private byte[] _buffer;
	private Stream _stream;
	private MemoryStreamInternalReadSpan _memoryStreamInternalReadSpan;

	/// <inheritdoc/>
	public override int Offset => 0;

	/// <inheritdoc/>
	public override int Length => (int)_stream.Length;

	/// <inheritdoc/>
	public override int Position { get => (int)_stream.Position; set => _stream.Position = value; }

	/// <inheritdoc/>
	public override int Remaining => (int)(_stream.Length - _stream.Position);

	public StreamBufferReader(Stream stream)
	{
		_buffer = new byte[16];
		_stream = stream;
		if (stream is MemoryStream memoryStream)
		{
			_memoryStreamInternalReadSpan = (MemoryStreamInternalReadSpan)_memoryStreamInternalReadSpanMethodInfo.CreateDelegate(typeof(MemoryStreamInternalReadSpan), memoryStream);
		}
	}

	/// <inheritdoc/>
	public override int Read(byte[] buffer, int index, int count)
	{
		return _stream.Read(buffer, index, count);
	}

	/// <inheritdoc/>
	public override byte[] ReadBytes(int count)
	{
		return InternalReadNewBytes(count);
	}

	/// <inheritdoc/>
	public override ReadOnlySpan<byte> ReadSpan(int count)
	{
		return InternalReadNewBytes(count);
	}

	protected override byte InternalReadByte()
	{
		if (_stream.Read(_buffer, 0, 1) == 0)
		{
			throw new EndOfStreamException("Reached to end of data");
		}
		return _buffer[0];
	}

	protected override ReadOnlySpan<byte> InternalReadSpan(int count)
	{
		if (_memoryStreamInternalReadSpan != null)
		{
			return _memoryStreamInternalReadSpan(count);
		}
		int offset = 0;
		do
		{
			int num = _stream.Read(_buffer, offset, count - offset);
			if (num == 0)
				throw new EndOfStreamException("Reached to end of data");
			offset += num;
		}
		while (offset < count);
		return (ReadOnlySpan<byte>)_buffer;
	}

#if NET6_0_OR_GREATER
	[SkipLocalsInit]
#endif
	private byte[] InternalReadNewBytes(int count)
	{
		var buffer = new byte[count];
		int offset = 0;
		do
		{
			int num = _stream.Read(buffer, offset, count - offset);
			if (num == 0)
				throw new EndOfStreamException("Reached to end of data");
			offset += num;
		}
		while (offset < count);
		return buffer;
	}

	/// <inheritdoc/>
	public void Dispose()
	{
		// not making this null can cause memory leaks
		_memoryStreamInternalReadSpan = null;
		_stream = null;
		_buffer = null;
	}
}
