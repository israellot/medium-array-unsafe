using System;
using BenchmarkDotNet;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using BenchmarkDotNet.Running;
using UnsafeArray;

public class Program
{
    public static void Main(string[] args)
    {
        var summary = BenchmarkRunner.Run(new[] {
            typeof(SequentialRead),
            typeof(RandomRead),
            typeof(RandomWrite)
        });

        Console.ReadKey();
    }
}

[DisassemblyDiagnoser(maxDepth: 100)]
public class RandomRead
{
    [Params(64, 256, 512, 1024, 4096, 1024 * 16, 1024 * 1024, 1024 * 1024 * 16)]
    public int ArraySize { get; set; }

    private uint[] _safeArray;
    private UnsafeUIntArray _unsafeArray;
    private int[] _sequence;

    public RandomRead()
    {
        _safeArray = new uint[ArraySize];
        _unsafeArray = new UnsafeUIntArray (ArraySize);
        _sequence = new int[ArraySize];
    }

    [GlobalSetup]
    public void GlobalSetup()
    {
        _safeArray = new uint[ArraySize];
        _unsafeArray = new UnsafeUIntArray (ArraySize);

        var random= new Random(111);

        _sequence = new int[ArraySize];
        for (var i = 0; i < ArraySize; i++)
        {
            _sequence[i] = i;

            var v = (uint)random.Next();
            _safeArray[i] = v;
            _unsafeArray[i] = v;
        }

        random.Shuffle(_sequence);
    }
        
    [Benchmark(Baseline = true)]
    public uint SafeArray()
    {
        uint v = 0;

        foreach (var i in _sequence)
            v += _safeArray[i];

        return v;
    }
        
    [Benchmark]
    public uint UnsafeArray()
    {
        uint v = 0;

        foreach (var i in _sequence)
            v += _unsafeArray.Get(i);

        return v;
    }
}

[DisassemblyDiagnoser(maxDepth: 100)]
public class SequentialRead
{
    [Params(64,256,512,1024,4096,1024*16,1024*1024, 1024 * 1024 * 16)]
    public int ArraySize { get; set; }

    private uint[] _safeArray;
    private UnsafeUIntArray _unsafeArray;

    public SequentialRead()
    {
        _safeArray = new uint[ArraySize];
        _unsafeArray = new UnsafeUIntArray(ArraySize);
    }

    [GlobalSetup]
    public void GlobalSetup()
    {
        _safeArray = new uint[ArraySize];
        _unsafeArray = new UnsafeUIntArray(ArraySize);

        var random = new Random();

        for (var i = 0; i < ArraySize; i++)
        {
            _safeArray[i] = (uint)random.Next();
            _unsafeArray[i] = _safeArray[i];
        }
    }


    [Benchmark(Baseline = true)]
    public uint SafeArray()
    {
        uint v = 0;

        var limit = ArraySize;

        for (var i = 0; i < limit; i++)
            v += _safeArray[i];

        return v;
    }


    [Benchmark]
    public uint UnsafeArray()
    {
        uint v = 0;

        var limit = ArraySize;

        for (var i = 0; i < limit; i++)
            v += _unsafeArray.Get(i);

        return v;
    }
}

[DisassemblyDiagnoser(maxDepth: 100)]
public class RandomWrite
{
    [Params(64, 256, 512, 1024, 4096, 1024 * 16, 1024 * 1024, 1024 * 1024 * 16)]
    public int ArraySize { get; set; }

    private uint[] _safeArray;
    private UnsafeUIntArray _unsafeArray;
    private int[] _sequence;

    public RandomWrite()
    {
        _safeArray = new uint[ArraySize];
        _unsafeArray = new UnsafeUIntArray(ArraySize);
        _sequence = new int[ArraySize];
    }

    [GlobalSetup]
    public void GlobalSetup()
    {
        _safeArray = new uint[ArraySize];
        _unsafeArray = new UnsafeUIntArray(ArraySize);

        var random = new Random(111);

        _sequence = new int[ArraySize];
        for (var i = 0; i < ArraySize; i++)
        {
            _sequence[i] = i;
        }
        random.Shuffle(_sequence);
    }

    [Benchmark(Baseline = true)]
    public void SafeArray()
    {                        
        foreach(var i in _sequence)
        {
            _safeArray[i] = 111;
        }
    }


    [Benchmark]
    public void UnsafeArray()
    {
        foreach (var i in _sequence)
        {
            _unsafeArray.Set(i, 111);
        }

    }
}


