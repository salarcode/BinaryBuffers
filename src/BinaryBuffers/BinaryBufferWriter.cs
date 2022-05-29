namespace BinaryBuffers
{
    using System;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Provides a writer for writing primitive data types to a byte array.
    /// </summary>
    public class BinaryBufferWriter
    {
        private readonly byte[] _buffer;
        private int _position;
        private int _relativePositon;

        /// <summary>
        /// Gets the offset into the underlying byte array to start writing from.
        /// </summary>
        public int Offset { get; }

        /// <summary>
        /// Gets the effective length of the writable region of the underlying byte array.
        /// </summary>
        public int Length { get; }

        /// <summary>
        /// Gets or sets the current writing position within the underlying byte array.
        /// </summary>
        public int Position
        {
            get => _relativePositon;
            set
            {
                var newPosition = Offset + value;

                if (value < 0) throw ExceptionHelper.PositionLessThanZeroException(nameof(value));
                if (value > Length) throw ExceptionHelper.PositionGreaterThanLengthOfByteArrayException(nameof(value));

                _relativePositon = value;
                _position = newPosition;
            }
        }

        /// <summary>
        /// Gets the total number of bytes written to the underlying byte array.
        /// </summary>
        public int WrittenLength { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="BinaryBufferWriter"/> class using the specified byte array to write the output to.
        /// </summary>
        /// <param name="buffer">The byte array to write to.</param>
        public BinaryBufferWriter(byte[] buffer)
        {
            _buffer = buffer ?? throw new ArgumentNullException(nameof(buffer));
            _position = 0;
            _relativePositon = 0;
            Offset = 0;
            Length = buffer.Length;
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
            _buffer = buffer ?? throw new ArgumentNullException(nameof(buffer));

            if (offset < 0) throw ExceptionHelper.OffsetLessThanZeroException(nameof(offset));
            if (length < 0) throw ExceptionHelper.LengthLessThanZeroException(nameof(length));
            if (length > _buffer.Length - offset) throw ExceptionHelper.LengthGreaterThanEffectiveLengthOfByteArrayException();

            _position = offset;
            _relativePositon = 0;
            Offset = offset;
            Length = length;
        }

        /// <summary>
        /// Writes a boolean value to the underlying byte array and advances the current position by one byte.
        /// </summary>
        /// <param name="value">The boolean value to write.</param>
        public virtual void Write(bool value)
        {
            var pos = _position;
            Advance(1);

            _buffer[pos + 0] = (byte)(value ? 1 : 0);
        }

        /// <summary>
        /// Writes a byte to the underlying byte array and advances the current position by one byte.
        /// </summary>
        /// <param name="value">The byte value to write.</param>
        public virtual void Write(byte value)
        {
            var pos = _position;
            Advance(1);

            _buffer[pos + 0] = value;
        }

        /// <summary>
        /// Copies the contents of a byte array to the underlying byte array of the writer and advances the current position by the number of bytes written.
        /// </summary>
        /// <param name="buffer">The buffer to copy data from.</param>
        public virtual void Write(byte[] buffer)
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
        public virtual void Write(byte[] buffer, int offset, int length)
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
        public virtual void Write(decimal value)
        {
            var bits = decimal.GetBits(value);
            var pos = _position;
            Advance(16);

            Write(bits[0], pos);
            Write(bits[1], pos + 4);
            Write(bits[2], pos + 4 + 4);
            Write(bits[3], pos + 4 + 4 + 4);
        }

        /// <summary>
        /// Writes a double-precision floating-point number to the underlying byte array and advances the current position by eight bytes.
        /// </summary>
        /// <param name="value">The double-precision floating-point number to write.</param>
        public virtual unsafe void Write(double value)
        {
            var pos = _position;
            var buff = _buffer;
            Advance(8);

            ulong tmpValue = *(ulong*)&value;
            buff[pos + 0] = (byte)tmpValue;
            buff[pos + 1] = (byte)(tmpValue >> 8);
            buff[pos + 2] = (byte)(tmpValue >> 16);
            buff[pos + 3] = (byte)(tmpValue >> 24);
            buff[pos + 4] = (byte)(tmpValue >> 32);
            buff[pos + 5] = (byte)(tmpValue >> 40);
            buff[pos + 6] = (byte)(tmpValue >> 48);
            buff[pos + 7] = (byte)(tmpValue >> 56);
        }

        /// <summary>
        /// Writes a 16-bit signed integer to the underlying byte array and advances the current position by two bytes.
        /// </summary>
        /// <param name="value">The 16-bit signed integer to write.</param>
        public virtual void Write(short value)
        {
            var pos = _position;
            Advance(2);

            _buffer[pos + 0] = (byte)value;
            _buffer[pos + 1] = (byte)(value >> 8);
        }

        /// <summary>
        /// Writes a 32-bit signed integer to the underlying byte array and advances the current position by four bytes.
        /// </summary>
        /// <param name="value">The 32-bit signed integer to write.</param>
        public virtual void Write(int value)
        {
            var pos = _position;
            Advance(4);

            _buffer[pos + 0] = (byte)value;
            _buffer[pos + 1] = (byte)(value >> 8);
            _buffer[pos + 2] = (byte)(value >> 16);
            _buffer[pos + 3] = (byte)(value >> 24);
        }

        /// <summary>
        /// Writes a 64-bit signed integer to the underlying byte array and advances the current position by eight bytes.
        /// </summary>
        /// <param name="value">The 64-bit signed integer to write.</param>
        public virtual void Write(long value)
        {
            var pos = _position;
            var buff = _buffer;
            Advance(8);

            buff[pos + 0] = (byte)value;
            buff[pos + 1] = (byte)(value >> 8);
            buff[pos + 2] = (byte)(value >> 16);
            buff[pos + 3] = (byte)(value >> 24);
            buff[pos + 4] = (byte)(value >> 32);
            buff[pos + 5] = (byte)(value >> 40);
            buff[pos + 6] = (byte)(value >> 48);
            buff[pos + 7] = (byte)(value >> 56);
        }

        /// <summary>
        /// Writes a signed byte to the underlying byte array and advances the current position by one byte.
        /// </summary>
        /// <param name="value">The signed byte value to write.</param>
        public virtual void Write(sbyte value)
        {
            var pos = _position;
            Advance(1);

            _buffer[pos + 0] = (byte)value;
        }

        /// <summary>
        /// Writes a single-precision floating-point number to the underlying byte array and advances the current position by one byte.
        /// </summary>
        /// <param name="value">The single-precision floating-point number to write.</param>
        public virtual unsafe void Write(float value)
        {
            var pos = _position;
            Advance(4);

            uint tmpValue = *(uint*)&value;
            _buffer[pos + 0] = (byte)tmpValue;
            _buffer[pos + 1] = (byte)(tmpValue >> 8);
            _buffer[pos + 2] = (byte)(tmpValue >> 16);
            _buffer[pos + 3] = (byte)(tmpValue >> 24);
        }

        /// <summary>
        /// Copies a span of bytes to the underlying byte array and advances the current position by the number of bytes written.
        /// </summary>
        /// <param name="buffer">The span of bytes to write.</param>
        public virtual void Write(in ReadOnlySpan<byte> buffer)
        {
            var pos = _position;
            var length = buffer.Length;
            Advance(length);

            buffer.CopyTo(pos == 0 ? _buffer : new Span<byte>(_buffer, pos, length));
        }

        /// <summary>
        /// Writes a 16-bit unsigned integer to the underlying byte array and advances the current position by two bytes.
        /// </summary>
        /// <param name="value">The 16-bit unsigned integer to write.</param>
        public virtual void Write(ushort value)
        {
            var pos = _position;
            Advance(2);

            _buffer[pos + 0] = (byte)value;
            _buffer[pos + 1] = (byte)(value >> 8);
        }

        /// <summary>
        /// Writes a 32-bit unsigned integer to the underlying byte array and advances the current position by four bytes.
        /// </summary>
        /// <param name="value">The 32-bit unsigned integer to write.</param>
        public virtual void Write(uint value)
        {
            var pos = _position;
            Advance(4);

            _buffer[pos + 0] = (byte)value;
            _buffer[pos + 1] = (byte)(value >> 8);
            _buffer[pos + 2] = (byte)(value >> 16);
            _buffer[pos + 3] = (byte)(value >> 24);
        }

        /// <summary>
        /// Writes a 64-bit unsigned integer value to the underlying byte array and advances the current position by eight bytes.
        /// </summary>
        /// <param name="value">The 64-bit unsigned integer to write.</param>
        public virtual void Write(ulong value)
        {
            var pos = _position;
            var buff = _buffer;
            Advance(8);

            buff[pos + 0] = (byte)value;
            buff[pos + 1] = (byte)(value >> 8);
            buff[pos + 2] = (byte)(value >> 16);
            buff[pos + 3] = (byte)(value >> 24);
            buff[pos + 4] = (byte)(value >> 32);
            buff[pos + 5] = (byte)(value >> 40);
            buff[pos + 6] = (byte)(value >> 48);
            buff[pos + 7] = (byte)(value >> 56);
        }

        /// <summary>
        /// Creates a span over the underlying byte array of the writer.
        /// </summary>
        public ReadOnlySpan<byte> ToReadOnlySpan() => new ReadOnlySpan<byte>(_buffer, Offset, WrittenLength);

        /// <summary>
        /// Returns the underlying byte array of the writer.
        /// </summary>
        public byte[] ToArray() => ToReadOnlySpan().ToArray();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Write(int value, int pos)
        {
            _buffer[pos + 0] = (byte)value;
            _buffer[pos + 1] = (byte)(value >> 8);
            _buffer[pos + 2] = (byte)(value >> 16);
            _buffer[pos + 3] = (byte)(value >> 24);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Advance(int count)
        {
            var newPos = _position + count;
            int relPos = _relativePositon + count;

            if ((uint)relPos > (uint)Length)
            {
                _relativePositon = Length;
                throw ExceptionHelper.EndOfDataException();
            }

            _relativePositon = relPos;
            _position = newPos;

            if (count > 0) WrittenLength = Math.Max(_relativePositon, WrittenLength);
        }
    }
}
