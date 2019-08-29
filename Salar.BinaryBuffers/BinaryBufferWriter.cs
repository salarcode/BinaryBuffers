using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace Salar.BinaryBuffers
{
	public class BinaryBufferWriter
	{
		private readonly byte[] _buffer;
		private readonly int _length;
		private readonly int _originPosition;
		private int _position;

		public BinaryBufferWriter(byte[] buffer)
		{
			_buffer = buffer;
			_position = 0;
			_originPosition = 0;
			_length = buffer.Length;
		}

		public BinaryBufferWriter(byte[] buffer, int position, int length)
		{
			_buffer = buffer;
			_position = position;
			_originPosition = position;
			_length = length;
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

		public int Length => _position - _originPosition;

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

		public virtual void Write(short value)
		{
			var pos = _position;
			Advance(2);

			_buffer[pos + 0] = (byte)value;
			_buffer[pos + 1] = (byte)(value >> 8);
		}

		public virtual void Write(ushort value)
		{
			var pos = _position;
			Advance(2);

			_buffer[pos + 0] = (byte)value;
			_buffer[pos + 1] = (byte)(value >> 8);
		}

		public virtual void Write(int value)
		{
			var pos = _position;
			Advance(4);

			_buffer[pos + 0] = (byte)value;
			_buffer[pos + 1] = (byte)(value >> 8);
			_buffer[pos + 2] = (byte)(value >> 16);
			_buffer[pos + 3] = (byte)(value >> 24);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void Write(int value, int pos)
		{
			_buffer[pos + 0] = (byte)value;
			_buffer[pos + 1] = (byte)(value >> 8);
			_buffer[pos + 2] = (byte)(value >> 16);
			_buffer[pos + 3] = (byte)(value >> 24);
		}

		public virtual void Write(uint value)
		{
			var pos = _position;
			Advance(4);

			_buffer[pos + 0] = (byte)value;
			_buffer[pos + 1] = (byte)(value >> 8);
			_buffer[pos + 2] = (byte)(value >> 16);
			_buffer[pos + 3] = (byte)(value >> 24);
		}

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

		public virtual void Write(bool value)
		{
			var pos = _position;
			Advance(1);

			_buffer[pos + 0] = (byte)(value ? 1 : 0);
		}

		public virtual void Write(byte value)
		{
			var pos = _position;
			Advance(1);

			_buffer[pos + 0] = value;
		}

		public virtual void Write(sbyte value)
		{
			var pos = _position;
			Advance(1);

			_buffer[pos + 0] = (byte)value;
		}

		public virtual void Write(byte[] buffer)
		{
			if (buffer == null)
				throw new ArgumentNullException(nameof(buffer));

			var pos = _position;
			var length = buffer.Length;
			Advance(length);

			Array.Copy(buffer, 0, _buffer, pos, length);
		}

		public virtual void Write(byte[] buffer, int index, int count)
		{
			if (buffer == null)
				throw new ArgumentNullException(nameof(buffer));

			var pos = _position;
			Advance(count);

			Array.Copy(buffer, index, _buffer, pos, count);
		}

		public virtual void Write(in ReadOnlySpan<byte> buffer)
		{
			if (buffer == null)
				throw new ArgumentNullException(nameof(buffer));

			var pos = _position;
			var length = buffer.Length;
			Advance(length);

			if (pos == 0)
				buffer.CopyTo(_buffer);
			else
				buffer.CopyTo(new Span<byte>(_buffer, pos, length));
		}

		public ReadOnlySpan<byte> ToReadOnlySpan() => new ReadOnlySpan<byte>(_buffer, _originPosition, Length);

		public byte[] ToArray() => ToReadOnlySpan().ToArray();

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void Advance(int count)
		{
			var newPos = _position + count;

			if ((uint)newPos > (uint)_length)
			{
				_position = _length;
				throw new EndOfStreamException("Reached to end of data");
			}

			_position = newPos;
		}
	}
}