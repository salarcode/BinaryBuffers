# BinaryBuffers

![logo](https://github.com/silkfire/BinaryBuffers/blob/master/img/logo.png)

[![NuGet](https://img.shields.io/nuget/v/BinaryBuffers.svg)](https://www.nuget.org/packages/BinaryBuffers)

BinaryBuffers offers a highly performant implementation of `BinaryReader` and `BinaryWriter`, working directly on a `byte` array, thus eliminating the need for an intermediate `Stream` object.

# How to use

`BinaryBufferReader` and `BinaryBufferWriter` are the respective names of the reader and writer. Unlike `BinaryReader`, these classes operate directly on the underlying buffer array (`byte[]`).

```csharp
// The  buffer of the reader/writer
var buffer = new byte[100];

// Writing to the buffer
var writer = new BinaryBufferWriter(buffer);

writer.Write(2019);
writer.Write(8.11);

// Reading from the buffer
var reader = new BinaryBufferReader(buffer);

var year = reader.ReadInt32();
var time = reader.ReadDouble();
```

# Benchmarks

Runtime is **.NET 6** running on a CPU with 16 cores.

## BinaryBufferReader

|               Method |     Mean |    Error |   StdDev |    Ratio | RatioSD |
|--------------------- |---------:|---------:|---------:|---------:|--------:|
| BinaryReader_ReadInt | 39.08 ms | 0.563 ms | 0.527 ms | baseline |         |
| BufferReader_ReadInt | 33.80 ms | 0.055 ms | 0.049 ms |     -14% |    1.3% |

|                   Method |     Mean |    Error |   StdDev |    Ratio | RatioSD |
|------------------------- |---------:|---------:|---------:|---------:|--------:|
| BinaryReader_ReadDecimal | 44.93 ms | 0.275 ms | 0.244 ms | baseline |         |
| BufferReader_ReadDecimal | 37.73 ms | 0.079 ms | 0.074 ms |     -16% |    0.5% |

|                 Method |     Mean |    Error |   StdDev |    Ratio | RatioSD |
|----------------------- |---------:|---------:|---------:|---------:|--------:|
| BinaryReader_ReadFloat | 22.48 ms | 0.051 ms | 0.047 ms | baseline |         |
| BufferReader_ReadFloat | 19.05 ms | 0.016 ms | 0.014 ms |     -15% |    0.2% |


## BinaryBufferWriter

|                Method |     Mean |    Error |   StdDev |    Ratio | RatioSD |
|---------------------- |---------:|---------:|---------:|---------:|--------:|
| BinaryWriter_WriteInt | 66.61 ms | 0.102 ms | 0.096 ms | baseline |         |
| BufferWriter_WriteInt | 33.77 ms | 0.085 ms | 0.075 ms |     -49% |    0.3% |

|                    Method |     Mean |    Error |   StdDev |    Ratio | RatioSD |
|-------------------------- |---------:|---------:|---------:|---------:|--------:|
| BinaryWriter_WriteDecimal | 41.44 ms | 0.159 ms | 0.149 ms | baseline |         |
| BufferWriter_WriteDecimal | 17.14 ms | 0.047 ms | 0.044 ms |     -59% |    0.6% |

|                  Method |     Mean |    Error |   StdDev |    Ratio | RatioSD |
|------------------------ |---------:|---------:|---------:|---------:|--------:|
| BinaryWriter_WriteFloat | 33.94 ms | 0.080 ms | 0.071 ms | baseline |         |
| BufferWriter_WriteFloat | 18.08 ms | 0.051 ms | 0.047 ms |     -47% |    0.3% |
