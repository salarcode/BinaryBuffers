using System;
using System.Runtime.CompilerServices;
#if NET6_0_OR_GREATER
using System.Runtime.InteropServices;
#endif

namespace Salar.BinaryBuffers;

/// <summary>
/// Provides a writer for writing primitive data types to a byte span.
/// </summary>
/// <remarks>
/// <para>
/// <see cref="BinarySpanBufferWriter"/> is a high-performance, zero-allocation writer that operates directly on <see cref="Span{T}"/>.
/// As a <c>ref struct</c>, it can work with stack-allocated memory (<c>stackalloc</c>) for optimal performance.
/// </para>
/// 
/// <para><strong>Basic Usage:</strong></para>
/// <code>
/// Span&lt;byte&gt; buffer = stackalloc byte[1024];
/// var writer = new BinarySpanBufferWriter(buffer);
/// writer.Write(42);
/// writer.Write(3.14);
/// ReadOnlySpan&lt;byte&gt; written = writer.ToReadOnlySpan();
/// </code>
/// 
/// <para><strong>Using with IBufferWriter in Generic Methods:</strong></para>
/// <code>
/// void Serialize&lt;TWriter&gt;(TWriter writer, int id) where TWriter : IBufferWriter
/// {
///     writer.Write(id);
/// }
/// 
/// Span&lt;byte&gt; buffer = stackalloc byte[1024];
/// var writer = new BinarySpanBufferWriter(buffer);
/// Serialize(writer, 42);  // Works with generic constraint
/// </code>
/// 
/// <para><strong>Ref Struct Limitations:</strong></para>
/// <list type="bullet">
/// <item><description>Cannot be stored as a field (use local variables or method parameters only)</description></item>
/// <item><description>Cannot be boxed to <see cref="IBufferWriter"/> interface directly</description></item>
/// <item><description>Cannot be captured in lambdas or used in async methods</description></item>
/// <item><description>Lifetime restricted to the current method scope</description></item>
/// </list>
/// 
/// <para>
/// Use <see cref="BinarySpanBufferWriter"/> for maximum performance with stack allocation.
/// Use <see cref="BinaryBufferWriter"/> when you need to store the writer as a field or work with async code.
/// </para>
/// </remarks>
public ref struct BinarySpanBufferWriter: IBufferWriter
{
	private Span<byte> _buffer;
	private int _position;
	private int _writtenLength;

	/// <summary>
	/// Gets the offset of the span in the original buffer.
	/// </summary>
	public int Offset => 0;

	/// <summary>
	/// Gets the length of the span.
	/// </summary>
	public int Length => _buffer.Length;

	/// <summary>
	/// Gets or sets the current position within the span.
	/// </summary>
	public int Position
	{
		get => _position;
		set
		{
			if (value < 0) throw ExceptionHelper.PositionLessThanZeroException(nameof(value));
			if (value > _buffer.Length) throw ExceptionHelper.PositionGreaterThanLengthOfByteArrayException(nameof(value));

			_position = value;
		}
	}

	/// <summary>
	/// Gets the total number of bytes written to the underlying byte span.
	/// </summary>
	public int WrittenLength => _writtenLength;

	/// <summary>
	/// Initializes a new instance of the <see cref="BinarySpanBufferWriter"/> struct using the specified byte span to write the output to.
	/// </summary>
	/// <param name="buffer">The byte span to write to.</param>
	public BinarySpanBufferWriter(Span<byte> buffer)
	{
		_buffer = buffer;
		_position = 0;
		_writtenLength = 0;
	}

	/// <summary>
	/// Reset the underlying buffer using the specified byte span to write the output to.
	/// </summary>
	/// <param name="buffer">The byte span to write to.</param>
	public void ResetBuffer(Span<byte> buffer)
	{
		_buffer = buffer;
		_position = 0;
		_writtenLength = 0;
	}

	/// <summary>
	/// Reset the status of what is written; doesn't clean the underlying buffer.
	/// </summary>
	public void ResetBuffer()
	{
		_position = 0;
		_writtenLength = 0;
	}

	/// <summary>
	/// Writes a boolean value to the underlying byte span and advances the current position by one byte.
	/// </summary>
	/// <param name="value">The boolean value to write.</param>
	public void Write(bool value)
	{
		var pos = _position;
		Advance(1);

		_buffer[pos] = (byte)(value ? 1 : 0);
	}

	/// <summary>
	/// Writes a byte to the underlying byte span and advances the current position by one byte.
	/// </summary>
	/// <param name="value">The byte value to write.</param>
	public void Write(byte value)
	{
		var pos = _position;
		Advance(1);

		_buffer[pos] = value;
	}

	/// <summary>
	/// Writes a signed byte to the underlying byte span and advances the current position by one byte.
	/// </summary>
	/// <param name="value">The signed byte value to write.</param>
	public void Write(sbyte value)
	{
		var pos = _position;
		Advance(1);

		_buffer[pos] = (byte)value;
	}

	/// <summary>
	/// Copies the contents of a byte array to the underlying byte span of the writer and advances the current position by the number of bytes written.
	/// </summary>
	/// <param name="buffer">The buffer to copy data from.</param>
	public void Write(byte[] buffer)
	{
		if (buffer == null) throw new ArgumentNullException(nameof(buffer));

		var pos = _position;
		var length = buffer.Length;
		Advance(length);

		buffer.AsSpan().CopyTo(_buffer.Slice(pos, length));
	}

	/// <summary>
	/// Copies a region of a byte array to the underlying byte span of the writer and advances the current position by the number of bytes written.
	/// </summary>
	/// <param name="buffer">The buffer to copy data from.</param>
	/// <param name="offset">The 0-based offset in buffer at which to start copying from.</param>
	/// <param name="length">The number of bytes to copy.</param>
	public void Write(byte[] buffer, int offset, int length)
	{
		if (buffer == null) throw new ArgumentNullException(nameof(buffer));

		var pos = _position;
		Advance(length);

		buffer.AsSpan(offset, length).CopyTo(_buffer.Slice(pos, length));
	}

	/// <summary>
	/// Writes a decimal value to the underlying byte span and advances the current position by sixteen bytes.
	/// </summary>
	/// <param name="value">The decimal value to write.</param>
	public void Write(decimal value)
	{
		var pos = _position;
		Advance(16);

#if NET6_0_OR_GREATER
		Span<byte> span = _buffer.Slice(pos);
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
	/// Writes a double-precision floating-point number to the underlying byte span and advances the current position by eight bytes.
	/// </summary>
	/// <param name="value">The double-precision floating-point number to write.</param>
	public unsafe void Write(double value)
	{
		var pos = _position;
		Advance(8);

#if NET6_0_OR_GREATER
		var span = _buffer.Slice(pos);
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
	/// Writes a single-precision floating-point number to the underlying byte span and advances the current position by four bytes.
	/// </summary>
	/// <param name="value">The single-precision floating-point number to write.</param>
	public unsafe void Write(float value)
	{
		var pos = _position;
		Advance(4);

#if NET6_0_OR_GREATER
		var span = _buffer.Slice(pos);
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
	/// Writes a 16-bit signed integer to the underlying byte span and advances the current position by two bytes.
	/// </summary>
	/// <param name="value">The 16-bit signed integer to write.</param>
	public void Write(short value)
	{
		var pos = _position;
		Advance(2);

#if NET6_0_OR_GREATER
		var span = _buffer.Slice(pos);
		Unsafe.WriteUnaligned<short>(ref MemoryMarshal.GetReference<byte>(span), value);
#else
		_buffer[pos + 0] = (byte)value;
		_buffer[pos + 1] = (byte)(value >> 8);
#endif
	}

	/// <summary>
	/// Writes a 16-bit unsigned integer to the underlying byte span and advances the current position by two bytes.
	/// </summary>
	/// <param name="value">The 16-bit unsigned integer to write.</param>
	public void Write(ushort value)
	{
		var pos = _position;
		Advance(2);

#if NET6_0_OR_GREATER
		var span = _buffer.Slice(pos);
		Unsafe.WriteUnaligned<ushort>(ref MemoryMarshal.GetReference<byte>(span), value);
#else
		_buffer[pos + 0] = (byte)value;
		_buffer[pos + 1] = (byte)(value >> 8);
#endif
	}

	/// <summary>
	/// Writes a 32-bit signed integer to the underlying byte span and advances the current position by four bytes.
	/// </summary>
	/// <param name="value">The 32-bit signed integer to write.</param>
	public void Write(int value)
	{
		var pos = _position;
		Advance(4);

#if NET6_0_OR_GREATER
		var span = _buffer.Slice(pos);
		Unsafe.WriteUnaligned<int>(ref MemoryMarshal.GetReference<byte>(span), value);
#else
		_buffer[pos + 0] = (byte)value;
		_buffer[pos + 1] = (byte)(value >> 8);
		_buffer[pos + 2] = (byte)(value >> 16);
		_buffer[pos + 3] = (byte)(value >> 24);
#endif
	}

	/// <summary>
	/// Writes a 32-bit unsigned integer to the underlying byte span and advances the current position by four bytes.
	/// </summary>
	/// <param name="value">The 32-bit unsigned integer to write.</param>
	public void Write(uint value)
	{
		var pos = _position;
		Advance(4);

#if NET6_0_OR_GREATER
		var span = _buffer.Slice(pos);
		Unsafe.WriteUnaligned<uint>(ref MemoryMarshal.GetReference<byte>(span), value);
#else
		_buffer[pos + 0] = (byte)value;
		_buffer[pos + 1] = (byte)(value >> 8);
		_buffer[pos + 2] = (byte)(value >> 16);
		_buffer[pos + 3] = (byte)(value >> 24);
#endif
	}

	/// <summary>
	/// Writes a 64-bit signed integer to the underlying byte span and advances the current position by eight bytes.
	/// </summary>
	/// <param name="value">The 64-bit signed integer to write.</param>
	public void Write(long value)
	{
		var pos = _position;
		Advance(8);

#if NET6_0_OR_GREATER
		var span = _buffer.Slice(pos);
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
	/// Writes a 64-bit unsigned integer value to the underlying byte span and advances the current position by eight bytes.
	/// </summary>
	/// <param name="value">The 64-bit unsigned integer to write.</param>
	public void Write(ulong value)
	{
		var pos = _position;
		Advance(8);

#if NET6_0_OR_GREATER
		var span = _buffer.Slice(pos);
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
	/// Copies a span of bytes to the underlying byte span and advances the current position by the number of bytes written.
	/// </summary>
	/// <param name="buffer">The span of bytes to write.</param>
	public void Write(ReadOnlySpan<byte> buffer)
	{
		var pos = _position;
		var length = buffer.Length;
		Advance(length);

		buffer.CopyTo(_buffer.Slice(pos, length));
	}

	/// <summary>
	/// Creates a span over the written bytes of the underlying span.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public ReadOnlySpan<byte> ToReadOnlySpan() => _buffer.Slice(0, _writtenLength);

	/// <summary>
	/// Returns a span over the remaining written bytes of the underlying span.
	/// Can be useful when used together with position property.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public ReadOnlySpan<byte> RemainingToReadOnlySpan() => _buffer.Slice(_position, _writtenLength - _position);

	/// <summary>
	/// Returns a copy of the written bytes as a byte array.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public byte[] ToArray() => ToReadOnlySpan().ToArray();

	/// <summary>
	/// Returns the remaining written bytes as a byte array.
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
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void SimulateWrite(int count)
	{
		Advance(count);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void Advance(int count)
	{
		var newPos = _position + count;

		if ((uint)newPos > (uint)_buffer.Length)
		{
			_position = _buffer.Length;
			throw ExceptionHelper.EndOfDataException();
		}

		_position = newPos;

		if (count > 0) _writtenLength = Math.Max(_position, _writtenLength);
	}
}
