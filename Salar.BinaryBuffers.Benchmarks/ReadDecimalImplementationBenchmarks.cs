using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using System;
using System.Buffers.Binary;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Microsoft.VSDiagnostics;

namespace Salar.BinaryBuffers.Benchmarks;
[ShortRunJob]
[CPUUsageDiagnoser]
public class ReadDecimalImplementationBenchmarks
{
    private const int Loops = 10_000;
    private readonly byte[] _data = new byte[16];
    private BinaryBufferReader _reader;
    [GlobalSetup]
    public void GlobalSetup()
    {
        var bits = decimal.GetBits(123456789.987654321m);
        for (var i = 0; i < bits.Length; i++)
        {
            BinaryPrimitives.WriteInt32LittleEndian(_data.AsSpan(i * sizeof(int)), bits[i]);
        }

        _reader = new BinaryBufferReader(_data);
    }

    [Benchmark(Baseline = true)]
    public long BufferReader_CurrentReadDecimal()
    {
        long checksum = 0;
        for (var i = 0; i < Loops; i++)
        {
            _reader.Position = 0;
            var value = _reader.ReadDecimal();
            checksum ^= Unsafe.As<decimal, long>(ref value);
        }

        return checksum;
    }

    [Benchmark]
    public long BufferReader_FixedOffsetReadDecimal()
    {
        long checksum = 0;
        for (var i = 0; i < Loops; i++)
        {
            _reader.Position = 0;
            var value = FixedOffsetReadDecimal();
            checksum ^= Unsafe.As<decimal, long>(ref value);
        }

        return checksum;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private decimal FixedOffsetReadDecimal()
    {
        var span = _reader.ReadSpan(16);
        ref byte data = ref MemoryMarshal.GetReference(span);
        var lo = Unsafe.ReadUnaligned<int>(ref data);
        var mid = Unsafe.ReadUnaligned<int>(ref Unsafe.Add(ref data, 4));
        var hi = Unsafe.ReadUnaligned<int>(ref Unsafe.Add(ref data, 8));
        var flags = Unsafe.ReadUnaligned<int>(ref Unsafe.Add(ref data, 12));
        if (!BitConverter.IsLittleEndian)
        {
            lo = BinaryPrimitives.ReverseEndianness(lo);
            mid = BinaryPrimitives.ReverseEndianness(mid);
            hi = BinaryPrimitives.ReverseEndianness(hi);
            flags = BinaryPrimitives.ReverseEndianness(flags);
        }

        return new decimal (stackalloc int[] { lo, mid, hi, flags });
    }
}