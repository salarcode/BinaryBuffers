---
applyTo: "Salar.BinaryBuffers.Benchmarks/**"
---

# Benchmark Project Guidelines

- Benchmark console entry points should bypass the interactive menu when command-line arguments are supplied and forward those arguments to BenchmarkDotNet.
- Benchmarks used for optimization decisions should prioritize stable results over short execution time.
- Unless a benchmark has a documented reason to use another profile, use at least 2 launches, 10 warmup iterations, and 20 measured iterations.
- Use operation counts large enough to prevent sub-nanosecond results, dead-code elimination, and timer-resolution noise; consumption-safe hot-path benchmarks should normally execute at least 1,000,000 operations per invocation.
- Shorter jobs are acceptable for exploratory runs, but their results must not be used for final before/after performance claims.
