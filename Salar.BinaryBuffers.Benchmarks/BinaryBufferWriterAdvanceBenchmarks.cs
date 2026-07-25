using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Jobs;
using System;
using System.Runtime.CompilerServices;
using Microsoft.VSDiagnostics;

namespace Salar.BinaryBuffers.Benchmarks;
[SimpleJob(RuntimeMoniker.Net90, launchCount: 1, warmupCount: 5, iterationCount: 15)]
[SimpleJob(RuntimeMoniker.Net10_0, launchCount: 1, warmupCount: 5, iterationCount: 15)]
[CPUUsageDiagnoser]
public class BinaryBufferWriterAdvanceBenchmarks
{
    private const int Operations = 1_000;
    private CurrentState _current = null!;
    private AbsoluteState _candidate = null!;
    private int _count;
    [GlobalSetup]
    public void Setup()
    {
        _current = new CurrentState(17, Operations * 4);
        _candidate = new AbsoluteState(17, Operations * 4);
        _count = 4;
    }

    [IterationSetup]
    public void Reset()
    {
        _current.Reset();
        _candidate.Reset();
    }

    [Benchmark(Baseline = true, OperationsPerInvoke = Operations)]
    public int CurrentAdvanceOne()
    {
        for (var i = 0; i < Operations; i++)
        {
            _current.AdvanceOne();
        }

        return _current.Consume();
    }

    [Benchmark(OperationsPerInvoke = Operations)]
    public int CurrentAdvanceVariable()
    {
        for (var i = 0; i < Operations; i++)
        {
            _current.Advance(_count);
        }

        return _current.Consume();
    }

    [Benchmark(OperationsPerInvoke = Operations)]
    public int NoCountCheckAdvanceVariable()
    {
        for (var i = 0; i < Operations; i++)
        {
            _current.AdvanceWithoutCountCheck(_count);
        }

        return _current.Consume();
    }

    [Benchmark(OperationsPerInvoke = Operations)]
    public int AbsoluteAdvanceOne()
    {
        for (var i = 0; i < Operations; i++)
        {
            _candidate.AdvanceOne();
        }

        return _candidate.Consume();
    }

    [Benchmark(OperationsPerInvoke = Operations)]
    public int CurrentAdvanceFour()
    {
        for (var i = 0; i < Operations; i++)
        {
            _current.Advance(4);
        }

        return _current.Consume();
    }

    [Benchmark(OperationsPerInvoke = Operations)]
    public int NoCountCheckAdvanceFour()
    {
        for (var i = 0; i < Operations; i++)
        {
            _current.AdvanceWithoutCountCheck(4);
        }

        return _current.Consume();
    }

    [Benchmark(OperationsPerInvoke = Operations)]
    public int AbsoluteAdvanceFour()
    {
        for (var i = 0; i < Operations; i++)
        {
            _candidate.Advance(4);
        }

        return _candidate.Consume();
    }

    private sealed class CurrentState
    {
        private readonly int _offset;
        private readonly int _length;
        private int _position;
        private int _relativePosition;
        private int _writtenLength;
        public CurrentState(int offset, int length)
        {
            _offset = offset;
            _length = length;
            Reset();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AdvanceOne()
        {
            var newRelativePosition = _relativePosition + 1;
            if ((uint)newRelativePosition > (uint)_length)
            {
                ThrowEndOfDataException();
            }

            _relativePosition = newRelativePosition;
            _position++;
            if ((uint)newRelativePosition > (uint)_writtenLength)
            {
                _writtenLength = newRelativePosition;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AdvanceWithoutCountCheck(int count)
        {
            var newRelativePosition = _relativePosition + count;
            if ((uint)newRelativePosition > (uint)_length)
            {
                ThrowEndOfDataException();
            }

            _relativePosition = newRelativePosition;
            _position += count;
            if ((uint)newRelativePosition > (uint)_writtenLength)
            {
                _writtenLength = newRelativePosition;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Advance(int count)
        {
            var newRelativePosition = _relativePosition + count;
            if ((uint)newRelativePosition > (uint)_length)
            {
                ThrowEndOfDataException();
            }

            _relativePosition = newRelativePosition;
            _position += count;
            if (count > 0 && (uint)newRelativePosition > (uint)_writtenLength)
            {
                _writtenLength = newRelativePosition;
            }
        }

        public void Reset()
        {
            _position = _offset;
            _relativePosition = 0;
            _writtenLength = 0;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void ThrowEndOfDataException() => throw new InvalidOperationException();
        [MethodImpl(MethodImplOptions.NoInlining)]
        public int Consume() => _position ^ _relativePosition ^ _writtenLength;
    }

    private sealed class AbsoluteState
    {
        private readonly int _offset;
        private readonly int _end;
        private int _position;
        private int _writtenPosition;
        public AbsoluteState(int offset, int length)
        {
            _offset = offset;
            _end = offset + length;
            Reset();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AdvanceOne()
        {
            var newPosition = _position + 1;
            if ((uint)newPosition > (uint)_end)
            {
                ThrowEndOfDataException();
            }

            _position = newPosition;
            if ((uint)newPosition > (uint)_writtenPosition)
            {
                _writtenPosition = newPosition;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Advance(int count)
        {
            var newPosition = _position + count;
            if ((uint)(newPosition - _offset) > (uint)(_end - _offset))
            {
                ThrowEndOfDataException();
            }

            _position = newPosition;
            if ((uint)newPosition > (uint)_writtenPosition)
            {
                _writtenPosition = newPosition;
            }
        }

        public void Reset()
        {
            _position = _offset;
            _writtenPosition = _offset;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void ThrowEndOfDataException() => throw new InvalidOperationException();
        [MethodImpl(MethodImplOptions.NoInlining)]
        public int Consume() => _position ^ (_position - _offset) ^ (_writtenPosition - _offset);
    }
}