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
	public bool ReadBoolean() => InternalReadByte() != 0;

	/// <inheritdoc/>
	public byte ReadByte() => InternalReadByte();

	/// <inheritdoc/>
	public abstract byte[] ReadBytes(int count);

	/// <inheritdoc/>
	public abstract int Read(byte[] buffer, int index, int count);

	/// <inheritdoc/>
	public abstract ReadOnlySpan<byte> ReadSpan(int count);

	public unsafe decimal ReadDecimal()
	{
		var span = InternalReadSpan(16);
		try
		{
			return new decimal(
#if NET6_0_OR_GREATER
			stackalloc
#else
			new
#endif
			[]
			{
				BinaryPrimitives.ReadInt32LittleEndian(span),          // lo
				BinaryPrimitives.ReadInt32LittleEndian(span.Slice(4)), // mid
				BinaryPrimitives.ReadInt32LittleEndian(span.Slice(8)), // hi
				BinaryPrimitives.ReadInt32LittleEndian(span.Slice(12)) // flags
			});
		}
		catch (ArgumentException e)
		{
			// ReadDecimal cannot leak out ArgumentException
			throw ExceptionHelper.DecimalReadingException(e);
		}
	}

	/// <inheritdoc/>
	public double ReadDouble()
	{
		var span = InternalReadSpan(8);
		return Unsafe.ReadUnaligned<double>(ref MemoryMarshal.GetReference<byte>(span));
	}

	/// <inheritdoc/>
	public short ReadInt16()
	{
		var span = InternalReadSpan(2);
		return Unsafe.ReadUnaligned<short>(ref MemoryMarshal.GetReference<byte>(span));
	}

	/// <inheritdoc/>
	public int ReadInt32()
	{
		var span = InternalReadSpan(4);
		return Unsafe.ReadUnaligned<int>(ref MemoryMarshal.GetReference<byte>(span));
	}

	/// <inheritdoc/>
	public long ReadInt64()
	{
		var span = InternalReadSpan(8);
		return Unsafe.ReadUnaligned<long>(ref MemoryMarshal.GetReference<byte>(span));
	}

	/// <inheritdoc/>
	public sbyte ReadSByte() => (sbyte)InternalReadByte();

	/// <inheritdoc/>
#if NETSTANDARD2_0
	public unsafe float ReadSingle()
	{
		var m_buffer = InternalReadSpan(4);
		uint tmpBuffer = (uint)(m_buffer[0] | m_buffer[1] << 8 | m_buffer[2] << 16 | m_buffer[3] << 24);

		return *((float*)&tmpBuffer);
	}
#else
	public float ReadSingle()
	{
		var span = InternalReadSpan(4);
		return Unsafe.ReadUnaligned<float>(ref MemoryMarshal.GetReference<byte>(span)); 
	}
#endif

	/// <inheritdoc/>
	public ushort ReadUInt16()
	{
		var span = InternalReadSpan(2);
		return Unsafe.ReadUnaligned<ushort>(ref MemoryMarshal.GetReference<byte>(span));
	}

	/// <inheritdoc/>
	public uint ReadUInt32()
	{
		var span = InternalReadSpan(4);
		return Unsafe.ReadUnaligned<uint>(ref MemoryMarshal.GetReference<byte>(span));
	}

	/// <inheritdoc/>
	public ulong ReadUInt64()
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
