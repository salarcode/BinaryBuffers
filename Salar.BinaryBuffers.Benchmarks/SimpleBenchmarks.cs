using BenchmarkDotNet.Attributes;

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
