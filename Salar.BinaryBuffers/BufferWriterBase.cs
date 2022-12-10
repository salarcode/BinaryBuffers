using System;

namespace Salar.BinaryBuffers;

/// <inheritdoc/>
public abstract class BufferWriterBase : IBufferWriter
{
	/// <inheritdoc/>
	public abstract int Offset { get; }

	/// <inheritdoc/>
	public abstract int Length { get; }

	/// <inheritdoc/>
	public abstract int Position { get; set; }


	/// <inheritdoc/>
	public abstract void ResetBuffer();

	/// <inheritdoc/>
	public abstract void Write(bool value);

	/// <inheritdoc/>
	public abstract void Write(byte value);

	/// <inheritdoc/>
	public abstract void Write(sbyte value);

	/// <inheritdoc/>
	public abstract void Write(byte[] buffer);

	/// <inheritdoc/>
	public abstract void Write(byte[] buffer, int offset, int length);

	/// <inheritdoc/>
	public abstract void Write(decimal value);

	/// <inheritdoc/>
	public abstract void Write(double value);

	/// <inheritdoc/>
	public abstract void Write(float value);

	/// <inheritdoc/>
	public abstract void Write(short value);

	/// <inheritdoc/>
	public abstract void Write(ushort value);

	/// <inheritdoc/>
	public abstract void Write(int value);

	/// <inheritdoc/>
	public abstract void Write(uint value);

	/// <inheritdoc/>
	public abstract void Write(long value);

	/// <inheritdoc/>
	public abstract void Write(ulong value);

	/// <inheritdoc/>
	public abstract void Write(ReadOnlySpan<byte> buffer);
}
