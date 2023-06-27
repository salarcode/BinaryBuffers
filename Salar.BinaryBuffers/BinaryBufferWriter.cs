using System;
using System.Runtime.CompilerServices;
#if NET6_0_OR_GREATER
using System.Runtime.InteropServices;
#endif

namespace Salar.BinaryBuffers;

/// <summary>
/// Provides a writer for writing primitive data types to a byte array.
/// </summary>
public class BinaryBufferWriter : BufferWriterBase
{
	private byte[] _buffer;
	private int _position;
	private int _relativePositon;
	private int _length;
	private int _offset;
	private int _writtenLength;

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

	/// <summary>
	/// Gets the total number of bytes written to the underlying byte array.
	/// </summary>
	public int WrittenLength => _writtenLength;

	/// <summary>
	/// Initializes a new instance of the <see cref="BinaryBufferWriter"/> class using the specified byte array to write the output to.
	/// </summary>
	/// <param name="buffer">The byte array to write to.</param>
	public BinaryBufferWriter(byte[] buffer)
	{
		ResetBuffer(buffer);
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="BinaryBufferWriter"/> class using the specified byte array to write the output to.
	/// <para>A provided offset and length specifies the boundaries to use for writing.</para>
	/// </summary>
	/// <param name="buffer">The output buffer to write to.</param>
	/// <param name="offset">The 0-based offset into the byte array at which to begin writing from.
	/// <para>Cannot exceed the bounds of the byte array.</para></param>
	/// <param name="length">The maximum number of bytes that the writer will use for writing, relative to the offset position.
	/// <para>Cannot exceed the bounds of the byte array.</para></param>
	public BinaryBufferWriter(byte[] buffer, int offset, int length)
	{
		ResetBuffer(buffer, offset, length);
	}

	/// <summary>
	/// Reset the underlying buffer using the specified byte array to write the output to.
	/// </summary>
	/// <param name="buffer">The byte array to write to.</param>
	public void ResetBuffer(byte[] buffer)
	{
		_buffer = buffer ?? throw new ArgumentNullException(nameof(buffer));
		_position = 0;
		_relativePositon = 0;
		_offset = 0;
		_length = buffer.Length;
		_writtenLength = 0;
	}

	/// <summary>
	/// Reset the underlying buffer using the specified byte array to write the output to.
	/// <para>A provided offset and length specifies the boundaries to use for writing.</para>
	/// </summary>
	/// <param name="buffer">The output buffer to write to.</param>
	/// <param name="offset">The 0-based offset into the byte array at which to begin writing from.
	/// <para>Cannot exceed the bounds of the byte array.</para></param>
	/// <param name="length">The maximum number of bytes that the writer will use for writing, relative to the offset position.
	/// <para>Cannot exceed the bounds of the byte array.</para></param>
	public void ResetBuffer(byte[] buffer, int offset, int length)
	{
		_buffer = buffer ?? throw new ArgumentNullException(nameof(buffer));

		if (offset < 0) throw ExceptionHelper.OffsetLessThanZeroException(nameof(offset));
		if (length < 0) throw ExceptionHelper.LengthLessThanZeroException(nameof(length));
		if (length > _buffer.Length - offset) throw ExceptionHelper.LengthGreaterThanEffectiveLengthOfByteArrayException();

		_position = offset;
		_relativePositon = 0;
		_offset = offset;
		_length = length;
		_writtenLength = 0;
	}

	/// <summary>
	/// Reset the status of what is written; doesn't clean the underlying buffer.
	/// </summary>
	public override void ResetBuffer()
	{
		_position = 0;
		_relativePositon = 0;
		_offset = 0;
		_writtenLength = 0;
	}

	/// <summary>
	/// Writes a boolean value to the underlying byte array and advances the current position by one byte.
	/// </summary>
	/// <param name="value">The boolean value to write.</param>
	public override void Write(bool value)
	{
		var pos = _position;
		Advance(1);

		_buffer[pos] = (byte)(value ? 1 : 0);
	}

	/// <summary>
	/// Writes a byte to the underlying byte array and advances the current position by one byte.
	/// </summary>
	/// <param name="value">The byte value to write.</param>
	public override void Write(byte value)
	{
		var pos = _position;
		Advance(1);

		_buffer[pos] = value;
	}

	/// <summary>
	/// Writes a signed byte to the underlying byte array and advances the current position by one byte.
	/// </summary>
	/// <param name="value">The signed byte value to write.</param>
	public override void Write(sbyte value)
	{
		var pos = _position;
		Advance(1);

		_buffer[pos] = (byte)value;
	}

	/// <summary>
	/// Copies the contents of a byte array to the underlying byte array of the writer and advances the current position by the number of bytes written.
	/// </summary>
	/// <param name="buffer">The buffer to copy data from.</param>
	public override void Write(byte[] buffer)
	{
		if (buffer == null) throw new ArgumentNullException(nameof(buffer));

		var pos = _position;
		var length = buffer.Length;
		Advance(length);

		Array.Copy(buffer, 0, _buffer, pos, length);
	}

	/// <summary>
	/// Copies a region of a byte array to the underlying byte array of the writer and advances the current position by the number of bytes written.
	/// </summary>
	/// <param name="buffer">The buffer to copy data from.</param>
	/// <param name="offset">The 0-based offset in buffer at which to start copying from.</param>
	/// <param name="length">The number of bytes to copy.</param>
	public override void Write(byte[] buffer, int offset, int length)
	{
		if (buffer == null) throw new ArgumentNullException(nameof(buffer));

		var pos = _position;
		Advance(length);

		Array.Copy(buffer, offset, _buffer, pos, length);
	}

	/// <summary>
	/// Writes a decimal value to the underlying byte array and advances the current position by sixteen bytes.
	/// </summary>
	/// <param name="value">The decimal value to write.</param>
	public override void Write(decimal value)
	{
		var pos = _position;
		Advance(16);

#if NET6_0_OR_GREATER
		Span<byte> span = _buffer.AsSpan(pos);
		decimal.GetBits(value, MemoryMarshal.Cast<byte, int>(span));
#else
		var bits = decimal.GetBits(value);

		Write(bits[0], pos);
		Write(bits[1], pos + 4);
		Write(bits[2], pos + 4 + 4);
		Write(bits[3], pos + 4 + 4 + 4);
#endif
	}

	/// <summary>
	/// Writes a double-precision floating-point number to the underlying byte array and advances the current position by eight bytes.
	/// </summary>
	/// <param name="value">The double-precision floating-point number to write.</param>
	public override unsafe void Write(double value)
	{
		var pos = _position;
		Advance(8);

#if NET6_0_OR_GREATER
		var span = _buffer.AsSpan(pos);
		Unsafe.WriteUnaligned<double>(ref MemoryMarshal.GetReference<byte>(span), value);
#else
		var buff = _buffer;
		ulong tmpValue = *(ulong*)&value;
		buff[pos + 0] = (byte)tmpValue;
		buff[pos + 1] = (byte)(tmpValue >> 8);
		buff[pos + 2] = (byte)(tmpValue >> 16);
		buff[pos + 3] = (byte)(tmpValue >> 24);
		buff[pos + 4] = (byte)(tmpValue >> 32);
		buff[pos + 5] = (byte)(tmpValue >> 40);
		buff[pos + 6] = (byte)(tmpValue >> 48);
		buff[pos + 7] = (byte)(tmpValue >> 56);
#endif
	}

	/// <summary>
	/// Writes a single-precision floating-point number to the underlying byte array and advances the current position by one byte.
	/// </summary>
	/// <param name="value">The single-precision floating-point number to write.</param>
	public override unsafe void Write(float value)
	{
		var pos = _position;
		Advance(4);

#if NET6_0_OR_GREATER
		var span = _buffer.AsSpan(pos);
		Unsafe.WriteUnaligned<float>(ref MemoryMarshal.GetReference<byte>(span), value);
#else
		uint tmpValue = *(uint*)&value;
		_buffer[pos + 0] = (byte)tmpValue;
		_buffer[pos + 1] = (byte)(tmpValue >> 8);
		_buffer[pos + 2] = (byte)(tmpValue >> 16);
		_buffer[pos + 3] = (byte)(tmpValue >> 24);
#endif
	}

	/// <summary>
	/// Writes a 16-bit signed integer to the underlying byte array and advances the current position by two bytes.
	/// </summary>
	/// <param name="value">The 16-bit signed integer to write.</param>
	public override void Write(short value)
	{
		var pos = _position;
		Advance(2);

#if NET6_0_OR_GREATER
		var span = _buffer.AsSpan(pos);
		Unsafe.WriteUnaligned<short>(ref MemoryMarshal.GetReference<byte>(span), value);
#else
		_buffer[pos + 0] = (byte)value;
		_buffer[pos + 1] = (byte)(value >> 8);
#endif
	}

	/// <summary>
	/// Writes a 16-bit unsigned integer to the underlying byte array and advances the current position by two bytes.
	/// </summary>
	/// <param name="value">The 16-bit unsigned integer to write.</param>
	public override void Write(ushort value)
	{
		var pos = _position;
		Advance(2);

#if NET6_0_OR_GREATER
		var span = _buffer.AsSpan(pos);
		Unsafe.WriteUnaligned<ushort>(ref MemoryMarshal.GetReference<byte>(span), value);
#else
		_buffer[pos + 0] = (byte)value;
		_buffer[pos + 1] = (byte)(value >> 8);
#endif
	}

	/// <summary>
	/// Writes a 32-bit signed integer to the underlying byte array and advances the current position by four bytes.
	/// </summary>
	/// <param name="value">The 32-bit signed integer to write.</param>
	public override void Write(int value)
	{
		var pos = _position;
		Advance(4);

#if NET6_0_OR_GREATER
		var span = _buffer.AsSpan(pos);
		Unsafe.WriteUnaligned<int>(ref MemoryMarshal.GetReference<byte>(span), value);
#else
		_buffer[pos + 0] = (byte)value;
		_buffer[pos + 1] = (byte)(value >> 8);
		_buffer[pos + 2] = (byte)(value >> 16);
		_buffer[pos + 3] = (byte)(value >> 24);
#endif
	}

	/// <summary>
	/// Writes a 32-bit unsigned integer to the underlying byte array and advances the current position by four bytes.
	/// </summary>
	/// <param name="value">The 32-bit unsigned integer to write.</param>
	public override void Write(uint value)
	{
		var pos = _position;
		Advance(4);

#if NET6_0_OR_GREATER
		var span = _buffer.AsSpan(pos);
		Unsafe.WriteUnaligned<uint>(ref MemoryMarshal.GetReference<byte>(span), value);
#else
		_buffer[pos + 0] = (byte)value;
		_buffer[pos + 1] = (byte)(value >> 8);
		_buffer[pos + 2] = (byte)(value >> 16);
		_buffer[pos + 3] = (byte)(value >> 24);
#endif
	}

	/// <summary>
	/// Writes a 64-bit signed integer to the underlying byte array and advances the current position by eight bytes.
	/// </summary>
	/// <param name="value">The 64-bit signed integer to write.</param>
	public override void Write(long value)
	{
		var pos = _position;
		Advance(8);

#if NET6_0_OR_GREATER
		var span = _buffer.AsSpan(pos);
		Unsafe.WriteUnaligned<long>(ref MemoryMarshal.GetReference<byte>(span), value);
#else
		var buff = _buffer;
		buff[pos + 0] = (byte)value;
		buff[pos + 1] = (byte)(value >> 8);
		buff[pos + 2] = (byte)(value >> 16);
		buff[pos + 3] = (byte)(value >> 24);
		buff[pos + 4] = (byte)(value >> 32);
		buff[pos + 5] = (byte)(value >> 40);
		buff[pos + 6] = (byte)(value >> 48);
		buff[pos + 7] = (byte)(value >> 56);
#endif
	}

	/// <summary>
	/// Writes a 64-bit unsigned integer value to the underlying byte array and advances the current position by eight bytes.
	/// </summary>
	/// <param name="value">The 64-bit unsigned integer to write.</param>
	public override void Write(ulong value)
	{
		var pos = _position;
		Advance(8);

#if NET6_0_OR_GREATER
		var span = _buffer.AsSpan(pos);
		Unsafe.WriteUnaligned<ulong>(ref MemoryMarshal.GetReference<byte>(span), value);
#else
		var buff = _buffer;
		buff[pos + 0] = (byte)value;
		buff[pos + 1] = (byte)(value >> 8);
		buff[pos + 2] = (byte)(value >> 16);
		buff[pos + 3] = (byte)(value >> 24);
		buff[pos + 4] = (byte)(value >> 32);
		buff[pos + 5] = (byte)(value >> 40);
		buff[pos + 6] = (byte)(value >> 48);
		buff[pos + 7] = (byte)(value >> 56);
#endif
	}

	/// <summary>
	/// Copies a span of bytes to the underlying byte array and advances the current position by the number of bytes written.
	/// </summary>
	/// <param name="buffer">The span of bytes to write.</param>
	public override void Write(ReadOnlySpan<byte> buffer)
	{
		var pos = _position;
		var length = buffer.Length;
		Advance(length);

		buffer.CopyTo(pos == 0 ? _buffer : new Span<byte>(_buffer, pos, length));
	}

	/// <summary>
	/// Creates a span over the underlying byte array of the writer.
	/// </summary>
	public ReadOnlySpan<byte> ToReadOnlySpan() => new(_buffer, _offset, _writtenLength);

	/// <summary>
	/// Returns a span over the remaining written bytes of the underlying byte array of the writer.
	/// Can be useful when used together with position property.
	/// </summary>
	public ReadOnlySpan<byte> RemainingToReadOnlySpan() => new(_buffer, _position, _writtenLength - _relativePositon);

	/// <summary>
	/// Creates a span over the underlying byte array of the writer.
	/// </summary>
	public ArraySegment<byte> ToArraySegment() => new(_buffer, _offset, _writtenLength);

	/// <summary>
	/// Returns a ArraySegment over the remaining written bytes of the underlying byte array of the writer.
	/// Can be useful when used together with position property.
	/// </summary>
	public ArraySegment<byte> RemainingToArraySegment() => new(_buffer, _position, _writtenLength - _relativePositon);

	/// <summary>
	/// Returns the underlying byte array of the writer.
	/// </summary>
	public byte[] ToArray() => ToReadOnlySpan().ToArray();

	/// <summary>
	/// Returns the remaining written bytes of the underlying byte array of the writer.
	/// Can be useful when used together with position property.
	/// </summary>
	public byte[] RemainingToArray() => RemainingToReadOnlySpan().ToArray();

#if !NET6_0_OR_GREATER
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void Write(int value, int pos)
	{
		_buffer[pos + 0] = (byte)value;
		_buffer[pos + 1] = (byte)(value >> 8);
		_buffer[pos + 2] = (byte)(value >> 16);
		_buffer[pos + 3] = (byte)(value >> 24);
	}
#endif

	/// <summary>
	/// Moves the current position of the writer ahead by the specified number of bytes.
	/// </summary>
	/// <param name="count">The number of bytes to advance</param>
	/// <exception cref="EndOfStreamException"/>
	public void SimulateWrite(int count)
	{
		Advance(count);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void Advance(int count)
	{
		var newPos = _position + count;
		int relPos = _relativePositon + count;

		if ((uint)relPos > (uint)_length)
		{
			_relativePositon = _length;
			throw ExceptionHelper.EndOfDataException();
		}

		_relativePositon = relPos;
		_position = newPos;

		if (count > 0) _writtenLength = Math.Max(_relativePositon, _writtenLength);
	}
}
