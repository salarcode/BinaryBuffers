using System;
using System.IO;
using System.Runtime.CompilerServices;
#if NET6_0_OR_GREATER
using System.Runtime.InteropServices;
#endif
namespace Salar.BinaryBuffers.Compatibility;

/// <summary>
/// Provides a writer for writing primitive data types to a stream of bytes.
/// </summary>
public class StreamBufferWriter : BufferWriterBase
{
	private readonly Stream _stream;

	/// <inheritdoc/>
	public override int Offset => 0;

	/// <inheritdoc/>
	public override int Length => (int)_stream.Length;

	/// <inheritdoc/>
	public override int Position { get => (int)_stream.Position; set => _stream.Position = value; }

	public StreamBufferWriter(Stream stream)
	{
		_stream = stream;
	}

	/// <summary>
	/// Reset the status of what is written; sepends on the underlying stream might clean itself.
	/// </summary>
	public override void ResetBuffer()
	{
		_stream.SetLength(0);
	}

	/// <inheritdoc/>
	public override void Write(bool value)
	{
		_stream.WriteByte(value ? (byte)1 : (byte)0);
	}

	/// <inheritdoc/>
	public override void Write(byte value)
	{
		_stream.WriteByte(value);
	}

	/// <inheritdoc/>
	public override void Write(sbyte value)
	{
		_stream.WriteByte((byte)value);
	}

	/// <inheritdoc/>
	public override void Write(byte[] buffer)
	{
		_stream.Write(buffer, 0, buffer.Length);
	}

	/// <inheritdoc/>
	public override void Write(byte[] buffer, int offset, int length)
	{
		_stream.Write(buffer, offset, length);
	}

	/// <inheritdoc/>
	public override void Write(decimal value)
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
	public override void Write(double value)
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
	public override void Write(float value)
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
	public override void Write(short value)
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
	public override void Write(ushort value)
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
	public override void Write(int value)
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
	public override void Write(uint value)
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
	public override void Write(long value)
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
	public override void Write(ulong value)
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
	public override void Write(ReadOnlySpan<byte> buffer)
	{
#if NET6_0_OR_GREATER
		_stream.Write(buffer);
#else
		var arr = buffer.ToArray();
		_stream.Write(arr, 0, arr.Length);
#endif
	}
}
