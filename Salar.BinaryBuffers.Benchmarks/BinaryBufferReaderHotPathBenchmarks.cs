using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using System;
using System.IO;
using Microsoft.VSDiagnostics;

namespace Salar.BinaryBuffers.Benchmarks;
[ShortRunJob]
[CPUUsageDiagnoser]
public class BinaryBufferReaderReadByteBenchmarks
{
    private const int Loops = 1_000;
    private readonly byte[] _data = new byte[1];
    private MemoryStream _stream;
    private BinaryReader _binaryReader;
    private BinaryBufferReader _bufferReader;
    [GlobalSetup]
    public void GlobalSetup()
    {
        _stream = new MemoryStream(_data);
        _binaryReader = new BinaryReader(_stream);
        _bufferReader = new BinaryBufferReader(_data);
    }

    [Benchmark(Baseline = true)]
    public void BinaryReader_ReadByte()
    {
        for (var i = 0; i < Loops; i++)
        {
            _stream.Position = 0;
            _binaryReader.ReadByte();
        }
    }

    [Benchmark]
    public void BinaryBufferReader_ReadByte()
    {
        for (var i = 0; i < Loops; i++)
        {
            _bufferReader.Position = 0;
            _bufferReader.ReadByte();
        }
    }
}

[ShortRunJob]
[CPUUsageDiagnoser]
public class BinaryBufferReaderAdvanceBenchmarks
{
    private const int Loops = 1_000;
    private readonly byte[] _data = new byte[sizeof(int)];
    private MemoryStream _stream;
    private BinaryReader _binaryReader;
    private BinaryBufferReader _bufferReader;
    [GlobalSetup]
    public void GlobalSetup()
    {
        _stream = new MemoryStream(_data);
        _binaryReader = new BinaryReader(_stream);
        _bufferReader = new BinaryBufferReader(_data);
    }

    [Benchmark(Baseline = true)]
    public void BinaryReader_ReadInt32()
    {
        for (var i = 0; i < Loops; i++)
        {
            _stream.Position = 0;
            _binaryReader.ReadInt32();
        }
    }

    [Benchmark]
    public void BinaryBufferReader_ReadInt32()
    {
        for (var i = 0; i < Loops; i++)
        {
            _bufferReader.Position = 0;
            _bufferReader.ReadInt32();
        }
    }
}

[ShortRunJob]
[CPUUsageDiagnoser]
public class BinaryBufferReaderReadSpanBenchmarks
{
    private const int Loops = 1_000;
    private byte[] _data;
    private BinaryBufferMemoryReader _memoryReader;
    private BinaryBufferReader _bufferReader;
    [Params(4, 64, 1024)]
    public int Count { get; set; }

    [GlobalSetup]
    public void GlobalSetup()
    {
        _data = new byte[Count];
        ReadOnlyMemory<byte> memory = _data;
        _memoryReader = new BinaryBufferMemoryReader(in memory);
        _bufferReader = new BinaryBufferReader(_data);
    }

    [Benchmark(Baseline = true)]
    public void BinaryBufferMemoryReader_ReadSpan()
    {
        for (var i = 0; i < Loops; i++)
        {
            _memoryReader.Position = 0;
            _memoryReader.ReadSpan(Count);
        }
    }

    [Benchmark]
    public void BinaryBufferReader_ReadSpan()
    {
        for (var i = 0; i < Loops; i++)
        {
            _bufferReader.Position = 0;
            _bufferReader.ReadSpan(Count);
        }
    }
}

[ShortRunJob]
[CPUUsageDiagnoser]
public class BinaryBufferReaderReadMemoryBenchmarks
{
    private const int Loops = 1_000;
    private byte[] _data;
    private BinaryBufferMemoryReader _memoryReader;
    private BinaryBufferReader _bufferReader;
    [Params(4, 64, 1024)]
    public int Count { get; set; }

    [GlobalSetup]
    public void GlobalSetup()
    {
        _data = new byte[Count];
        ReadOnlyMemory<byte> memory = _data;
        _memoryReader = new BinaryBufferMemoryReader(in memory);
        _bufferReader = new BinaryBufferReader(_data);
    }

    [Benchmark(Baseline = true)]
    public void BinaryBufferMemoryReader_ReadMemory()
    {
        for (var i = 0; i < Loops; i++)
        {
            _memoryReader.Position = 0;
            _memoryReader.ReadMemory(Count);
        }
    }

    [Benchmark]
    public void BinaryBufferReader_ReadMemory()
    {
        for (var i = 0; i < Loops; i++)
        {
            _bufferReader.Position = 0;
            _bufferReader.ReadMemory(Count);
        }
    }
}