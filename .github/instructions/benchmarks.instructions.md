---
applyTo: "Salar.BinaryBuffers.Benchmarks/**"
---

# Benchmark Project Guidelines

- Benchmark console entry points should bypass the interactive menu when command-line arguments are supplied and forward those arguments to BenchmarkDotNet.
- Performance benchmarks should use limited BenchmarkDotNet jobs/iterations and finish in under one minute whenever practical.
