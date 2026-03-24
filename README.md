# BinaryBuffers

![BinaryBuffers logo](https://github.com/salarcode/BinaryBuffers/blob/master/img/logo.png)

[![NuGet](https://img.shields.io/nuget/v/Salar.BinaryBuffers.svg)](https://www.nuget.org/packages/Salar.BinaryBuffers)

BinaryBuffers is a high-performance .NET library for reading and writing primitive values directly from binary buffers. It gives you `BinaryReader`/`BinaryWriter`-style APIs without requiring an intermediate `Stream`, which reduces allocations and improves throughput in buffer-heavy workloads.

## Why BinaryBuffers?

- Work directly with `byte[]` buffers
- Reuse existing buffers with `ResetBuffer(...)`
- Read from `ReadOnlyMemory<byte>` and `ReadOnlySequence<byte>`
- Use shared abstractions through `IBufferReader` and `IBufferWriter`
- Swap in stream-based compatibility types when you still need a `Stream`

## Installation

```bash
dotnet add package Salar.BinaryBuffers
```

Targets:

- `netstandard2.0`
- `net6.0`
- `net7.0`

## Quick start

```csharp
using Salar.BinaryBuffers;

var buffer = new byte[32];

var writer = new BinaryBufferWriter(buffer);
writer.Write(2022);
writer.Write(8.11);

var bytesWritten = writer.WrittenLength;

var reader = new BinaryBufferReader(buffer, 0, bytesWritten);
var year = reader.ReadInt32();
var value = reader.ReadDouble();
```

## Main types

### `BinaryBufferWriter`

Use `BinaryBufferWriter` when you already have a `byte[]` and want fast writes for primitive values.

```csharp
using Salar.BinaryBuffers;

var buffer = new byte[128];
var writer = new BinaryBufferWriter(buffer);

writer.Write(42);
writer.Write(123.45m);

writer.ResetBuffer();
writer.Write(7);
```

### `BinaryBufferReader`

Use `BinaryBufferReader` to read primitive values from a `byte[]` or `ArraySegment<byte>`.

```csharp
using Salar.BinaryBuffers;

var payload = new byte[16];
var writer = new BinaryBufferWriter(payload);
writer.Write(42);
writer.Write(2.5f);

var reader = new BinaryBufferReader(payload);
var id = reader.ReadInt32();
var amount = reader.ReadSingle();
```

### Additional readers and compatibility types

- `BinaryBufferMemoryReader` reads from `ReadOnlyMemory<byte>`
- `SequenceBufferReader` reads from `ReadOnlySequence<byte>`
- `StreamBufferWriter` is a stream-based writer that implements the same writer abstraction
- `StreamBufferReader` is a stream-based reader that integrates with the same reader abstraction

This makes it easier to program against `IBufferReader` and `IBufferWriter` instead of tying your code to a single storage model.

## When to use it

BinaryBuffers is a good fit when you:

- already own the underlying byte buffer
- want to avoid wrapping buffers in `MemoryStream`
- need predictable, low-allocation binary serialization of primitive values
- want to reuse the same buffer across repeated operations

## Benchmarks

Benchmarks in this repository show substantial improvements for common primitive reads and writes when compared to `BinaryReader` and `BinaryWriter`.

### Read benchmarks

Lower is better.

| Method | Mean | Error | StdDev | Relative time |
| --- | --- | --- | --- | --- |
| `BinaryReader_ReadInt` | 42.23 ms | 0.1487 ms | 0.1318 ms | 1.00x |
| `BufferReader_ReadInt` | 5.53 ms | 0.0265 ms | 0.0221 ms | 0.13x |
| `BinaryReader_ReadDecimal` | 48.28 ms | 0.2038 ms | 0.1906 ms | 1.00x |
| `BufferReader_ReadDecimal` | 34.75 ms | 0.3921 ms | 0.3476 ms | 0.72x |
| `BinaryReader_ReadFloat` | 25.76 ms | 0.1012 ms | 0.0947 ms | 1.00x |
| `BufferReader_ReadFloat` | 3.75 ms | 0.0209 ms | 0.0195 ms | 0.15x |

### Write benchmarks

Lower is better.

| Method | Mean | Error | StdDev | Relative time |
| --- | --- | --- | --- | --- |
| `BinaryWriter_WriteInt` | 62.71 ms | 0.5090 ms | 0.4761 ms | 1.00x |
| `BufferWriter_WriteInt` | 11.05 ms | 0.0307 ms | 0.0240 ms | 0.18x |
| `BinaryWriter_WriteDecimal` | 42.07 ms | 0.1556 ms | 0.1455 ms | 1.00x |
| `BufferWriter_WriteDecimal` | 7.79 ms | 0.0191 ms | 0.0169 ms | 0.19x |
| `BinaryWriter_WriteFloat` | 33.38 ms | 0.1869 ms | 0.1561 ms | 1.00x |
| `BufferWriter_WriteFloat` | 7.79 ms | 0.0191 ms | 0.0169 ms | 0.23x |

These benchmark results were last recorded with the benchmark project in this repository using .NET 7.0.5 on:

```text
AMD Ryzen 9 5900X, 1 CPU, 24 logical and 12 physical cores
```
