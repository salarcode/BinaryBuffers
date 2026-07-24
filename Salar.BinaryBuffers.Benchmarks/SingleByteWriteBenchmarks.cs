using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using System.IO;
using Microsoft.VSDiagnostics;

namespace Salar.BinaryBuffers.Benchmarks;
[ShortRunJob]
[CPUUsageDiagnoser]
public class SingleByteWriteBenchmarks
{
    private const int Loops = 10_000;
    private readonly byte[] _destination = new byte[1];
    private MemoryStream _memoryStream;
    private BinaryWriter _binaryWriter;
    private BinaryBufferWriter _bufferWriter;
    [GlobalSetup]
    public void GlobalSetup()
    {
        _memoryStream = new MemoryStream(_destination);
        _binaryWriter = new BinaryWriter(_memoryStream);
        _bufferWriter = new BinaryBufferWriter(_destination);
    }

    [Benchmark]
    [BenchmarkCategory("Boolean")]
    public void BinaryWriter_WriteBoolean()
    {
        for (var i = 0; i < Loops; i++)
        {
            _memoryStream.Position = 0;
            _binaryWriter.Write(true);
        }
    }

    [Benchmark]
    [BenchmarkCategory("Boolean")]
    public void BinaryBufferWriter_WriteBoolean()
    {
        for (var i = 0; i < Loops; i++)
        {
            _bufferWriter.Position = 0;
            _bufferWriter.Write(true);
        }
    }

    [Benchmark]
    [BenchmarkCategory("Boolean")]
    public void BinarySpanBufferWriter_WriteBoolean()
    {
        var writer = new BinarySpanBufferWriter(_destination);
        for (var i = 0; i < Loops; i++)
        {
            writer.Position = 0;
            writer.Write(true);
        }
    }

    [Benchmark]
    [BenchmarkCategory("Byte")]
    public void BinaryWriter_WriteByte()
    {
        for (var i = 0; i < Loops; i++)
        {
            _memoryStream.Position = 0;
            _binaryWriter.Write((byte)42);
        }
    }

    [Benchmark]
    [BenchmarkCategory("Byte")]
    public void BinaryBufferWriter_WriteByte()
    {
        for (var i = 0; i < Loops; i++)
        {
            _bufferWriter.Position = 0;
            _bufferWriter.Write((byte)42);
        }
    }

    [Benchmark]
    [BenchmarkCategory("Byte")]
    public void BinarySpanBufferWriter_WriteByte()
    {
        var writer = new BinarySpanBufferWriter(_destination);
        for (var i = 0; i < Loops; i++)
        {
            writer.Position = 0;
            writer.Write((byte)42);
        }
    }

    [Benchmark(Baseline = true)]
    [BenchmarkCategory("SByte")]
    public void BinaryWriter_WriteSByte()
    {
        for (var i = 0; i < Loops; i++)
        {
            _memoryStream.Position = 0;
            _binaryWriter.Write((sbyte)-42);
        }
    }

    [Benchmark]
    [BenchmarkCategory("SByte")]
    public void BinaryBufferWriter_WriteSByte()
    {
        for (var i = 0; i < Loops; i++)
        {
            _bufferWriter.Position = 0;
            _bufferWriter.Write((sbyte)-42);
        }
    }

    [Benchmark]
    [BenchmarkCategory("SByte")]
    public void BinarySpanBufferWriter_WriteSByte()
    {
        var writer = new BinarySpanBufferWriter(_destination);
        for (var i = 0; i < Loops; i++)
        {
            writer.Position = 0;
            writer.Write((sbyte)-42);
        }
    }
}