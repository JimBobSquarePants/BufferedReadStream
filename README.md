# BufferedReadStream
*Experiments with buffering stream reading to improve performance of multiple short reads.*  

`BufferReadStream` works as intended on NET Core platforms but suffers sever performance issues on NET Framework. However, the **same code** when implemented as a wrapper with no inheritance from `Stream` runs as expected on NET Framework.  
  
I'd really like to know why this is the case?  
  
### Current Benchmark Results.

``` bash
BenchmarkDotNet=v0.12.1, OS=Windows 10.0.18363.778 (1909/November2018Update/19H2)
Intel Core i7-8650U CPU 1.90GHz (Kaby Lake R), 1 CPU, 8 logical and 4 physical cores
.NET Core SDK=3.1.300-preview-015095
  [Host]     : .NET Core 3.1.3 (CoreCLR 4.700.20.11803, CoreFX 4.700.20.12001), X64 RyuJIT
  Job-BGLOUD : .NET Framework 4.8 (4.8.4121.0), X64 RyuJIT
  Job-CUMLBX : .NET Core 2.1.17 (CoreCLR 4.6.28619.01, CoreFX 4.6.28619.01), X64 RyuJIT
  Job-BIWYTD : .NET Core 3.1.3 (CoreCLR 4.700.20.11803, CoreFX 4.700.20.12001), X64 RyuJIT

IterationCount=3  LaunchCount=1  WarmupCount=3
```

|                         Method |        Job |       Runtime |       Mean |      Error |    StdDev | Ratio | RatioSD |
|------------------------------- |----------- |-------------- |-----------:|-----------:|----------:|------:|--------:|
|             StandardStreamRead | Job-BGLOUD |    .NET 4.7.2 |  54.939 us | 15.6522 us | 0.8579 us |  0.66 |    0.01 |
|         BufferedReadStreamRead | Job-BGLOUD |    .NET 4.7.2 |  75.760 us |  2.4355 us | 0.1335 us |  0.91 |    0.01 |
|     BufferedReadStreamWrapRead | Job-BGLOUD |    .NET 4.7.2 |  38.417 us |  6.0844 us | 0.3335 us |  0.46 |    0.00 |
|         StandardStreamReadByte | Job-BGLOUD |    .NET 4.7.2 |  83.327 us |  8.0045 us | 0.4388 us |  1.00 |    0.00 |
|     BufferedReadStreamReadByte | Job-BGLOUD |    .NET 4.7.2 | 101.489 us |  3.1471 us | 0.1725 us |  1.22 |    0.01 |
| BufferedReadStreamWrapReadByte | Job-BGLOUD |    .NET 4.7.2 |  14.795 us |  5.3215 us | 0.2917 us |  0.18 |    0.00 |
|                  ArrayReadByte | Job-BGLOUD |    .NET 4.7.2 |  10.183 us |  0.1695 us | 0.0093 us |  0.12 |    0.00 |
|                                |            |               |            |            |           |       |         |
|             StandardStreamRead | Job-CUMLBX | .NET Core 2.1 |  52.599 us |  7.8719 us | 0.4315 us |  0.68 |    0.01 |
|         BufferedReadStreamRead | Job-CUMLBX | .NET Core 2.1 |  31.800 us |  8.1967 us | 0.4493 us |  0.41 |    0.01 |
|     BufferedReadStreamWrapRead | Job-CUMLBX | .NET Core 2.1 |  32.040 us |  3.8141 us | 0.2091 us |  0.41 |    0.00 |
|         StandardStreamReadByte | Job-CUMLBX | .NET Core 2.1 |  77.764 us |  3.6278 us | 0.1989 us |  1.00 |    0.00 |
|     BufferedReadStreamReadByte | Job-CUMLBX | .NET Core 2.1 |  13.907 us | 24.1798 us | 1.3254 us |  0.18 |    0.02 |
| BufferedReadStreamWrapReadByte | Job-CUMLBX | .NET Core 2.1 |  13.166 us |  0.7063 us | 0.0387 us |  0.17 |    0.00 |
|                  ArrayReadByte | Job-CUMLBX | .NET Core 2.1 |   9.545 us |  0.2658 us | 0.0146 us |  0.12 |    0.00 |
|                                |            |               |            |            |           |       |         |
|             StandardStreamRead | Job-BIWYTD | .NET Core 3.1 |  49.692 us |  5.1482 us | 0.2822 us |  0.67 |    0.01 |
|         BufferedReadStreamRead | Job-BIWYTD | .NET Core 3.1 |  26.924 us | 16.9050 us | 0.9266 us |  0.36 |    0.01 |
|     BufferedReadStreamWrapRead | Job-BIWYTD | .NET Core 3.1 |  36.065 us | 57.5055 us | 3.1521 us |  0.49 |    0.05 |
|         StandardStreamReadByte | Job-BIWYTD | .NET Core 3.1 |  73.977 us | 12.9548 us | 0.7101 us |  1.00 |    0.00 |
|     BufferedReadStreamReadByte | Job-BIWYTD | .NET Core 3.1 |  14.710 us | 21.7169 us | 1.1904 us |  0.20 |    0.01 |
| BufferedReadStreamWrapReadByte | Job-BIWYTD | .NET Core 3.1 |  20.235 us |  2.7917 us | 0.1530 us |  0.27 |    0.00 |
|                  ArrayReadByte | Job-BIWYTD | .NET Core 3.1 |  13.218 us |  1.1534 us | 0.0632 us |  0.18 |    0.00 |

``` bash
// * Legends *
  Mean    : Arithmetic mean of all measurements
  Error   : Half of 99.9% confidence interval
  StdDev  : Standard deviation of all measurements
  Ratio   : Mean of the ratio distribution ([Current]/[Baseline])
  RatioSD : Standard deviation of the ratio distribution ([Current]/[Baseline])
  1 us    : 1 Microsecond (0.000001 sec)
```