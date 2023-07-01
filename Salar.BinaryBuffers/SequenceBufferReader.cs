using System;
using System.Buffers;
using System.Runtime.CompilerServices;

namespace Salar.BinaryBuffers;

public sealed class SequenceBufferReader : BufferReaderBase, IDisposable
{
	private int _length;
	private int _position;
	private ReadOnlySequence<byte> _buffer = ReadOnlySequence<byte>.Empty;

	/// <inheritdoc/>
	public override int Offset => 0;

	/// <inheritdoc/>
	public override int Length => _length;

	/// <inheritdoc/>
	public override int Position
	{
		get => _position;
		set
		{
			if (value < 0) throw ExceptionHelper.PositionLessThanZeroException(nameof(value));
			if (value > _length) throw ExceptionHelper.PositionGreaterThanLengthOfByteArrayException(nameof(value));

			_position = value;
		}
	}

	/// <inheritdoc/>
	public override int Remaining => _length - _position;

	/// <summary>
	/// Initializes a new instance of the <see cref="SequenceBufferReader"/> class based on the specified <see cref="ReadOnlySequence{T}"/>.
	/// </summary>
	/// <param name="buffer"></param>
	public SequenceBufferReader(in ReadOnlySequence<byte> buffer)
	{
		_buffer = buffer;
		_position = 0;
		_length = (int)buffer.Length;
	}

	/// <inheritdoc/>
	public override int Read(byte[] buffer, int index, int count)
	{
		if (count <= 0)
			return 0;

		int curPos = _position;
		int newPos = curPos + count;

		if ((uint)newPos > (uint)_length)
		{
			_position = _length;
			throw ExceptionHelper.EndOfDataException();
		}

		_position = newPos;

		if (_buffer.IsSingleSegment)
		{
#if NET6_0_OR_GREATER
			_buffer.FirstSpan
				.Slice(curPos, count)
				.CopyTo(buffer.AsSpan(index, count));
#else
			_buffer.First
				.Slice(curPos, count)
				.CopyTo(buffer.AsMemory(index, count));
#endif
		}
		else
		{
			var result = _buffer.Slice(curPos, count);
			result.CopyTo(buffer.AsSpan(index, count));
		}
		return count;
	}

	/// <inheritdoc/>
	public override ReadOnlySpan<byte> ReadSpan(int count) => InternalReadSpan(count);

	protected override byte InternalReadByte()
	{
		var sequence = InternalReadSequence(1);

#if NET6_0_OR_GREATER
		return sequence.FirstSpan[0];
#else
        return sequence.First.Span[0];
#endif
	}


	/// <inheritdoc/>
	public override byte[] ReadBytes(int count)
	{
		if (count <= 0)
			return Array.Empty<byte>();

		var sequence = InternalReadSequence(count);

		return sequence.ToArray();
	}

	/// <inheritdoc/>
	public override ReadOnlyMemory<byte> ReadMemory(int count)
	{
		if (count <= 0)
			return ReadOnlyMemory<byte>.Empty;

		var sequence = InternalReadSequence(count);
		if (sequence.IsSingleSegment)
		{
			return sequence.First;
		}
		return sequence.ToArray();
	}

	protected override ReadOnlySpan<byte> InternalReadSpan(int count)
	{
		if (count <= 0)
			return ReadOnlySpan<byte>.Empty;

		int curPos = _position;
		int newPos = curPos + count;

		if ((uint)newPos > (uint)_length)
		{
			_position = _length;
			throw ExceptionHelper.EndOfDataException();
		}

		_position = newPos;

		if (_buffer.IsSingleSegment)
		{
#if NET6_0_OR_GREATER
			return _buffer.FirstSpan.Slice(curPos, count);
#else
			return _buffer.First.Span.Slice(curPos, count);
#endif
		}

		var sequence = _buffer.Slice(curPos, count);
		if (sequence.IsSingleSegment)
		{
#if NET6_0_OR_GREATER
			return sequence.FirstSpan;
#else
			return sequence.First.Span;
#endif
		}
		return sequence.ToArray();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private ReadOnlySequence<byte> InternalReadSequence(int count)
	{
		int curPos = _position;
		int newPos = curPos + count;

		if ((uint)newPos > (uint)_length)
		{
			_position = _length;
			throw ExceptionHelper.EndOfDataException();
		}

 		_position = newPos;

		return _buffer.Slice(curPos, count);
	}

	public void Dispose()
	{
		_buffer = ReadOnlySequence<byte>.Empty;
	}
}
