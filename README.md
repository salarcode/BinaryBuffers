# BinaryBuffers

![logo](https://github.com/silkfire/BinaryBuffers/blob/master/img/logo.png)

[![NuGet](https://img.shields.io/nuget/v/BinaryBuffers.svg)](https://www.nuget.org/packages/BinaryBuffers)

BinaryBuffers offers a highly performant implementation of `BinaryReader` and `BinaryWriter`, working directly on a `byte` array, thus eliminating the need for an intermediate `Stream` object.

# How to use

`BinaryBufferReader` and `BinaryBufferWriter` are the respective names of the reader and writer. Both classes operate on a `byte[]` as its underlying data buffer.

```csharp
// Provide a buffer to the reader/writer
var buffer = new byte[100];

// Write to the buffer
var writer = new BinaryBufferWriter(buffer);

writer.Write(2019);
writer.Write(8.11);

// Read from the buffer
var reader = new BinaryBufferReader(buffer);

var year = reader.ReadInt32();
var time = reader.ReadDouble();
```

# Benchmarks

Performance tests were generated using **.NET 6** running on a machine with a 16-core CPU.

<table style="width: 100%">
    <thead>
        <tr>
          <th colspan="5"><span style="font-size: 20px;">BinaryBufferReader</span></th>
        </tr>
        <tr>
            <th>Method</th>
            <th>Mean</th>
            <th>Error</th>
            <th>StdDev</th>
            <th>Ratio</th>
        </tr>
    </thead>
    <tbody>
        <tr>
            <td><code>BinaryReader_ReadInt</code></td>
            <td>39.08 ms</td>
            <td>0.563 ms</td>
            <td>0.527 ms</td>
            <td><em>baseline</em></td>
        </tr>
        <tr>
            <td><code>BufferReader_ReadInt</code></td>
            <td>33.80 ms</td>
            <td>0.055 ms</td>
            <td>0.049 ms</td>
            <td style="text-align: right;">-14%</td>
        </tr>
        <tr>
          <th colspan="5"><span style="font-size: 20px;"></span></th>
        </tr>
        <tr>
            <td><code>BinaryReader_ReadDecimal</code></td>
            <td>44.93 ms</td>
            <td>0.275 ms</td>
            <td>0.244 ms</td>
            <td><em>baseline</em></td>
        </tr>
        <tr>
            <td><code>BufferReader_ReadDecimal</code></td>
            <td>37.73 ms</td>
            <td>0.079 ms</td>
            <td>0.074 ms</td>
            <td style="text-align: right;">-16%</td>
        </tr>
        <tr>
          <th colspan="5"><span style="font-size: 20px;"></span></th>
        </tr>
        <tr>
            <td><code>BinaryReader_ReadFloat</code></td>
            <td>22.48 ms</td>
            <td>0.051 ms</td>
            <td>0.047 ms</td>
            <td><em>baseline</em></td>
        </tr>
        <tr>
            <td><code>BufferReader_ReadFloat</code></td>
            <td>19.05 ms</td>
            <td>0.016 ms</td>
            <td>0.014 ms</td>
            <td style="text-align: right;">-13%</td>
        </tr>
        <tr>
          <th colspan="5"><span style="font-size: 20px;"></span></th>
        </tr>
    </tbody>
    <thead>
        <tr>
          <th colspan="5"><span style="font-size: 20px;">BinaryBufferWriter</span></th>
        </tr>
        <tr>
            <th>Method</th>
            <th>Mean</th>
            <th>Error</th>
            <th>StdDev</th>
            <th>Ratio</th>
        </tr>
    </thead>
    <tbody>
        <tr>
            <td><code>BinaryWriter_WriteInt</code></td>
            <td>66.61 ms</td>
            <td>0.102 ms</td>
            <td>0.096 ms</td>
            <td><em>baseline</em></td>
        </tr>
        <tr>
            <td><code>BufferWriter_WriteInt</code></td>
            <td>33.77 ms</td>
            <td>0.085 ms</td>
            <td>0.075 ms</td>
            <td style="text-align: right;">-49%</td>
        </tr>
        <tr>
          <th colspan="5"><span style="font-size: 20px;"></span></th>
        </tr>
        <tr>
            <td><code>BinaryWriter_WriteDecimal</code></td>
            <td>41.44 ms</td>
            <td>0.159 ms</td>
            <td>0.149 ms</td>
            <td><em>baseline</em></td>
        </tr>
        <tr>
            <td><code>BufferWriter_WriteDecimal</code></td>
            <td>17.14 ms</td>
            <td>0.047 ms</td>
            <td>0.044 ms</td>
            <td style="text-align: right;">-59%</td>
        </tr>
        <tr>
          <th colspan="5"><span style="font-size: 20px;"></span></th>
        </tr>
        <tr>
            <td><code>BinaryWriter_WriteFloat</code></td>
            <td>33.94 ms</td>
            <td>0.080 ms</td>
            <td>0.071 ms</td>
            <td><em>baseline</em></td>
        </tr>
        <tr>
            <td><code>BufferWriter_WriteFloat</code></td>
            <td>18.08 ms</td>
            <td>0.051 ms</td>
            <td>0.047 ms</td>
            <td style="text-align: right;">-47%</td>
        </tr>
    </tbody>
</table>
