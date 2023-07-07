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
Use `StreamBufferWriter` as a drop in replacement for `BinaryWriter` to gain ~10% improvement in performance.

Use `StreamBufferReader` as a drop in replacement for `BinaryReader`. Note that there is no performance benefit in using `StreamBufferReader`, it just helps widen the use of `IBufferReader`.

Use `ResetBuffer` method in `BinaryBufferReader` and `BinaryBufferWriter` instead of creating a new one and have less allocations!

# Benchmarks

Benchmarks shows up to **92%** improvement in writing and **84%** in reading.

| BinaryBufferReader |     |     |     |     |
| --- | --- | --- | --- | --- |
| **Method** | **Mean** | **Error** | **StdDev** | **Ratio** |
| `BinaryReader_ReadInt` | 42.23 ms | 0.1487 ms | 0.1318 ms | baseline |
| `BufferReader_ReadInt` |  5.53 ms | 0.0265 ms | 0.0221 ms |     -89% |
| `BinaryReader_ReadDecimal` | 48.28 ms | 0.2038 ms | 0.1906 ms | baseline |
| `BufferReader_ReadDecimal` | 34.75 ms | 0.3921 ms | 0.3476 ms |     -28% |
| `BinaryReader_ReadFloat` | 25.76 ms | 0.1012 ms | 0.0947 ms | baseline |
| `BufferReader_ReadFloat` |  3.75 ms | 0.0209 ms | 0.0195 ms |     -92% |

| BinaryBufferWriter |     |     |     |     |
| --- | --- | --- | --- | --- |
| **Method** | **Mean** | **Error** | **StdDev** | **Ratio** |
| `BinaryWriter_WriteInt` | 62.71 ms | 0.5090 ms | 0.4761 ms | baseline |
| `BufferWriter_WriteInt` | 11.05 ms | 0.0307 ms | 0.0240 ms |     -77% |
| `BinaryWriter_WriteDecimal` | 42.07 ms | 0.1556 ms | 0.1455 ms | baseline |
| `BufferWriter_WriteDecimal` |  7.79 ms | 0.0191 ms | 0.0169 ms |     -84% |
| `BinaryWriter_WriteFloat` | 33.38 ms | 0.1869 ms | 0.1561 ms | baseline |
| `BufferWriter_WriteFloat` |  7.79 ms | 0.0191 ms | 0.0169 ms |     -84% |

Performance tests were generated using **.NET 7.0.5** on:
```
AMD Ryzen 9 5900X, 1 CPU, 24 logical and 12 physical cores
```