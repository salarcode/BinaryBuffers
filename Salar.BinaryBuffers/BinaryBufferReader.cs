using System;
using System.Buffers.Binary;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace Salar.BinaryBuffers
{
	public class BinaryBufferReader : IBufferReader
	{
		private readonly byte[] _data;
		private int _originPosition;
		private int _position;
		private int _length;

		public BinaryBufferReader(byte[] data)
		{
			if (data == null)
				throw new ArgumentNullException(nameof(data));

			_data = data;
			_position = 0;
			_originPosition = 0;
			_length = data.Length;
		}

		public BinaryBufferReader(byte[] data, int position, int length)
		{
			if (data == null)
				throw new ArgumentNullException(nameof(data));

			_data = data;
			_position = position;
			_originPosition = position;
			_length = length;
		}

		public BinaryBufferReader(in ArraySegment<byte> data)
		{
			if (data.Array == null)
				throw new ArgumentNullException(nameof(data));

			_data = data.Array;
			_position = data.Offset;
			_originPosition = _position;
			_length = data.Count;
		}

		public int Position
		{
			get => _position;
			set
			{
				var newPos = _originPosition + value;
				if (newPos > _length)
					throw new ArgumentOutOfRangeException("value", "The new position cannot be larger than the length");
				if (newPos < 0)
					throw new ArgumentOutOfRangeException("value", "The new position is invalid");
				_position = newPos;
			}
		}

		public virtual short ReadInt16() => BinaryPrimitives.ReadInt16LittleEndian(InternalReadSpan(2));

		public virtual ushort ReadUInt16() => BinaryPrimitives.ReadUInt16LittleEndian(InternalReadSpan(2));

		public virtual int ReadInt32() => BinaryPrimitives.ReadInt32LittleEndian(InternalReadSpan(4));

		public virtual uint ReadUInt32() => BinaryPrimitives.ReadUInt32LittleEndian(InternalReadSpan(4));

		public virtual long ReadInt64() => BinaryPrimitives.ReadInt64LittleEndian(InternalReadSpan(8));

		public virtual ulong ReadUInt64() => BinaryPrimitives.ReadUInt64LittleEndian(InternalReadSpan(8));

#if NETFRAMEWORK
		public virtual unsafe float ReadSingle()
		{
			var m_buffer = InternalReadSpan(4);
			uint tmpBuffer = (uint)(m_buffer[0] | m_buffer[1] << 8 | m_buffer[2] << 16 | m_buffer[3] << 24);
			return *((float*)&tmpBuffer);
		}
#else
		public virtual float ReadSingle() =>
			BitConverter.Int32BitsToSingle(BinaryPrimitives.ReadInt32LittleEndian(InternalReadSpan(4)));
#endif

		public virtual double ReadDouble() =>
			BitConverter.Int64BitsToDouble(BinaryPrimitives.ReadInt64LittleEndian(InternalReadSpan(8)));

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
				throw new IOException("Failed to read decimal value", e);
			}
		}

		public virtual byte ReadByte() => InternalReadByte();

		public virtual byte[] ReadBytes(int count) => InternalReadSpan(count).ToArray();
		
		public virtual ReadOnlySpan<byte> ReadSpan(int count) => InternalReadSpan(count);
		
		public virtual sbyte ReadSByte() => (sbyte) InternalReadByte();

		public virtual bool ReadBoolean() => InternalReadByte() != 0;

		protected byte InternalReadByte()
		{
			int origPos = _position;
			int newPos = origPos + 1;

			if ((uint) newPos > (uint) _length)
			{
				_position = _length;
				throw new EndOfStreamException("Reached to end of data");
			}

			var b = _data[origPos];
			_position = newPos;
			return b;
		}

		protected ReadOnlySpan<byte> InternalReadSpan(int count)
		{
			int origPos = _position;
			int newPos = origPos + count;

			if ((uint) newPos > (uint) _length)
			{
				_position = _length;
				throw new EndOfStreamException("Reached to end of data");
			}

			var span = new ReadOnlySpan<byte>(_data, origPos, count);
			_position = newPos;
			return span;
		}
	}
}