using System;
using System.IO;
using System.Runtime.CompilerServices;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Jobs;
using Microsoft.VSDiagnostics;

namespace Salar.BinaryBuffers.Benchmarks;
[SimpleJob(RuntimeMoniker.Net10_0, launchCount: 2, warmupCount: 10, iterationCount: 20)]
[CPUUsageDiagnoser]
public class BinaryBufferVsSpanReaderConsumedBenchmarks
{
    private const int Loops = 1_000_000;
    private readonly byte[] _integerData = new byte[12];
    private readonly byte[] _floatData = new byte[4];
    private readonly byte[] _decimalData = new byte[16];
    private BinaryBufferReader _integerReader = null!;
    private BinaryBufferReader _floatReader = null!;
    private BinaryBufferReader _decimalReader = null!;
    private long _longSink;
    private int _intSink;
    [GlobalSetup]
    public void Setup()
    {
        using (var writer = new BinaryWriter(new MemoryStream(_integerData)))
        {
            writer.Write(123456789);
            writer.Write(9876543210L);
        }

        using (var writer = new BinaryWriter(new MemoryStream(_floatData)))
        {
            writer.Write(123.5f);
        }

        using (var writer = new BinaryWriter(new MemoryStream(_decimalData)))
        {
            writer.Write(123.45m);
        }

        _integerReader = new BinaryBufferReader(_integerData);
        _floatReader = new BinaryBufferReader(_floatData);
        _decimalReader = new BinaryBufferReader(_decimalData);
    }

    [Benchmark(Baseline = true, OperationsPerInvoke = Loops)]
    public void BufferReader_ReadInt()
    {
        long checksum = 0;
        for (var i = 0; i < Loops; i++)
        {
            _integerReader.Position = 0;
            checksum += _integerReader.ReadInt32();
            checksum ^= _integerReader.ReadInt64();
        }

        _longSink = checksum;
    }

    [Benchmark(OperationsPerInvoke = Loops)]
    public void SpanBufferReader_ReadInt()
    {
        var reader = new BinarySpanBufferReader(_integerData);
        long checksum = 0;
        for (var i = 0; i < Loops; i++)
        {
            reader.Position = 0;
            checksum += reader.ReadInt32();
            checksum ^= reader.ReadInt64();
        }

        _longSink = checksum;
    }

    [Benchmark(OperationsPerInvoke = Loops)]
    public void BufferReader_ReadFloat()
    {
        var checksum = 0;
        for (var i = 0; i < Loops; i++)
        {
            _floatReader.Position = 0;
            checksum ^= BitConverter.SingleToInt32Bits(_floatReader.ReadSingle());
        }

        _intSink = checksum;
    }

    [Benchmark(OperationsPerInvoke = Loops)]
    public void SpanBufferReader_ReadFloat()
    {
        var reader = new BinarySpanBufferReader(_floatData);
        var checksum = 0;
        for (var i = 0; i < Loops; i++)
        {
            reader.Position = 0;
            checksum ^= BitConverter.SingleToInt32Bits(reader.ReadSingle());
        }

        _intSink = checksum;
    }

    [Benchmark(OperationsPerInvoke = Loops)]
    public void BufferReader_ReadDecimal()
    {
        long checksum = 0;
        for (var i = 0; i < Loops; i++)
        {
            _decimalReader.Position = 0;
            var value = _decimalReader.ReadDecimal();
            checksum ^= Unsafe.As<decimal, long>(ref value);
        }

        _longSink = checksum;
    }

    [Benchmark(OperationsPerInvoke = Loops)]
    public void SpanBufferReader_ReadDecimal()
    {
        var reader = new BinarySpanBufferReader(_decimalData);
        long checksum = 0;
        for (var i = 0; i < Loops; i++)
        {
            reader.Position = 0;
            var value = reader.ReadDecimal();
            checksum ^= Unsafe.As<decimal, long>(ref value);
        }

        _longSink = checksum;
    }
}