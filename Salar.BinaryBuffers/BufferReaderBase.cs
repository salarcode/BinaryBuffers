using System;
using System.Buffers.Binary;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Salar.BinaryBuffers;

/// <summary>
/// Represents a reader that can read primitive data types from a binary data source.
/// </summary>
public abstract class BufferReaderBase : IBufferReader
{
#if NET6_0_OR_GREATER && DISABLED
	delegate decimal DecimalToDecimal(ReadOnlySpan<byte> span);
	private static readonly DecimalToDecimal _decimalToDecimal = (DecimalToDecimal)typeof(decimal)
			  .GetMethod("ToDecimal", BindingFlags.Static | BindingFlags.NonPublic, new[] { typeof(ReadOnlySpan<byte>) })
			  .CreateDelegate(typeof(DecimalToDecimal));
	
	delegate decimal CreateDecimal(int lo, int mid, int hi, int flags);
	private static CreateDecimal _createDecimal;

	private static void InitializeCreateDecimal()
	{
		var paramLo = Expression.Parameter(typeof(int), "lo");
		var paramMid = Expression.Parameter(typeof(int), "mi");
		var paramHi = Expression.Parameter(typeof(int), "hi");
		var paramFlags = Expression.Parameter(typeof(int), "flags");

		var ctor = typeof(Decimal).GetConstructor(
			BindingFlags.Instance | BindingFlags.NonPublic,
			null,
			new[] { typeof(int), typeof(int), typeof(int), typeof(int) },
			null);

		var lambda = Expression.Lambda<CreateDecimal>(
			Expression.New(ctor, paramLo, paramMid, paramHi, paramFlags), paramLo, paramMid, paramHi, paramFlags);

		_createDecimal = lambda.Compile();
	}
#endif

	/// <inheritdoc/>
	public abstract int Offset { get; }

	/// <inheritdoc/>
	public abstract int Length { get; }

	/// <inheritdoc/>
	public abstract int Position { get; set; }

	/// <inheritdoc/>
	public abstract int Remaining { get; }

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool ReadBoolean() => InternalReadByte() != 0;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public byte ReadByte() => InternalReadByte();

	/// <inheritdoc/>
	public abstract byte[] ReadBytes(int count);

	/// <inheritdoc/>
	public abstract int Read(byte[] buffer, int index, int count);

	/// <inheritdoc/>
	public abstract ReadOnlySpan<byte> ReadSpan(int count);

	/// <inheritdoc/>
	public abstract ReadOnlyMemory<byte> ReadMemory(int count);

	/// <inheritdoc/>
	public unsafe decimal ReadDecimal()
	{
		var span = InternalReadSpan(16);
		ref byte data = ref MemoryMarshal.GetReference(span);
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
				stackalloc
#else
				new
#endif
				[] { lo, mid, hi, flags });
		}
		catch (ArgumentException e)
		{
			// ReadDecimal cannot leak out ArgumentException
			throw ExceptionHelper.DecimalReadingException(e);
		}
	}

	/// <inheritdoc/>
	public virtual float ReadSingle()
	{
		var span = InternalReadSpan(4);
		return Unsafe.ReadUnaligned<float>(ref MemoryMarshal.GetReference<byte>(span));
	}

	/// <inheritdoc/>
	public virtual double ReadDouble()
	{
		var span = InternalReadSpan(8);
		return Unsafe.ReadUnaligned<double>(ref MemoryMarshal.GetReference<byte>(span));
	}

	/// <inheritdoc/>
	public virtual short ReadInt16()
	{
		var span = InternalReadSpan(2);
		return Unsafe.ReadUnaligned<short>(ref MemoryMarshal.GetReference<byte>(span));
	}

	/// <inheritdoc/>
	public virtual int ReadInt32()
	{
		var span = InternalReadSpan(4);
		return Unsafe.ReadUnaligned<int>(ref MemoryMarshal.GetReference<byte>(span));
	}

	/// <inheritdoc/>
	public virtual long ReadInt64()
	{
		var span = InternalReadSpan(8);
		return Unsafe.ReadUnaligned<long>(ref MemoryMarshal.GetReference<byte>(span));
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public sbyte ReadSByte() => (sbyte)InternalReadByte();

	/// <inheritdoc/>
	public virtual ushort ReadUInt16()
	{
		var span = InternalReadSpan(2);
		return Unsafe.ReadUnaligned<ushort>(ref MemoryMarshal.GetReference<byte>(span));
	}

	/// <inheritdoc/>
	public virtual uint ReadUInt32()
	{
		var span = InternalReadSpan(4);
		return Unsafe.ReadUnaligned<uint>(ref MemoryMarshal.GetReference<byte>(span));
	}

	/// <inheritdoc/>
	public virtual ulong ReadUInt64()
	{
		var span = InternalReadSpan(8);
		return Unsafe.ReadUnaligned<ulong>(ref MemoryMarshal.GetReference<byte>(span));
	}

	/// <summary>
	/// Reads the next byte from the underlying byte stream and advances the current position by one byte.
	/// </summary>
	protected abstract byte InternalReadByte();

	/// <summary>
	/// Returns a read-only span over the specified number of bytes from the underlying byte stream and advances the current position by that number of bytes.
	/// </summary>
	/// <param name="count">The size of the read-only span to return.</param>
	protected abstract ReadOnlySpan<byte> InternalReadSpan(int count);
}
