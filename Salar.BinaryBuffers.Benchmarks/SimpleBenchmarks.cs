using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using System;

namespace Salar.BinaryBuffers.Benchmarks;

[DisassemblyDiagnoser(maxDepth: 3, exportCombinedDisassemblyReport: true)]
public class BinaryBufferWriter_WriteInt32
{
	private readonly byte[] _buffer = new byte[sizeof(int)];
	private readonly BinaryBufferWriter _writer;

	public BinaryBufferWriter_WriteInt32()
	{
		_writer = new BinaryBufferWriter(_buffer);
	}

	[Benchmark]
	public void WriteInt32()
	{
		_writer.Position = 0;
		_writer.Write(0x12345678);
	}
}

[DisassemblyDiagnoser(maxDepth: 3, exportCombinedDisassemblyReport: true)]
[ShortRunJob]
public class BinarySpanBufferWriter_WriteSpan
{
	private const int OperationsPerInvoke = 256;
	private readonly byte[] _source = new byte[1024];
	private readonly byte[] _destination = new byte[1024 * OperationsPerInvoke];

	[Params(4, 64, 1024)]
	public int Length { get; set; }

	[Benchmark(OperationsPerInvoke = OperationsPerInvoke)]
	public int WriteSpan()
	{
		var source = _source.AsSpan(0, Length);
		var writer = new BinarySpanBufferWriter(_destination);

		for (var i = 0; i < OperationsPerInvoke; i++)
		{
			writer.Write(source);
		}

		return writer.WrittenLength;
	}
}
