namespace BinaryBuffers
{
    using System;
    using System.Buffers.Binary;

    /// <summary>
    /// Implements an <see cref="IBufferReader"/> that can read primitive data types from a <see cref="byte"/>-based <see cref="ReadOnlyMemory{T}"/>.
    /// </summary>
    public class BinaryBufferMemoryReader : IBufferReader
    {
        private readonly ReadOnlyMemory<byte> _data;
        private int _position;

        /// <summary>
        /// Gets the offset into the underlying <see cref="ReadOnlyMemory{T}"/> to start reading from.
        /// </summary>
        public int Offset { get; }

        /// <summary>
        /// Gets the effective length of the readable region of the underlying <see cref="ReadOnlyMemory{T}"/>.
        /// </summary>
        public int Length { get; }

        /// <summary>
        /// Gets or sets the current reading position within the underlying <see cref="ReadOnlyMemory{T}"/>.
        /// </summary>
        public int Position
        {
            get => _position;
            set
            {
                var newPosition = _position + value;

                if (newPosition < 0) throw ExceptionHelper.PositionLessThanZeroException(nameof(value));
                if (newPosition > Length) throw ExceptionHelper.PositionGreaterThanLengthOfReadOnlyMemoryException(nameof(value));

                _position = newPosition;
            }
        }


        /// <summary>
        /// Initializes a new instance of <see cref="BinaryBufferReader"/> based on the specified <see cref="ReadOnlyMemory{T}"/>.
        /// </summary>
        /// <param name="data">The input <see cref="ReadOnlyMemory{T}"/>.</param>
        /// 
        public BinaryBufferMemoryReader(in ReadOnlyMemory<byte> data)
        {
            _data = data;
            _position = 0;
            Offset = 0;
            Length = data.Length;
        }

        /// <summary>
        /// Reads a boolean value from the underlying <see cref="ReadOnlyMemory{T}"/> and advances the current position by one byte.
        /// </summary>
        public virtual bool ReadBoolean() => InternalReadByte() != 0;

        /// <summary>
        /// Reads the next byte from the underlying <see cref="ReadOnlyMemory{T}"/> and advances the current position by one byte.
        /// </summary>
        public virtual byte ReadByte() => InternalReadByte();

        /// <summary>
        /// Reads the specified number of bytes from the underlying <see cref="ReadOnlyMemory{T}"/> into a new byte array and advances the current position by that number of bytes.
        /// </summary>
        /// <param name="count">The number of bytes to read.</param>
        public virtual byte[] ReadBytes(int count) => InternalReadSpan(count).ToArray();

        /// <summary>
        /// Reads a decimal value from the underlying <see cref="ReadOnlyMemory{T}"/> and advances the current position by sixteen bytes.
        /// </summary>
        public virtual decimal ReadDecimal()
        {
            var buffer = InternalReadSpan(16);
            try
            {
                return new decimal(
                    new[]
                    {
                        BinaryPrimitives.ReadInt32LittleEndian(buffer), // lo
                        BinaryPrimitives.ReadInt32LittleEndian(buffer.Slice(4)), // mid
                        BinaryPrimitives.ReadInt32LittleEndian(buffer.Slice(8)), // hi
                        BinaryPrimitives.ReadInt32LittleEndian(buffer.Slice(12)) // flags
                    });
            }
            catch (ArgumentException e)
            {
                // ReadDecimal cannot leak out ArgumentException
                throw ExceptionHelper.DecimalReadingException(e);
            }
        }

        /// <summary>
        /// Reads a double-precision floating-point number from the underlying <see cref="ReadOnlyMemory{T}"/> and advances the current position by eight bytes.
        /// </summary>
        public virtual double ReadDouble() => BitConverter.Int64BitsToDouble(BinaryPrimitives.ReadInt64LittleEndian(InternalReadSpan(8)));

        /// <summary>
        /// Reads a 16-bit signed integer from the underlying <see cref="ReadOnlyMemory{T}"/> and advances the current position by two bytes.
        /// </summary>
        public virtual short ReadInt16() => BinaryPrimitives.ReadInt16LittleEndian(InternalReadSpan(2));

        /// <summary>
        /// Reads a 32-bit signed integer from the underlying <see cref="ReadOnlyMemory{T}"/> and advances the current position by four bytes.
        /// </summary>
        public virtual int ReadInt32() => BinaryPrimitives.ReadInt32LittleEndian(InternalReadSpan(4));

        /// <summary>
        /// Reads a 64-bit signed integer signed integer from the underlying <see cref="ReadOnlyMemory{T}"/> and advances the current position by eight bytes.
        /// </summary>
        public virtual long ReadInt64() => BinaryPrimitives.ReadInt64LittleEndian(InternalReadSpan(8));

        /// <summary>
        /// Reads a signed byte from the underlying <see cref="ReadOnlyMemory{T}"/> and advances the current position by one byte.
        /// </summary>
        public virtual sbyte ReadSByte() => (sbyte) InternalReadByte();

        /// <summary>
        /// Reads a single-precision floating-point number from the underlying <see cref="ReadOnlyMemory{T}"/> and advances the current position by four bytes.
        /// </summary>
#if NETSTANDARD2_0
        public virtual unsafe float ReadSingle()
        {
            var m_buffer = InternalReadSpan(4);
            uint tmpBuffer = (uint)(m_buffer[0] | m_buffer[1] << 8 | m_buffer[2] << 16 | m_buffer[3] << 24);
            return *((float*)&tmpBuffer);
        }
#else
        public virtual float ReadSingle() => BitConverter.Int32BitsToSingle(BinaryPrimitives.ReadInt32LittleEndian(InternalReadSpan(4)));
#endif

        /// <summary>
        /// Reads a span of bytes from the underlying <see cref="ReadOnlyMemory{T}"/> and advances the current position by the number of bytes read.
        /// </summary>
        /// <param name="count">The number of bytes to read.</param>
        public virtual ReadOnlySpan<byte> ReadSpan(int count) => InternalReadSpan(count);

        /// <summary>
        /// Reads a 16-bit unsigned integer from the underlying <see cref="ReadOnlyMemory{T}"/> and advances the current position by two bytes.
        /// </summary>
        public virtual ushort ReadUInt16() => BinaryPrimitives.ReadUInt16LittleEndian(InternalReadSpan(2));

        /// <summary>
        /// Reads a 32-bit unsigned integer from the underlying <see cref="ReadOnlyMemory{T}"/> and advances the current position by four bytes.
        /// </summary>
        public virtual uint ReadUInt32() => BinaryPrimitives.ReadUInt32LittleEndian(InternalReadSpan(4));

        /// <summary>
        /// Reads a 64-bit unsigned integer from the underlying <see cref="ReadOnlyMemory{T}"/> and advances the current position by eight bytes.
        /// </summary>
        public virtual ulong ReadUInt64() => BinaryPrimitives.ReadUInt64LittleEndian(InternalReadSpan(8));

        /// <summary>
        /// Reads the next byte from the underlying <see cref="ReadOnlyMemory{T}"/> and advances the current position by one byte.
        /// </summary>
        protected byte InternalReadByte()
        {
            int curPos = _position;
            int newPos = curPos + 1;

            if ((uint)newPos > (uint)Length)
            {
                _position = Length;
                throw ExceptionHelper.EndOfDataException();
            }

            _position = newPos;

            return _data.Span[curPos];
        }

        /// <summary>
        /// Returns a read-only span over the specified number of bytes from the underlying <see cref="ReadOnlyMemory{T}"/> and advances the current position by that number of bytes.
        /// </summary>
        /// <param name="count">The size of the read-only span to return.</param>
        protected ReadOnlySpan<byte> InternalReadSpan(int count)
        {
            if (count <= 0) return ReadOnlySpan<byte>.Empty;

            int curPos = _position;
            int newPos = curPos + count;

            if ((uint)newPos > (uint) Length)
            {
                _position = Length;
                throw ExceptionHelper.EndOfDataException();
            }

            _position = newPos;

            return _data.Slice(curPos, count).Span;
        }
    }
}
