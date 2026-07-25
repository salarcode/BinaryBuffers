using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using Microsoft.VSDiagnostics;

namespace Salar.BinaryBuffers.Benchmarks;
[DisassemblyDiagnoser(maxDepth: 3, printSource: true, exportCombinedDisassemblyReport: true)]
[SimpleJob(RuntimeMoniker.Net90, launchCount: 1, warmupCount: 3, iterationCount: 8)]
[SimpleJob(RuntimeMoniker.Net10_0, launchCount: 1, warmupCount: 3, iterationCount: 8)]
[CPUUsageDiagnoser]
public class BinarySpanBufferReaderPrimitiveBenchmarks
{
    private byte[] _data = null!;
    [GlobalSetup]
    public void GlobalSetup()
    {
        _data = new byte[46];
        new Random(42).NextBytes(_data);
    }

    [Benchmark(Baseline = true)]
    public long GenericHelper()
    {
        var reader = new BinarySpanBufferReader(_data);
        return reader.ReadByte() + reader.ReadSByte() + reader.ReadInt16() + reader.ReadUInt16() + reader.ReadInt32() + reader.ReadUInt32() + reader.ReadInt64() + unchecked((long)reader.ReadUInt64()) + BitConverter.SingleToInt32Bits(reader.ReadSingle()) + BitConverter.DoubleToInt64Bits(reader.ReadDouble());
    }

    [Benchmark]
    public long SpecializedDirect()
    {
        var reader = new SpecializedReader(_data);
        return reader.ReadByte() + reader.ReadSByte() + reader.ReadInt16() + reader.ReadUInt16() + reader.ReadInt32() + reader.ReadUInt32() + reader.ReadInt64() + unchecked((long)reader.ReadUInt64()) + BitConverter.SingleToInt32Bits(reader.ReadSingle()) + BitConverter.DoubleToInt64Bits(reader.ReadDouble());
    }

    private ref struct SpecializedReader
    {
        private readonly ReadOnlySpan<byte> _buffer;
        private int _position;
        public SpecializedReader(ReadOnlySpan<byte> buffer)
        {
            _buffer = buffer;
            _position = 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public byte ReadByte()
        {
            var position = Advance(1);
            return Unsafe.Add(ref MemoryMarshal.GetReference(_buffer), position);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public sbyte ReadSByte() => (sbyte)ReadByte();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public short ReadInt16()
        {
            var position = Advance(sizeof(short));
            ref byte source = ref Unsafe.Add(ref MemoryMarshal.GetReference(_buffer), position);
            return Unsafe.ReadUnaligned<short>(ref source);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ushort ReadUInt16()
        {
            var position = Advance(sizeof(ushort));
            ref byte source = ref Unsafe.Add(ref MemoryMarshal.GetReference(_buffer), position);
            return Unsafe.ReadUnaligned<ushort>(ref source);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int ReadInt32()
        {
            var position = Advance(sizeof(int));
            ref byte source = ref Unsafe.Add(ref MemoryMarshal.GetReference(_buffer), position);
            return Unsafe.ReadUnaligned<int>(ref source);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public uint ReadUInt32()
        {
            var position = Advance(sizeof(uint));
            ref byte source = ref Unsafe.Add(ref MemoryMarshal.GetReference(_buffer), position);
            return Unsafe.ReadUnaligned<uint>(ref source);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public long ReadInt64()
        {
            var position = Advance(sizeof(long));
            ref byte source = ref Unsafe.Add(ref MemoryMarshal.GetReference(_buffer), position);
            return Unsafe.ReadUnaligned<long>(ref source);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ulong ReadUInt64()
        {
            var position = Advance(sizeof(ulong));
            ref byte source = ref Unsafe.Add(ref MemoryMarshal.GetReference(_buffer), position);
            return Unsafe.ReadUnaligned<ulong>(ref source);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float ReadSingle()
        {
            var position = Advance(sizeof(float));
            ref byte source = ref Unsafe.Add(ref MemoryMarshal.GetReference(_buffer), position);
            return Unsafe.ReadUnaligned<float>(ref source);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public double ReadDouble()
        {
            var position = Advance(sizeof(double));
            ref byte source = ref Unsafe.Add(ref MemoryMarshal.GetReference(_buffer), position);
            return Unsafe.ReadUnaligned<double>(ref source);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int Advance(int count)
        {
            var position = _position;
            var newPosition = position + count;
            if ((uint)newPosition > (uint)_buffer.Length)
                ThrowEndOfDataException();
            _position = newPosition;
            return position;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private void ThrowEndOfDataException() => throw new InvalidOperationException();
    }
}