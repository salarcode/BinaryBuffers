# BinaryBuffers

![BinaryBuffers logo](https://github.com/salarcode/BinaryBuffers/blob/master/img/logo.png)

[![NuGet](https://img.shields.io/nuget/v/Salar.BinaryBuffers.svg)](https://www.nuget.org/packages/Salar.BinaryBuffers)

BinaryBuffers is a high-performance .NET library for reading and writing primitive values directly from binary buffers. It gives you `BinaryReader`/`BinaryWriter`-style APIs without requiring an intermediate `Stream`, which reduces allocations and improves throughput in buffer-heavy workloads.

## Why BinaryBuffers?

- Use the recommended span-based reader and writer for direct `byte[]` access
- Work directly with `byte[]`, `Span<byte>`, and `ReadOnlySpan<byte>` buffers
- Reuse existing buffers with `ResetBuffer(...)`
- Read from `ReadOnlyMemory<byte>` and `ReadOnlySequence<byte>`
- Use shared abstractions through `IBufferReader` and `IBufferWriter`
- Swap in stream-based compatibility types when you still need a `Stream`

## Installation

```bash
dotnet add package Salar.BinaryBuffers
```

## Quick start

When working directly with a `byte[]`, `Span<byte>`, or `ReadOnlySpan<byte>`, prefer `BinarySpanBufferWriter` and `BinarySpanBufferReader`. They provide the recommended zero-allocation, lowest-overhead path.

### Recommended span-based API

```csharp
using Salar.BinaryBuffers;

var buffer = new byte[32];
var writer = new BinarySpanBufferWriter(buffer);

writer.Write(2022);
writer.Write(8.11);

var reader = new BinarySpanBufferReader(writer.ToReadOnlySpan());
var year = reader.ReadInt32();
var value = reader.ReadDouble();
```

`BinarySpanBufferWriter` and `BinarySpanBufferReader` are `ref struct` types, so they cannot be stored as class fields or used across `async` boundaries. In those cases, use `BinaryBufferWriter` and `BinaryBufferReader`; they are still very fast and can also be referenced through their shared interfaces.

### Class-based API

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

## Span-based reader and writer

`BinarySpanBufferWriter` and `BinarySpanBufferReader` are zero-allocation, high-performance types that operate directly on spans. As `ref struct` types, they can work with stack-allocated memory (`stackalloc`) without copying or allocating intermediate buffers.

### `BinarySpanBufferWriter`

Use `BinarySpanBufferWriter` to write directly to a `Span<byte>`:

```csharp
// Stack-allocated buffer — no heap allocation
Span<byte> buffer = stackalloc byte[1024];
var writer = new BinarySpanBufferWriter(buffer);

writer.Write(2022);
writer.Write(8.11);

// Get the written bytes as a ReadOnlySpan<byte>
ReadOnlySpan<byte> written = writer.ToReadOnlySpan();
```

`BinarySpanBufferWriter` implements `IBufferWriter` and works seamlessly with generic methods:

```csharp
void Serialize<TBufferWriter>(TBufferWriter writer, int id) where TBufferWriter : IBufferWriter
{
    writer.Write(id);
}

Span<byte> buffer = stackalloc byte[1024];
var writer = new BinarySpanBufferWriter(buffer);
Serialize(writer, 42); // Works via generic constraint — no boxing
```

### `BinarySpanBufferReader`

Use `BinarySpanBufferReader` to read directly from a `ReadOnlySpan<byte>`:

```csharp
ReadOnlySpan<byte> buffer = GetBuffer();
var reader = new BinarySpanBufferReader(buffer);

var id = reader.ReadInt32();
var amount = reader.ReadDouble();
```

Because they are `ref struct` types, span-based readers and writers cannot be stored as class fields, used across `async` boundaries, or boxed to interfaces directly. Use the still-high-performance `BinaryBufferWriter` and `BinaryBufferReader` when those capabilities are needed.

## Additional Goodies

Use `StreamBufferWriter` as a drop-in replacement for `BinaryWriter` when a `Stream` is required.

### `BinaryBufferWriter`

Reuse `BinaryBufferReader`, `BinaryBufferWriter`, `BinarySpanBufferReader`, and `BinarySpanBufferWriter` with `ResetBuffer(...)` instead of creating new instances.

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

- already own the underlying byte buffer and can use the recommended span-based APIs
- want to avoid wrapping buffers in `MemoryStream`
- need predictable, low-allocation binary serialization of primitive values
- want to reuse the same buffer across repeated operations

**Compatibility**

BinaryBuffers does not support cross-endian binary data exchange.

## Benchmarks

Benchmarks in this repository show substantial improvements for common primitive reads and writes when compared to `BinaryReader` and `BinaryWriter`.

### Read benchmarks

Lower is better.

**Int**

| Method                                 | Mean | Error | StdDev | Relative time |
|----------------------------------------| --- | --- | --- | --- |
| `BinaryReader_ReadInt` (.NET built-in) | 25.64 ms | 0.2979 ms | 0.2787 ms | baseline |
| `BufferReader_ReadInt`                 | 3.97 ms | 0.0675 ms | 0.1071 ms | -80% |
| `SpanBufferReader_ReadInt`             | 5.21 ms | 0.0596 ms | 0.0557 ms | -74% |
| `StreamBufferReader_ReadInt`           | 17.54 ms | 0.2505 ms | 0.2343 ms | -13% |
| `BinaryBufferMemoryReader_ReadInt`     | 12.35 ms | 0.1158 ms | 0.1026 ms | -39% |
| `SequenceBufferReader_ReadInt`         | 39.11 ms | 0.5776 ms | 0.5403 ms | +93% |

**Decimal**

| Method | Mean | Error | StdDev | Relative time |
| --- | --- | --- | --- | --- |
| `BinaryReader_ReadDecimal` (.NET built-in) | 20.26 ms | 0.2713 ms | 0.2405 ms | baseline |
| `BufferReader_ReadDecimal` | 20.51 ms | 0.4102 ms | 0.8470 ms | +1% |
| `SpanBufferReader_ReadDecimal` | 3.05 ms | 0.0378 ms | 0.0296 ms | -85% |
| `StreamBufferReader_ReadDecimal` | 24.27 ms | 0.4706 ms | 0.5780 ms | +20% |
| `BinaryBufferMemoryReader_ReadDecimal` | 23.76 ms | 0.1748 ms | 0.1635 ms | +17% |
| `SequenceBufferReader_ReadDecimal` | 38.12 ms | 0.1591 ms | 0.1329 ms | +88% |

**Float**

| Method | Mean | Error | StdDev | Relative time |
| --- | --- | --- | --- | --- |
| `BinaryReader_ReadFloat` (.NET built-in) | 14.60 ms | 0.1148 ms | 0.1018 ms | baseline |
| `BufferReader_ReadFloat` | 2.76 ms | 0.0544 ms | 0.0482 ms | -86% |
| `SpanBufferReader_ReadFloat` | 3.36 ms | 0.0238 ms | 0.0199 ms | -83% |
| `StreamBufferReader_ReadFloat` | 10.16 ms | 0.0977 ms | 0.0816 ms | -50% |
| `BinaryBufferMemoryReader_ReadFloat` | 6.45 ms | 0.0704 ms | 0.0624 ms | -68% |
| `SequenceBufferReader_ReadFloat` | 19.12 ms | 0.3742 ms | 0.3500 ms | -6% |

### Write benchmarks

Lower is better.

**Int**

| Method | Mean | Error | StdDev | Relative time |
| --- | --- | --- | --- | --- |
| `BinaryWriter_WriteInt` (.NET built-in) | 62.53 ms | 1.2336 ms | 2.1279 ms | baseline |
| `BufferWriter_WriteInt` | 11.60 ms | 0.2227 ms | 0.2383 ms | -43% |
| `SpanBufferWriter_WriteInt` | 6.74 ms | 0.1292 ms | 0.1680 ms | -67% |
| `StreamWriter_WriteInt` | 59.54 ms | 1.0247 ms | 0.9585 ms | +194% |

**Decimal**

| Method | Mean | Error | StdDev | Relative time |
| --- | --- | --- | --- | --- |
| `BinaryWriter_WriteDecimal` (.NET built-in) | 40.29 ms | 0.8057 ms | 0.8274 ms | baseline |
| `BufferWriter_WriteDecimal` | 7.11 ms | 0.1013 ms | 0.0947 ms | -65% |
| `SpanBufferWriter_WriteDecimal` | 5.49 ms | 0.0622 ms | 0.0611 ms | -73% |
| `StreamWriter_WriteDecimal` | 33.87 ms | 0.3489 ms | 0.3093 ms | +67% |

**Float**

| Method | Mean | Error | StdDev | Relative time |
| --- | --- | --- | --- | --- |
| `BinaryWriter_WriteFloat` (.NET built-in) | 30.67 ms | 0.3490 ms | 0.3094 ms | baseline |
| `BufferWriter_WriteFloat` | 6.07 ms | 0.1009 ms | 0.1080 ms | -70% |
| `SpanBufferWriter_WriteFloat` | 3.41 ms | 0.0310 ms | 0.0290 ms | -83% |
| `StreamWriter_WriteFloat` | 29.58 ms | 0.5377 ms | 0.5029 ms | +46% |

These benchmark results were recorded with the benchmark project in this repository using .NET 10.0 on:

```text
AMD Ryzen 9 5900X, 1 CPU, 24 logical and 12 physical cores
```
