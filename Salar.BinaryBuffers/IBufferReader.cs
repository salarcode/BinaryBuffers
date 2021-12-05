using System;

namespace Salar.BinaryBuffers
{
	public interface IBufferReader
	{
		short ReadInt16();
		ushort ReadUInt16();
		int ReadInt32();
		uint ReadUInt32();
		long ReadInt64();
		ulong ReadUInt64();
		float ReadSingle();
		double ReadDouble();
		decimal ReadDecimal();
		byte ReadByte();
		byte[] ReadBytes(int count);
		ReadOnlySpan<byte> ReadSpan(int count);
		sbyte ReadSByte();
		bool ReadBoolean();
	}
}
