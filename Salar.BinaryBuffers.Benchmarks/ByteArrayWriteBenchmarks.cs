using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using System.IO;
using Microsoft.VSDiagnostics;

namespace Salar.BinaryBuffers.Benchmarks;
[ShortRunJob]
[CPUUsageDiagnoser]
public class ByteArrayWriteBenchmarks
{
    private const int Loops = 1_000;
    private byte[] _source;
    private byte[] _sliceSource;
    private byte[] _destination;
    private MemoryStream _memoryStream;
    private BinaryWriter _binaryWriter;
    private BinaryBufferWriter _bufferWriter;
    [Params(4, 64, 1024)]
    public int Length { get; set; }

    [GlobalSetup]
    public void GlobalSetup()
    {
        _source = new byte[Length];
        _sliceSource = new byte[Length + 2];
        _destination = new byte[Length];
        _memoryStream = new MemoryStream(_destination);
        _binaryWriter = new BinaryWriter(_memoryStream);
        _bufferWriter = new BinaryBufferWriter(_destination);
    }

    [Benchmark(Baseline = true)]
    public void BinaryWriter_WriteByteArray()
    {
        for (var i = 0; i < Loops; i++)
        {
            _memoryStream.Position = 0;
            _binaryWriter.Write(_source);
        }
    }

    [Benchmark]
    public void BinaryBufferWriter_WriteByteArray()
    {
        for (var i = 0; i < Loops; i++)
        {
            _bufferWriter.Position = 0;
            _bufferWriter.Write(_source);
        }
    }

    [Benchmark]
    public void BinarySpanBufferWriter_WriteByteArray()
    {
        var writer = new BinarySpanBufferWriter(_destination);
        for (var i = 0; i < Loops; i++)
        {
            writer.Position = 0;
            writer.Write(_source);
        }
    }

    [Benchmark]
    public void BinaryBufferWriter_WriteByteArraySlice()
    {
        for (var i = 0; i < Loops; i++)
        {
            _bufferWriter.Position = 0;
            _bufferWriter.Write(_sliceSource, 1, Length);
        }
    }

    [Benchmark]
    public void BinarySpanBufferWriter_WriteByteArraySlice()
    {
        var writer = new BinarySpanBufferWriter(_destination);
        for (var i = 0; i < Loops; i++)
        {
            writer.Position = 0;
            writer.Write(_sliceSource, 1, Length);
        }
    }
}