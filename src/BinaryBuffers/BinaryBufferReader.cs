namespace BinaryBuffers
{
    using System;
    using System.Buffers.Binary;

    /// <summary>
    /// Implements an <see cref="IBufferReader"/> that can read primitive data types from a byte array.
    /// </summary>
    public class BinaryBufferReader : IBufferReader
    {
        private readonly byte[] _data;
        private int _relativePositon;
        private int _position;

        /// <summary>
        /// Gets the offset into the underlying byte array to start reading from.
        /// </summary>
        public int Offset { get; }

        /// <summary>
        /// Gets the effective length of the readable region of the underlying byte array.
        /// </summary>
        public int Length { get; }

        /// <summary>
        /// Gets or sets the current reading position within the underlying byte array.
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
        /// Initializes a new instance of the <see cref="BinaryBufferReader"/> class based on the specified byte array.
        /// </summary>
        /// <param name="data">The byte array to read from.</param>
        public BinaryBufferReader(byte[] data)
        {
            _data = data ?? throw new ArgumentNullException(nameof(data));
            _position = 0;
            _relativePositon = 0;
            Offset = 0;
            Length = data.Length;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BinaryBufferReader"/> class based on the specified byte array.
        /// <para>A provided offset and length specifies the boundaries to use for reading.</para>
        /// </summary>
        /// <param name="data">The byte array to read from.</param>
        /// <param name="offset">The 0-based offset into the byte array at which to begin reading from.
        /// <para>Cannot exceed the bounds of the byte array.</para></param>
        /// <param name="length">The maximum number of bytes that the reader will use for reading, relative to the offset position.
        /// <para>Cannot exceed the bounds of the byte array.</para></param>
        public BinaryBufferReader(byte[] data, int offset, int length)
        {
            _data = data ?? throw new ArgumentNullException(nameof(data));

            if (offset < 0) throw ExceptionHelper.OffsetLessThanZeroException(nameof(offset));
            if (length < 0) throw ExceptionHelper.LengthLessThanZeroException(nameof(length));
            if (length > _data.Length - offset) throw ExceptionHelper.LengthGreaterThanEffectiveLengthOfByteArrayException();

            _position = offset;
            _relativePositon = 0;
            Offset = offset;
            Length = length;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BinaryBufferReader"/> class based on the specified byte array segment.
        /// </summary>
        /// <param name="data">The byte array segment to read from.</param>
        public BinaryBufferReader(in ArraySegment<byte> data)
        {
            _data = data.Array ?? throw new ArgumentNullException(nameof(data));
            _position = data.Offset;
            _relativePositon = 0;
            Offset = data.Offset;
            Length = data.Count;
        }

        

        /// <summary>
        /// Reads a boolean value from the underlying byte array and advances the current position by one byte.
        /// </summary>
        public virtual bool ReadBoolean() => InternalReadByte() != 0;

        /// <summary>
        /// Reads the next byte from the underlying byte array and advances the current position by one byte.
        /// </summary>
        public virtual byte ReadByte() => InternalReadByte();

        /// <summary>
        /// Reads the specified number of bytes from the underlying byte array into a new byte array and advances the current position by that number of bytes.
        /// </summary>
        /// <param name="count">The number of bytes to read.</param>
        public virtual byte[] ReadBytes(int count) => InternalReadSpan(count).ToArray();

        /// <summary>
        /// Reads a decimal value from the underlying byte array and advances the current position by sixteen bytes.
        /// </summary>
        public virtual decimal ReadDecimal()
        {
            var buffer = InternalReadSpan(16);
            try
            {
                return new decimal(new[]
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
        /// Reads a double-precision floating-point number from the underlying byte array and advances the current position by eight bytes.
        /// </summary>
        public virtual double ReadDouble() => BitConverter.Int64BitsToDouble(BinaryPrimitives.ReadInt64LittleEndian(InternalReadSpan(8)));

        /// <summary>
        /// Reads a 16-bit signed integer from the underlying byte array and advances the current position by two bytes.
        /// </summary>
        public virtual short ReadInt16() => BinaryPrimitives.ReadInt16LittleEndian(InternalReadSpan(2));

        /// <summary>
        /// Reads a 32-bit signed integer from the underlying byte array and advances the current position by four bytes.
        /// </summary>
        public virtual int ReadInt32() => BinaryPrimitives.ReadInt32LittleEndian(InternalReadSpan(4));

        /// <summary>
        /// Reads a 64-bit signed integer from the underlying byte array and advances the current position by eight bytes.
        /// </summary>
        public virtual long ReadInt64() => BinaryPrimitives.ReadInt64LittleEndian(InternalReadSpan(8));

        /// <summary>
        /// Reads a signed byte from the underlying byte array and advances the current position by one byte.
        /// </summary>
        public virtual sbyte ReadSByte() => (sbyte)InternalReadByte();

        /// <summary>
        /// Reads a single-precision floating-point number from the underlying byte array and advances the current position by four bytes.
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
        /// Reads a span of bytes from the underlying byte array and advances the current position by the number of bytes read.
        /// </summary>
        /// <param name="count">The number of bytes to read.</param>
        public virtual ReadOnlySpan<byte> ReadSpan(int count) => InternalReadSpan(count);

        /// <summary>
        /// Reads a 16-bit unsigned integer from the underlying byte array and advances the current position by two bytes.
        /// </summary>
        public virtual ushort ReadUInt16() => BinaryPrimitives.ReadUInt16LittleEndian(InternalReadSpan(2));

        /// <summary>
        /// Reads a 32-bit unsigned integer from the underlying byte array and advances the current position by four bytes.
        /// </summary>
        public virtual uint ReadUInt32() => BinaryPrimitives.ReadUInt32LittleEndian(InternalReadSpan(4));

        /// <summary>
        /// Reads a 64-bit unsigned integer from the underlying byte array and advances the current position by eight bytes.
        /// </summary>
        public virtual ulong ReadUInt64() => BinaryPrimitives.ReadUInt64LittleEndian(InternalReadSpan(8));
        

        /// <summary>
        /// Reads the next byte from the underlying byte array and advances the current position by one byte.
        /// </summary>
        protected byte InternalReadByte()
        {
            int curPos = _position;
            int newPos = curPos + 1;
            int relPos = _relativePositon + 1;

            if ((uint)relPos > (uint)Length)
            {
                _relativePositon = Length;
                throw ExceptionHelper.EndOfDataException();
            }

            _relativePositon = relPos;
            _position = newPos;

            return _data[curPos];
        }

        /// <summary>
        /// Returns a read-only span over the specified number of bytes from the underlying byte array and advances the current position by that number of bytes.
        /// </summary>
        /// <param name="count">The size of the read-only span to return.</param>
        protected ReadOnlySpan<byte> InternalReadSpan(int count)
        {
            if (count <= 0) return ReadOnlySpan<byte>.Empty;

            int curPos = _position;
            int newPos = curPos + count;
            int relPos = _relativePositon + count;

            if ((uint)relPos > (uint) Length)
            {
                _relativePositon = Length;
                throw ExceptionHelper.EndOfDataException();
            }

            _relativePositon = relPos;
            _position = newPos;

            return new ReadOnlySpan<byte>(_data, curPos, count);
        }
    }
}
