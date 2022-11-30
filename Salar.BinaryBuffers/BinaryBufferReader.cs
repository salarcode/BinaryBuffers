using System;

namespace Salar.BinaryBuffers;

/// <summary>
/// Implements an <see cref="BufferReaderBase"/> that can read primitive data types from a byte array.
/// </summary>
public class BinaryBufferReader : BufferReaderBase
{
	private byte[] _data;
	private int _length;
	private int _offset;
	private int _relativePositon;
	private int _position;

	/// <summary>
	/// Gets the offset into the underlying byte array to start reading from.
	/// </summary>
	public override int Offset => _offset;

	/// <summary>
	/// Gets the effective length of the readable region of the underlying byte array.
	/// </summary>
	public override int Length => _length;

	/// <summary>
	/// Gets or sets the current reading position within the underlying byte array.
	/// </summary>
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

	/// <summary>
	/// Initializes a new instance of the <see cref="BinaryBufferReader"/> class based on the specified byte array.
	/// </summary>
	/// <param name="data">The byte array to read from.</param>
	public BinaryBufferReader(byte[] data)
	{
		ResetBuffer(data);
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="BinaryBufferReader"/> class based on the specified byte array.
	/// <para>A provided offset and length specifies the boundaries to use for reading.</para>
	/// </summary>
	/// <param name="data">The byte array to read from.</param>
	/// <param name="offset">The 0-based offset into the byte array at which to begin reading from.
	/// <para>Cannot exceed the bounds of the byte array.</para></param>
	/// <param name="length">The maximum number of bytes that the reader will use for reading, relative to the offset position.
	/// <para>Cannot exceed the bounds of the byte array.</para></param>
	public BinaryBufferReader(byte[] data, int offset, int length)
	{
		ResetBuffer(data, offset, length);
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="BinaryBufferReader"/> class based on the specified byte array segment.
	/// </summary>
	/// <param name="data">The byte array segment to read from.</param>
	public BinaryBufferReader(in ArraySegment<byte> data)
	{
		ResetBuffer(in data);
	}


	/// <summary>
	/// Resets the undelying bufer based on the specified byte array.
	/// </summary>
	/// <param name="data">The byte array to read from.</param>
	public void ResetBuffer(byte[] data)
	{
		_data = data ?? throw new ArgumentNullException(nameof(data));
		_position = 0;
		_relativePositon = 0;
		_offset = 0;
		_length = data.Length;
	}

	/// <summary>
	/// Resets the undelying bufer based on the specified byte array.
	/// <para>A provided offset and length specifies the boundaries to use for reading.</para>
	/// </summary>
	/// <param name="data">The byte array to read from.</param>
	/// <param name="offset">The 0-based offset into the byte array at which to begin reading from.
	/// <para>Cannot exceed the bounds of the byte array.</para></param>
	/// <param name="length">The maximum number of bytes that the reader will use for reading, relative to the offset position.
	/// <para>Cannot exceed the bounds of the byte array.</para></param>
	public void ResetBuffer(byte[] data, int offset, int length)
	{
		_data = data ?? throw new ArgumentNullException(nameof(data));

		if (offset < 0) throw ExceptionHelper.OffsetLessThanZeroException(nameof(offset));
		if (length < 0) throw ExceptionHelper.LengthLessThanZeroException(nameof(length));
		if (length > _data.Length - offset) throw ExceptionHelper.LengthGreaterThanEffectiveLengthOfByteArrayException();

		_position = offset;
		_relativePositon = 0;
		_offset = offset;
		_length = length;
	}

	/// <summary>
	/// Resets the buffer of the <see cref="BinaryBufferReader"/> class based on the specified byte array segment.
	/// </summary>
	/// <param name="data">The byte array segment to read from.</param>
	public void ResetBuffer(in ArraySegment<byte> data)
	{
		_data = data.Array ?? throw new ArgumentNullException(nameof(data));
		_position = data.Offset;
		_relativePositon = 0;
		_offset = data.Offset;
		_length = data.Count;
	}

	/// <inheritdoc/>
	public override byte[] ReadBytes(int count) => InternalReadSpan(count).ToArray();

	/// <inheritdoc/>
	public override ReadOnlySpan<byte> ReadSpan(int count) => InternalReadSpan(count);
	
	/// <inheritdoc/>
	public override int Read(byte[] buffer, int index, int count)
	{
		if (count <= 0)
			return 0;

		int relPos = _relativePositon + count;

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
	/// Reads the next byte from the underlying byte array and advances the current position by one byte.
	/// </summary>
	protected override byte InternalReadByte()
	{
		int curPos = _position;
		int newPos = curPos + 1;
		int relPos = _relativePositon + 1;

		if ((uint)relPos > (uint)_length)
		{
			_relativePositon = _length;
			throw ExceptionHelper.EndOfDataException();
		}

		_relativePositon = relPos;
		_position = newPos;

		return _data[curPos];
	}

	/// <summary>
	/// Returns a read-only span over the specified number of bytes from the underlying byte array and advances the current position by that number of bytes.
	/// </summary>
	/// <param name="count">The size of the read-only span to return.</param>
	protected override ReadOnlySpan<byte> InternalReadSpan(int count)
	{
		if (count <= 0)
			return ReadOnlySpan<byte>.Empty;

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

		return new ReadOnlySpan<byte>(_data, curPos, count);
	}
}
