﻿using System;
using System.Buffers;
using System.Runtime.CompilerServices;

namespace Salar.BinaryBuffers;

public class SequenceBufferReader : BufferReaderBase, IDisposable
{
	private int _length;
	private int _offset;
	private int _relativePositon;
	private int _position;
	private ReadOnlySequence<byte>? _buffer;

	/// <inheritdoc/>
	public override int Offset => _offset;

	/// <inheritdoc/>
	public override int Length => _length;

	/// <inheritdoc/>
	public override int Position
	{
		get => _relativePositon;
		set
		{
			var newPosition = _offset + value;

			if (value < 0) throw ExceptionHelper.PositionLessThanZeroException(nameof(value));
			if (value > _length) throw ExceptionHelper.PositionGreaterThanLengthOfByteArrayException(nameof(value));

			_relativePositon = value;
			_position = newPosition;
		}
	}

	public override int Remaining => _length - _relativePositon;

	public SequenceBufferReader(ReadOnlySequence<byte> buffer)
	{
		_buffer = buffer;
		_position = 0;
		_relativePositon = 0;
		_offset = 0;
		_length = (int)buffer.Length;
	}

	/// <inheritdoc/>
	public override int Read(byte[] buffer, int index, int count)
	{
		if (count <= 0)
			return 0;
		if (_buffer == null)
			throw ExceptionHelper.DisposedException(nameof(SequenceBufferReader));

		int relPos = _relativePositon + count;

		if ((uint)relPos > (uint)_length)
		{
			count = relPos - _length;
		}
		if (count <= 0)
			return 0;

		if (_buffer == null)
			throw ExceptionHelper.DisposedException(nameof(SequenceBufferReader));

		int curPos = _position;
		int newPos = curPos + count;
		relPos = _relativePositon + count;

		if ((uint)relPos > (uint)_length)
		{
			_relativePositon = _length;
			throw ExceptionHelper.EndOfDataException();
		}

		_relativePositon = relPos;
		_position = newPos;

		var result = _buffer.Value.Slice(curPos, count);
		result.CopyTo(buffer.AsSpan(index, count));

		return count;
	}

	/// <inheritdoc/>
	public override ReadOnlySpan<byte> ReadSpan(int count) => InternalReadSpan(count);

	protected override byte InternalReadByte()
	{
		if (_buffer == null)
			throw ExceptionHelper.DisposedException(nameof(SequenceBufferReader));

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
		if (_buffer == null)
			throw ExceptionHelper.DisposedException(nameof(SequenceBufferReader));

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

		var sequence = InternalReadSequence(count);
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
		if (_buffer == null)
			throw ExceptionHelper.DisposedException(nameof(SequenceBufferReader));

		int curPos = _position;
		int newPos = curPos + count;
		int relPos = _relativePositon + count;

		if ((uint)relPos > (uint)_length)
		{
			_relativePositon = _length;
			throw ExceptionHelper.EndOfDataException();
		}

		_relativePositon = relPos;
		_position = newPos;

		return _buffer.Value.Slice(curPos, count);
	}

	public void Dispose()
	{
		_buffer = null;
	}
}
