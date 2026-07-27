using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using System.IO;
using Microsoft.VSDiagnostics;

namespace Salar.BinaryBuffers.Benchmarks;
[SimpleJob(RuntimeMoniker.Net90, launchCount: 1, warmupCount: 5, iterationCount: 15)]
[SimpleJob(RuntimeMoniker.Net10_0, launchCount: 1, warmupCount: 5, iterationCount: 15)]
[CPUUsageDiagnoser]
public class BinarySpanBufferWriterDecimalBenchmarks
{
    private const int Loops = 10_000;
    private readonly byte[] _binaryWriterBuffer = new byte[Loops * 16];
    private readonly byte[] _spanWriterBuffer = new byte[Loops * 16];
    private MemoryStream _memoryStream = null!;
    private BinaryWriter _binaryWriter = null!;
    [GlobalSetup]
    public void Setup()
    {
        _memoryStream = new MemoryStream(_binaryWriterBuffer);
        _binaryWriter = new BinaryWriter(_memoryStream);
    }

    [Benchmark(Baseline = true, OperationsPerInvoke = Loops)]
    public byte BinaryWriter_WriteDecimal()
    {
        _memoryStream.Position = 0;
        for (var i = 0; i < Loops; i++)
        {
            _binaryWriter.Write(1024.1024m);
        }

        return _binaryWriterBuffer[^1];
    }

    [Benchmark(OperationsPerInvoke = Loops)]
    public byte BinarySpanBufferWriter_WriteDecimal()
    {
        var writer = new BinarySpanBufferWriter(_spanWriterBuffer);
        for (var i = 0; i < Loops; i++)
        {
            writer.Write(1024.1024m);
        }

        return _spanWriterBuffer[^1];
    }
}