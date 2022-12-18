using System;

namespace Salar.BinaryBuffers;

/// <summary>
/// Implements an <see cref="BufferReaderBase"/> that can read primitive data types from a <see cref="byte"/>-based <see cref="ReadOnlyMemory{T}"/>.
/// </summary>
public class BinaryBufferMemoryReader : BufferReaderBase
{
	private readonly ReadOnlyMemory<byte> _data;
	private readonly int _offset;
	private readonly int _length;
	private int _position;

	/// <summary>
	/// Gets the offset into the underlying <see cref="ReadOnlyMemory{T}"/> to start reading from.
	/// </summary>
	public override int Offset => _offset;

	/// <summary>
	/// Gets the effective length of the readable region of the underlying <see cref="ReadOnlyMemory{T}"/>.
	/// </summary>
	public override int Length => _length;

	/// <summary>
	/// Gets or sets the current reading position within the underlying <see cref="ReadOnlyMemory{T}"/>.
	/// </summary>
	public override int Position
	{
		get => _position;
		set
		{
			var newPosition = _position + value;

			if (newPosition < 0) throw ExceptionHelper.PositionLessThanZeroException(nameof(value));
			if (newPosition > _length) throw ExceptionHelper.PositionGreaterThanLengthOfReadOnlyMemoryException(nameof(value));

			_position = newPosition;
		}
	}

	/// <inheritdoc/>
	public override int Remaining => _length - _position;

	/// <summary>
	/// Initializes a new instance of <see cref="BinaryBufferReader"/> based on the specified <see cref="ReadOnlyMemory{T}"/>.
	/// </summary>
	/// <param name="data">The input <see cref="ReadOnlyMemory{T}"/>.</param>
	/// 
	public BinaryBufferMemoryReader(in ReadOnlyMemory<byte> data)
	{
		_data = data;
		_position = 0;
		_offset = 0;
		_length = data.Length;
	}

	/// <inheritdoc/>
	public override byte[] ReadBytes(int count) => InternalReadSpan(count).ToArray();

	public override ReadOnlySpan<byte> ReadSpan(int count) => InternalReadSpan(count);

	/// <inheritdoc/>
	public override int Read(byte[] buffer, int index, int count)
	{
		if (count <= 0)
			return 0;

		int relPos = _position + count;

		if ((uint)relPos > (uint)_length)
		{
			count = relPos - _length;
		}
		if (count <= 0)
			return 0;

		var span = InternalReadSpan(count);
		span.CopyTo(buffer.AsSpan(index, count));

		return count;
	}


	/// <summary>
	/// Reads the next byte from the underlying <see cref="ReadOnlyMemory{T}"/> and advances the current position by one byte.
	/// </summary>
	protected override byte InternalReadByte()
	{
		int curPos = _position;
		int newPos = curPos + 1;

		if ((uint)newPos > (uint)_length)
		{
			_position = _length;
			throw ExceptionHelper.EndOfDataException();
		}

		_position = newPos;

		return _data.Span[curPos];
	}

	/// <summary>
	/// Returns a read-only span over the specified number of bytes from the underlying <see cref="ReadOnlyMemory{T}"/> and advances the current position by that number of bytes.
	/// </summary>
	/// <param name="count">The size of the read-only span to return.</param>
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

		return _data.Slice(curPos, count).Span;
	}
}
