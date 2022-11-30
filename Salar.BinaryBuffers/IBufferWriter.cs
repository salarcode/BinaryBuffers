using System;

namespace Salar.BinaryBuffers;

/// <summary>
/// Provides a writer for writing primitive data types to a byte array.
/// </summary>
public interface IBufferWriter
{
	/// <summary>
	/// Gets the offset into the underlying byte array to start writing from.
	/// </summary>
	int Offset { get; }

	/// <summary>
	/// Gets the effective length of the writable region of the underlying byte array.
	/// </summary>
	int Length { get; }

	/// <summary>
	/// Gets or sets the current writing position within the underlying byte array.
	/// </summary>
	int Position { get; set; }

	/// <summary>
	/// Writes a boolean value to the underlying byte array and advances the current position by one byte.
	/// </summary>
	/// <param name="value">The boolean value to write.</param>
	void Write(bool value);

	/// <summary>
	/// Writes a byte to the underlying byte array and advances the current position by one byte.
	/// </summary>
	/// <param name="value">The byte value to write.</param>
	void Write(byte value);

	/// <summary>
	/// Writes a signed byte to the underlying byte array and advances the current position by one byte.
	/// </summary>
	/// <param name="value">The signed byte value to write.</param>
	void Write(sbyte value);

	/// <summary>
	/// Copies the contents of a byte array to the underlying byte array of the writer and advances the current position by the number of bytes written.
	/// </summary>
	/// <param name="buffer">The buffer to copy data from.</param>
	void Write(byte[] buffer);

	/// <summary>
	/// Copies a region of a byte array to the underlying byte array of the writer and advances the current position by the number of bytes written.
	/// </summary>
	/// <param name="buffer">The buffer to copy data from.</param>
	/// <param name="offset">The 0-based offset in buffer at which to start copying from.</param>
	/// <param name="length">The number of bytes to copy.</param>
	void Write(byte[] buffer, int offset, int length);

	/// <summary>
	/// Writes a decimal value to the underlying byte array and advances the current position by sixteen bytes.
	/// </summary>
	/// <param name="value">The decimal value to write.</param>
	void Write(decimal value);

	/// <summary>
	/// Writes a double-precision floating-point number to the underlying byte array and advances the current position by eight bytes.
	/// </summary>
	/// <param name="value">The double-precision floating-point number to write.</param>
	void Write(double value);

	/// <summary>
	/// Writes a single-precision floating-point number to the underlying byte array and advances the current position by one byte.
	/// </summary>
	/// <param name="value">The single-precision floating-point number to write.</param>
	void Write(float value);

	/// <summary>
	/// Writes a 16-bit signed integer to the underlying byte array and advances the current position by two bytes.
	/// </summary>
	/// <param name="value">The 16-bit signed integer to write.</param>
	void Write(short value);

	/// <summary>
	/// Writes a 16-bit unsigned integer to the underlying byte array and advances the current position by two bytes.
	/// </summary>
	/// <param name="value">The 16-bit unsigned integer to write.</param>
	void Write(ushort value);

	/// <summary>
	/// Writes a 32-bit signed integer to the underlying byte array and advances the current position by four bytes.
	/// </summary>
	/// <param name="value">The 32-bit signed integer to write.</param>
	void Write(int value);

	/// <summary>
	/// Writes a 32-bit unsigned integer to the underlying byte array and advances the current position by four bytes.
	/// </summary>
	/// <param name="value">The 32-bit unsigned integer to write.</param>
	void Write(uint value);

	/// <summary>
	/// Writes a 64-bit signed integer to the underlying byte array and advances the current position by eight bytes.
	/// </summary>
	/// <param name="value">The 64-bit signed integer to write.</param>
	void Write(long value);
	/// <summary>
	/// Writes a 64-bit unsigned integer value to the underlying byte array and advances the current position by eight bytes.
	/// </summary>
	/// <param name="value">The 64-bit unsigned integer to write.</param>
	void Write(ulong value);

	/// <summary>
	/// Copies a span of bytes to the underlying byte array and advances the current position by the number of bytes written.
	/// </summary>
	/// <param name="buffer">The span of bytes to write.</param>
	void Write(ReadOnlySpan<byte> buffer);
}
