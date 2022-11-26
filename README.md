# BinaryBuffers

![logo](https://github.com/salarcode/BinaryBuffers/blob/master/img/logo.png)

[![NuGet](https://img.shields.io/nuget/v/Salar.BinaryBuffers.svg)](https://www.nuget.org/packages/Salar.BinaryBuffers)

BinaryBuffers offers a highly performant implementation of `BinaryReader` and `BinaryWriter`, working directly on a `byte` array, thus eliminating the need for an intermediate `Stream` object.

# How to use

`BinaryBufferReader` and `BinaryBufferWriter` are the respective names of the reader and writer. Both classes operate on a `byte[]` as its underlying data buffer.

```csharp
// Provide a buffer to the reader/writer
var buffer = new byte[100];

// Write to the buffer
var writer = new BinaryBufferWriter(buffer);

writer.Write(2022);
writer.Write(8.11);

// Read from the buffer
var reader = new BinaryBufferReader(buffer);

var year = reader.ReadInt32();
var time = reader.ReadDouble();
```

## Additional Goodies
Use `StreamBufferReader` as a drop in replacement for `BinaryReader`. Note that there is no performance benefit in using `StreamBufferReader`, it just helps widen the use of `IBufferReader`.

Use `ResetBuffer` method in `BinaryBufferReader` instead of creating a new one and have less allocations!

# Benchmarks

Benchmarks shows up to 59% improvement in writing and 32% in reading.

| BinaryBufferReader |     |     |     |     |
| --- | --- | --- | --- | --- |
| **Method** | **Mean** | **Error** | **StdDev** | **Ratio** |
| `BinaryReader_ReadInt` | 38.96 ms | 0.198 ms | 0.185 ms | baseline |
| `BufferReader_ReadInt` | 26.51 ms | 0.125 ms | 0.104 ms |     -32% |
| `BinaryReader_ReadDecimal` | 45.29 ms | 0.169 ms | 0.158 ms | baseline |
| `BufferReader_ReadDecimal` | 43.49 ms | 0.296 ms | 0.247 ms |      -4% |
| `BinaryReader_ReadFloat` | 22.66 ms | 0.049 ms | 0.044 ms | baseline |
| `BufferReader_ReadFloat` | 15.91 ms | 0.071 ms | 0.067 ms |     -30% |

| BinaryBufferWriter |     |     |     |     |
| --- | --- | --- | --- | --- |
| **Method** | **Mean** | **Error** | **StdDev** | **Ratio** |
| `BinaryWriter_WriteInt` | 67.97 ms | 0.361 ms | 0.337 ms | baseline |
| `BufferWriter_WriteInt` | 34.68 ms | 0.225 ms | 0.210 ms |     -49% |
| `BinaryWriter_WriteDecimal` | 42.65 ms | 0.290 ms | 0.271 ms | baseline |
| `BufferWriter_WriteDecimal` | 17.30 ms | 0.028 ms | 0.022 ms |     -59% |
| `BinaryWriter_WriteFloat` | 34.63 ms | 0.114 ms | 0.101 ms | baseline |
| `BufferWriter_WriteFloat` | 18.53 ms | 0.093 ms | 0.087 ms |     -46% |

Performance tests were generated using **.NET 6.0.11** on:
```
AMD Ryzen 9 5900X, 1 CPU, 24 logical and 12 physical cores
```