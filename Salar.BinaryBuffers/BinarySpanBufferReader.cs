using System;
using System.Buffers.Binary;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
#if NET6_0_OR_GREATER
using System.Diagnostics.CodeAnalysis;
#endif

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
#if NET7_0_OR_GREATER
	// Ref-readonly byte allows reading bytes safely without mutating them,
	// while letting ResetBuffer rebind _bufferHead to a new buffer.
	private ref readonly byte _bufferHead;
	private int _length;
#else
	private ReadOnlySpan<byte> _buffer;
#endif
	private int _position;

	/// <summary>
	/// Gets the offset of this reader relative to an original backing buffer.
	/// For <see cref="BinarySpanBufferReader"/>, this is always <c>0</c>.
	/// </summary>
	public int Offset => 0;

	/// <summary>
	/// Gets the length of the readable span.
	/// </summary>
	public int Length =>
#if NET7_0_OR_GREATER
		_length;
#else
		_buffer.Length;
#endif

	/// <summary>
	/// Gets or sets the current reading position within the span.
	/// </summary>
	public int Position
	{
		get => _position;
		set
		{
			if (value < 0) throw ExceptionHelper.PositionLessThanZeroException(nameof(value));
			if (value > Length) throw ExceptionHelper.PositionGreaterThanLengthOfByteArrayException(nameof(value));

			_position = value;
		}
	}

	/// <summary>
	/// Gets the number of bytes remaining in the span.
	/// </summary>
	public int Remaining => Length - _position;

	/// <summary>
	/// Initializes a new instance of the <see cref="BinarySpanBufferReader"/> struct using the specified byte span.
	/// </summary>
	/// <param name="buffer">The byte span to read.</param>
	public BinarySpanBufferReader(ReadOnlySpan<byte> buffer)
	{
#if NET7_0_OR_GREATER
		_bufferHead = ref MemoryMarshal.GetReference(buffer);
		_length = buffer.Length;
#else
		_buffer = buffer;
#endif
		_position = 0;
	}

	/// <summary>
	/// Resets the underlying buffer using the specified byte span.
	/// </summary>
	/// <param name="buffer">The byte span to read.</param>
	public void ResetBuffer(ReadOnlySpan<byte> buffer)
	{
#if NET7_0_OR_GREATER
		_bufferHead = ref MemoryMarshal.GetReference(buffer);
		_length = buffer.Length;
#else
		_buffer = buffer;
#endif
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
		ref byte data = ref AdvanceAsRef(1);
		return data;
	}

	/// <summary>
	/// Reads a signed byte and advances the current position by one byte.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public sbyte ReadSByte() => (sbyte)ReadByte();

	/// <summary>
	/// Reads a decimal value and advances the current position by sixteen bytes.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public decimal ReadDecimal()
	{
		ref byte data = ref AdvanceAsRef(16);
#if NET6_0_OR_GREATER
		return Unsafe.ReadUnaligned<decimal>(ref data);
#else
		var lo = Unsafe.ReadUnaligned<int>(ref data);
		var mid = Unsafe.ReadUnaligned<int>(ref Unsafe.Add(ref data, 4));
		var hi = Unsafe.ReadUnaligned<int>(ref Unsafe.Add(ref data, 8));
		var flags = Unsafe.ReadUnaligned<int>(ref Unsafe.Add(ref data, 12));

		try
		{
			return new decimal([lo, mid, hi, flags]);
		}
		catch (ArgumentException exception)
		{
			throw ExceptionHelper.DecimalReadingException(exception);
		}
#endif
	}

	/// <summary>
	/// Reads a single-precision floating-point number and advances the current position by four bytes.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public float ReadSingle()
	{
		ref byte data = ref AdvanceAsRef(4);
		return Unsafe.ReadUnaligned<float>(ref data);
	}

	/// <summary>
	/// Reads a double-precision floating-point number and advances the current position by eight bytes.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public double ReadDouble()
	{
		ref byte data = ref AdvanceAsRef(8);
		return Unsafe.ReadUnaligned<double>(ref data);
	}

	/// <summary>
	/// Reads a 16-bit signed integer and advances the current position by two bytes.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public short ReadInt16()
	{
		ref byte data = ref AdvanceAsRef(2);
		return Unsafe.ReadUnaligned<short>(ref data);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public int ReadInt32()
	{
		ref byte data = ref AdvanceAsRef(4);
		return Unsafe.ReadUnaligned<int>(ref data);
	}

	/// <summary>
	/// Reads a 64-bit signed integer and advances the current position by eight bytes.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public long ReadInt64()
	{
		ref byte data = ref AdvanceAsRef(8);
		return Unsafe.ReadUnaligned<long>(ref data);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public ushort ReadUInt16()
	{
		ref byte data = ref AdvanceAsRef(2);
		return Unsafe.ReadUnaligned<ushort>(ref data);
	}

	/// <summary>
	/// Reads a 32-bit unsigned integer and advances the current position by four bytes.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public uint ReadUInt32()
	{
		ref byte data = ref AdvanceAsRef(4);
		return Unsafe.ReadUnaligned<uint>(ref data);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public ulong ReadUInt64()
	{
		ref byte data = ref AdvanceAsRef(8);
		return Unsafe.ReadUnaligned<ulong>(ref data);
	}

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

		return AdvanceAsSpan(count);
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

		var remaining = Length - _position;
		if (count > remaining)
			count = remaining;

		if (count == 0)
			return 0;

		ReadSpan(count).CopyTo(buffer.AsSpan(index, count));
		return count;
	}

	/// <summary>
	/// Low-level helper that advances position and returns a reference to the start of the read region.
	/// Eliminates Span struct creation overhead on primitive reads.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private ref byte AdvanceAsRef(int count)
	{
		int position = _position;
		int newPosition = position + count;

		if ((uint)newPosition > (uint)Length)
		{
			ThrowEndOfDataException();
		}

		_position = newPosition;

#if NET7_0_OR_GREATER
		return ref Unsafe.Add(ref Unsafe.AsRef(in _bufferHead), position);
#else
		return ref Unsafe.Add(ref MemoryMarshal.GetReference(_buffer), position);
#endif
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private ReadOnlySpan<byte> AdvanceAsSpan(int count)
	{
		ref byte data = ref AdvanceAsRef(count);

#if NET6_0_OR_GREATER
		return MemoryMarshal.CreateReadOnlySpan(ref data, count);
#else
		return _buffer.Slice(_position - count, count);
#endif
	}

#if NET6_0_OR_GREATER
	[DoesNotReturn]
#endif
	[MethodImpl(MethodImplOptions.NoInlining)]
	private void ThrowEndOfDataException()
	{
		_position = Length;
		throw ExceptionHelper.EndOfDataException();
	}
}