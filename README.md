# BinaryBuffers
An implementation of BinaryReader adn BinaryWriter which works on binary arrays directly by eliminating the need of a middle man Stream.


# Benchmarks

## BinaryBufferReader Benchmarks

|               Method |      Mean |     Error |    StdDev | Ratio |
|--------------------- |----------:|----------:|----------:|------:|
| BinaryReader_ReadInt | 148.77 ms | 0.6931 ms | 0.6144 ms |  1.00 |
| BufferReader_ReadInt |  48.53 ms | 0.1261 ms | 0.1180 ms |  0.33 |


|                   Method |     Mean |     Error |    StdDev | Ratio |
|------------------------- |---------:|----------:|----------:|------:|
| BinaryReader_ReadDecimal | 220.6 ms | 1.4608 ms | 1.2949 ms |  1.00 |
| BufferReader_ReadDecimal | 159.2 ms | 0.5477 ms | 0.4855 ms |  0.72 |


## BinaryBufferWriter Benchmarks


|                Method |      Mean |     Error |     StdDev |    Median | Ratio | RatioSD |
|---------------------- |----------:|----------:|-----------:|----------:|------:|--------:|
| BinaryReader_WriteInt | 183.71 ms | 0.8579 ms |  0.8025 ms | 183.66 ms |  1.00 |    0.00 |
| BufferReader_WriteInt |  76.77 ms | 4.5940 ms | 13.4735 ms |  71.75 ms |  0.43 |    0.08 |


|                    Method |     Mean |    Error |   StdDev | Ratio |
|-------------------------- |---------:|---------:|---------:|------:|
| BinaryReader_WriteDecimal | 226.3 ms | 1.345 ms | 1.258 ms |  1.00 |
| BufferReader_WriteDecimal | 221.5 ms | 2.754 ms | 2.576 ms |  0.98 |