# BinaryBuffers
An implementation of BinaryReader and BinaryWriter which works on binary arrays directly by eliminating the need of a middle man Stream.


# Benchmarks

## BinaryBufferReader Benchmarks

|               Method |      Mean |     Error |    StdDev | Ratio |
|--------------------- |----------:|----------:|----------:|------:|
| BinaryReader_ReadInt | 152.47 ms | 0.6484 ms | 0.6065 ms |  1.00 |
| BufferReader_ReadInt |  49.15 ms | 0.2015 ms | 0.1885 ms |  0.32 |


|                   Method |     Mean |     Error |    StdDev | Ratio |
|------------------------- |---------:|----------:|----------:|------:|
|   BinaryReader_ReadFloat | 86.75 ms | 0.3637 ms | 0.3224 ms |  1.00 |
|   BufferReader_ReadFloat | 31.78 ms | 0.1566 ms | 0.1465 ms |  0.37 |


## BinaryBufferWriter Benchmarks


|                Method |      Mean |     Error |    StdDev | Ratio |
|---------------------- |----------:|----------:|----------:|------:|
| BinaryWriter_WriteInt | 185.77 ms | 0.7258 ms | 0.6789 ms |  1.00 |
| BufferWriter_WriteInt |  64.78 ms | 0.2940 ms | 0.2750 ms |  0.35 |


|                  Method |     Mean |     Error |    StdDev | Ratio |
|------------------------ |---------:|----------:|----------:|------:|
| BinaryWriter_WriteFloat | 83.79 ms | 0.3845 ms | 0.3597 ms |  1.00 |
| BufferWriter_WriteFloat | 35.62 ms | 0.1663 ms | 0.1474 ms |  0.43 |