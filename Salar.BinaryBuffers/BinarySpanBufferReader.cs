using System;
using System.Buffers.Binary;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Salar.BinaryBuffers;

/// <summary>
/// Provides a reader for reading primitive data types from a read-only byte span.
/// </summary>
/// <remarks>
/// <para>
/// <see cref="BinarySpanBufferReader"/> is a high-performance, zero-allocation reader that operates directly on <see cref="ReadOnlySpan{T}"/>.
/// As a <c>ref struct</c>, it can read stack-allocated memory (<c>stackalloc</c>) without copying.
/// </para>
/// <para>
/// Use <see cref="BinarySpanBufferReader"/> for maximum performance with span-based input.
/// Use <see cref="BinaryBufferReader"/> when the reader must be stored as a field or used by async code.
/// </para>
/// </remarks>
public ref struct BinarySpanBufferReader
{
	private ReadOnlySpan<byte> _buffer;
	private int _position;

	/// <summary>
	/// Gets the offset of the span in the original buffer.
	/// </summary>
	public int Offset => 0;

	/// <summary>
	/// Gets the length of the readable span.
	/// </summary>
	public int Length => _buffer.Length;

	/// <summary>
	/// Gets or sets the current reading position within the span.
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
	/// Gets the number of bytes remaining in the span.
	/// </summary>
	public int Remaining => _buffer.Length - _position;

	/// <summary>
	/// Initializes a new instance of the <see cref="BinarySpanBufferReader"/> struct using the specified byte span.
	/// </summary>
	/// <param name="buffer">The byte span to read.</param>
	public BinarySpanBufferReader(ReadOnlySpan<byte> buffer)
	{
		_buffer = buffer;
		_position = 0;
	}

	/// <summary>
	/// Resets the underlying buffer using the specified byte span.
	/// </summary>
	/// <param name="buffer">The byte span to read.</param>
	public void ResetBuffer(ReadOnlySpan<byte> buffer)
	{
		_buffer = buffer;
		_position = 0;
	}

	/// <summary>
	/// Resets the current reading position without changing the underlying span.
	/// </summary>
	public void ResetBuffer()
	{
		_position = 0;
	}

	/// <summary>
	/// Reads a boolean value and advances the current position by one byte.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool ReadBoolean() => ReadByte() != 0;

	/// <summary>
	/// Reads a byte and advances the current position by one byte.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public byte ReadByte()
	{
		var position = _position;
		AdvanceOne();
		return _buffer[position];
	}

	/// <summary>
	/// Reads a signed byte and advances the current position by one byte.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public sbyte ReadSByte() => (sbyte)ReadByte();

	/// <summary>
	/// Reads a decimal value and advances the current position by sixteen bytes.
	/// </summary>
	public decimal ReadDecimal()
	{
		var position = Advance(16);
		ref byte data = ref Unsafe.Add(ref MemoryMarshal.GetReference(_buffer), position);
		var lo = Unsafe.ReadUnaligned<int>(ref data);
		var mid = Unsafe.ReadUnaligned<int>(ref Unsafe.Add(ref data, 4));
		var hi = Unsafe.ReadUnaligned<int>(ref Unsafe.Add(ref data, 8));
		var flags = Unsafe.ReadUnaligned<int>(ref Unsafe.Add(ref data, 12));

		if (!BitConverter.IsLittleEndian)
		{
			lo = BinaryPrimitives.ReverseEndianness(lo);
			mid = BinaryPrimitives.ReverseEndianness(mid);
			hi = BinaryPrimitives.ReverseEndianness(hi);
			flags = BinaryPrimitives.ReverseEndianness(flags);
		}

		try
		{
			return new decimal(
#if NET6_0_OR_GREATER
				stackalloc[] { lo, mid, hi, flags });
#else
				[lo, mid, hi, flags]);
#endif
		}
		catch (ArgumentException exception)
		{
			throw ExceptionHelper.DecimalReadingException(exception);
		}
	}

	/// <summary>
	/// Reads a single-precision floating-point number and advances the current position by four bytes.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public float ReadSingle() => ReadUnaligned<float>(4);

	/// <summary>
	/// Reads a double-precision floating-point number and advances the current position by eight bytes.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public double ReadDouble() => ReadUnaligned<double>(8);

	/// <summary>
	/// Reads a 16-bit signed integer and advances the current position by two bytes.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public short ReadInt16() => ReadUnaligned<short>(2);

	/// <summary>
	/// Reads a 32-bit signed integer and advances the current position by four bytes.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public int ReadInt32() => ReadUnaligned<int>(4);

	/// <summary>
	/// Reads a 64-bit signed integer and advances the current position by eight bytes.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public long ReadInt64() => ReadUnaligned<long>(8);

	/// <summary>
	/// Reads a 16-bit unsigned integer and advances the current position by two bytes.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public ushort ReadUInt16() => ReadUnaligned<ushort>(2);

	/// <summary>
	/// Reads a 32-bit unsigned integer and advances the current position by four bytes.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public uint ReadUInt32() => ReadUnaligned<uint>(4);

	/// <summary>
	/// Reads a 64-bit unsigned integer and advances the current position by eight bytes.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public ulong ReadUInt64() => ReadUnaligned<ulong>(8);

	/// <summary>
	/// Reads the specified number of bytes into a new byte array and advances the current position.
	/// </summary>
	/// <param name="count">The number of bytes to read.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public byte[] ReadBytes(int count) => ReadSpan(count).ToArray();

	/// <summary>
	/// Reads the specified number of bytes as a span and advances the current position.
	/// </summary>
	/// <param name="count">The number of bytes to read.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public ReadOnlySpan<byte> ReadSpan(int count)
	{
		if (count <= 0)
			return ReadOnlySpan<byte>.Empty;

		var position = Advance(count);
		return _buffer.Slice(position, count);
	}

	/// <summary>
	/// Reads bytes into a region of an array and advances the current position by the number of bytes read.
	/// </summary>
	/// <param name="buffer">The destination array.</param>
	/// <param name="index">The destination offset at which to begin writing.</param>
	/// <param name="count">The maximum number of bytes to read.</param>
	/// <returns>The number of bytes read, which can be less than requested at the end of the span.</returns>
	public int Read(byte[] buffer, int index, int count)
	{
		if (count <= 0)
			return 0;

		var remaining = _buffer.Length - _position;
		if (count > remaining)
			count = remaining;

		if (count == 0)
			return 0;

		ReadSpan(count).CopyTo(buffer.AsSpan(index, count));
		return count;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private T ReadUnaligned<T>(int size) where T : unmanaged
	{
		var position = Advance(size);
		ref byte source = ref Unsafe.Add(ref MemoryMarshal.GetReference(_buffer), position);
		return Unsafe.ReadUnaligned<T>(ref source);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void AdvanceOne()
	{
		var newPosition = _position + 1;
		if ((uint)newPosition > (uint)_buffer.Length)
		{
			ThrowEndOfDataException();
		}

		_position = newPosition;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private int Advance(int count)
	{
		var position = _position;
		var newPosition = position + count;
		if ((uint)newPosition > (uint)_buffer.Length)
		{
			ThrowEndOfDataException();
		}

		_position = newPosition;
		return position;
	}

#if NET6_0_OR_GREATER
	[DoesNotReturn]
#endif
	[MethodImpl(MethodImplOptions.NoInlining)]
	private void ThrowEndOfDataException()
	{
		_position = _buffer.Length;
		throw ExceptionHelper.EndOfDataException();
	}
}
