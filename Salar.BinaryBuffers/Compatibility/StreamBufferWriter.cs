using System;
using System.Buffers.Binary;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
#if NET6_0_OR_GREATER
using System.Runtime.InteropServices;
#endif
namespace Salar.BinaryBuffers.Compatibility;

/// <summary>
/// Provides a writer for writing primitive data types to a stream of bytes.
/// </summary>
public class StreamBufferWriter : IBufferWriter
{
	private readonly Stream _stream;

	/// <inheritdoc/>
	public int Offset => 0;

	/// <inheritdoc/>
	public int Length => (int)_stream.Length;

	/// <inheritdoc/>
	public int Position { get => (int)_stream.Position; set => _stream.Position = value; }

	public StreamBufferWriter(Stream stream)
	{
		_stream = stream;
	}

	/// <inheritdoc/>
	public void Write(bool value)
	{
		_stream.WriteByte(value ? (byte)1 : (byte)0);
	}

	/// <inheritdoc/>
	public void Write(byte value)
	{
		_stream.WriteByte(value);
	}

	/// <inheritdoc/>
	public void Write(sbyte value)
	{
		_stream.WriteByte((byte)value);
	}

	/// <inheritdoc/>
	public void Write(byte[] buffer)
	{
		_stream.Write(buffer, 0, buffer.Length);
	}

	/// <inheritdoc/>
	public void Write(byte[] buffer, int offset, int length)
	{
		_stream.Write(buffer, offset, length);
	}

	/// <inheritdoc/>
	public void Write(decimal value)
	{
#if NET6_0_OR_GREATER
		Span<byte> buffer = stackalloc byte[16];

		decimal.GetBits(value, MemoryMarshal.Cast<byte, int>(buffer));
		_stream.Write(buffer);
#else
		var bits = decimal.GetBits(value);
		for (byte i = 0; i < bits.Length; i++)
		{
			var bytes = BitConverter.GetBytes(bits[i]);
			_stream.Write(bytes, 0, bytes.Length);
		}
#endif
	}

	/// <inheritdoc/>
	public void Write(double value)
	{
#if NET6_0_OR_GREATER
		Span<byte> buffer = stackalloc byte[8];
		Unsafe.WriteUnaligned<double>(ref MemoryMarshal.GetReference<byte>(buffer), value);

		_stream.Write(buffer);
#else
		var bitsArray = BitConverter.GetBytes(value);
		_stream.Write(bitsArray, 0, bitsArray.Length);
#endif
	}

	/// <inheritdoc/>
	public void Write(float value)
	{
#if NET6_0_OR_GREATER
		Span<byte> buffer = stackalloc byte[4];
		Unsafe.WriteUnaligned<float>(ref MemoryMarshal.GetReference<byte>(buffer), value);

		_stream.Write(buffer);
#else
		var bitsArray = BitConverter.GetBytes(value);
		_stream.Write(bitsArray, 0, bitsArray.Length);
#endif
	}

	/// <inheritdoc/>
	public void Write(short value)
	{
#if NET6_0_OR_GREATER
		Span<byte> buffer = stackalloc byte[2];
		Unsafe.WriteUnaligned<short>(ref MemoryMarshal.GetReference<byte>(buffer), value);

		_stream.Write(buffer);
#else
		var bitsArray = BitConverter.GetBytes(value);
		_stream.Write(bitsArray, 0, bitsArray.Length);
#endif
	}

	/// <inheritdoc/>
	public void Write(ushort value)
	{
#if NET6_0_OR_GREATER
		Span<byte> buffer = stackalloc byte[2];
		Unsafe.WriteUnaligned<ushort>(ref MemoryMarshal.GetReference<byte>(buffer), value);

		_stream.Write(buffer);
#else
		var bitsArray = BitConverter.GetBytes(value);
		_stream.Write(bitsArray, 0, bitsArray.Length);
#endif
	}

	/// <inheritdoc/>
	public void Write(int value)
	{
#if NET6_0_OR_GREATER
		Span<byte> buffer = stackalloc byte[4];
		Unsafe.WriteUnaligned<int>(ref MemoryMarshal.GetReference<byte>(buffer), value);

		_stream.Write(buffer);
#else
		var bitsArray = BitConverter.GetBytes(value);
		_stream.Write(bitsArray, 0, bitsArray.Length);
#endif
	}

	/// <inheritdoc/>
	public void Write(uint value)
	{
#if NET6_0_OR_GREATER
		Span<byte> buffer = stackalloc byte[4];
		Unsafe.WriteUnaligned<uint>(ref MemoryMarshal.GetReference<byte>(buffer), value);

		_stream.Write(buffer);
#else
		var bitsArray = BitConverter.GetBytes(value);
		_stream.Write(bitsArray, 0, bitsArray.Length);
#endif
	}

	/// <inheritdoc/>
	public void Write(long value)
	{
#if NET6_0_OR_GREATER
		Span<byte> buffer = stackalloc byte[8];
		Unsafe.WriteUnaligned<long>(ref MemoryMarshal.GetReference<byte>(buffer), value);

		_stream.Write(buffer);
#else
		var bitsArray = BitConverter.GetBytes(value);
		_stream.Write(bitsArray, 0, bitsArray.Length);
#endif
	}

	/// <inheritdoc/>
	public void Write(ulong value)
	{
#if NET6_0_OR_GREATER
		Span<byte> buffer = stackalloc byte[8];
		Unsafe.WriteUnaligned<ulong>(ref MemoryMarshal.GetReference<byte>(buffer), value);

		_stream.Write(buffer);
#else
		var bitsArray = BitConverter.GetBytes(value);
		_stream.Write(bitsArray, 0, bitsArray.Length);
#endif
	}

	/// <inheritdoc/>
	public void Write(ReadOnlySpan<byte> buffer)
	{
#if NET6_0_OR_GREATER
		_stream.Write(buffer);
#else
		var arr = buffer.ToArray();
		_stream.Write(arr, 0, arr.Length);
#endif
	}
}
