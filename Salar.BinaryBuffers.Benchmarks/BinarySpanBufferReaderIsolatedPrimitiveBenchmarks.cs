using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using Microsoft.VSDiagnostics;

namespace Salar.BinaryBuffers.Benchmarks;
[SimpleJob(RuntimeMoniker.Net10_0, launchCount: 2, warmupCount: 10, iterationCount: 20)]
[GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
[CategoriesColumn]
[CPUUsageDiagnoser]
public class BinarySpanBufferReaderIsolatedPrimitiveBenchmarks
{
    private const int Operations = 1_000_000;
    private byte[] _data = null!;
    private long _longSink;
    [GlobalSetup]
    public void GlobalSetup()
    {
        _data = new byte[16];
        new Random(42).NextBytes(_data);
        _data[12] = 0;
        _data[13] = 0;
        _data[14] = 0;
        _data[15] = 0;
    }

    [Benchmark(Baseline = true, OperationsPerInvoke = Operations)]
    [BenchmarkCategory(nameof(BinarySpanBufferReader.ReadInt32))]
    public void Generic_ReadInt32()
    {
        var reader = new BinarySpanBufferReader(_data);
        long result = 0;
        for (var i = 0; i < Operations; i++)
        {
            reader.Position = 0;
            result += reader.ReadInt32();
        }

        _longSink = result;
    }

    [Benchmark(OperationsPerInvoke = Operations)]
    [BenchmarkCategory(nameof(BinarySpanBufferReader.ReadInt32))]
    public void Specialized_ReadInt32()
    {
        var reader = new SpecializedReader(_data);
        long result = 0;
        for (var i = 0; i < Operations; i++)
        {
            reader.Position = 0;
            result += reader.ReadInt32();
        }

        _longSink = result;
    }

    [Benchmark(Baseline = true, OperationsPerInvoke = Operations)]
    [BenchmarkCategory(nameof(BinarySpanBufferReader.ReadInt64))]
    public void Generic_ReadInt64()
    {
        var reader = new BinarySpanBufferReader(_data);
        long result = 0;
        for (var i = 0; i < Operations; i++)
        {
            reader.Position = 0;
            result ^= reader.ReadInt64();
        }

        _longSink = result;
    }

    [Benchmark(OperationsPerInvoke = Operations)]
    [BenchmarkCategory(nameof(BinarySpanBufferReader.ReadInt64))]
    public void Specialized_ReadInt64()
    {
        var reader = new SpecializedReader(_data);
        long result = 0;
        for (var i = 0; i < Operations; i++)
        {
            reader.Position = 0;
            result ^= reader.ReadInt64();
        }

        _longSink = result;
    }

    [Benchmark(Baseline = true, OperationsPerInvoke = Operations)]
    [BenchmarkCategory(nameof(BinarySpanBufferReader.ReadSingle))]
    public void Generic_ReadSingle()
    {
        var reader = new BinarySpanBufferReader(_data);
        long result = 0;
        for (var i = 0; i < Operations; i++)
        {
            reader.Position = 0;
            result += BitConverter.SingleToInt32Bits(reader.ReadSingle());
        }

        _longSink = result;
    }

    [Benchmark(OperationsPerInvoke = Operations)]
    [BenchmarkCategory(nameof(BinarySpanBufferReader.ReadSingle))]
    public void Specialized_ReadSingle()
    {
        var reader = new SpecializedReader(_data);
        long result = 0;
        for (var i = 0; i < Operations; i++)
        {
            reader.Position = 0;
            result += BitConverter.SingleToInt32Bits(reader.ReadSingle());
        }

        _longSink = result;
    }

    [Benchmark(Baseline = true, OperationsPerInvoke = Operations)]
    [BenchmarkCategory(nameof(BinarySpanBufferReader.ReadDecimal))]
    public void Generic_ReadDecimal()
    {
        var reader = new BinarySpanBufferReader(_data);
        long result = 0;
        for (var i = 0; i < Operations; i++)
        {
            reader.Position = 0;
            var value = reader.ReadDecimal();
            result ^= Unsafe.As<decimal, long>(ref value);
        }

        _longSink = result;
    }

    [Benchmark(OperationsPerInvoke = Operations)]
    [BenchmarkCategory(nameof(BinarySpanBufferReader.ReadDecimal))]
    public void Specialized_ReadDecimal()
    {
        var reader = new SpecializedReader(_data);
        long result = 0;
        for (var i = 0; i < Operations; i++)
        {
            reader.Position = 0;
            var value = reader.ReadDecimal();
            result ^= Unsafe.As<decimal, long>(ref value);
        }

        _longSink = result;
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

        public int Position {[MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => _position = value; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int ReadInt32()
        {
            var position = Advance(sizeof(int));
            return Unsafe.ReadUnaligned<int>(ref Unsafe.Add(ref MemoryMarshal.GetReference(_buffer), position));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public long ReadInt64()
        {
            var position = Advance(sizeof(long));
            return Unsafe.ReadUnaligned<long>(ref Unsafe.Add(ref MemoryMarshal.GetReference(_buffer), position));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float ReadSingle()
        {
            var position = Advance(sizeof(float));
            return Unsafe.ReadUnaligned<float>(ref Unsafe.Add(ref MemoryMarshal.GetReference(_buffer), position));
        }

        public decimal ReadDecimal()
        {
            var position = Advance(16);
            var lo = Unsafe.ReadUnaligned<int>(ref Unsafe.Add(ref MemoryMarshal.GetReference(_buffer), position));
            var mid = Unsafe.ReadUnaligned<int>(ref Unsafe.Add(ref MemoryMarshal.GetReference(_buffer), position + 4));
            var hi = Unsafe.ReadUnaligned<int>(ref Unsafe.Add(ref MemoryMarshal.GetReference(_buffer), position + 8));
            var flags = Unsafe.ReadUnaligned<int>(ref Unsafe.Add(ref MemoryMarshal.GetReference(_buffer), position + 12));
            return new decimal (stackalloc[] { lo, mid, hi, flags });
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int Advance(int count)
        {
            var position = _position;
            var newPosition = position + count;
            if ((uint)newPosition > (uint)_buffer.Length)
            {
                ThrowEndOfDataException();
            }

            _position = newPosition;
            return position;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void ThrowEndOfDataException() => throw new InvalidOperationException();
    }
}