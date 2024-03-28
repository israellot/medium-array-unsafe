using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Buffers.Binary;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;

namespace UnsafeArray;

public unsafe class UnsafeUIntArray
{
    public int Length { get; init; }

    public uint[] Data { get; init; }

    private readonly uint[] _data;
    private readonly uint* _dataPtr;
    public UnsafeUIntArray(int length)
    {
        _data = GC.AllocateArray<uint>(length, pinned: true);

        ref uint dataRef = ref MemoryMarshal.GetArrayDataReference(_data);

        _dataPtr = (uint*)Unsafe.AsPointer<uint>(ref dataRef);

        Length = length;

        Data = _data;        
    }
   
    public uint this[int key]
    {
        get => Get(key);
        set => Set(key, value);
    }    

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe uint Get(int index)
    {
        var ptr = (_dataPtr + (uint)index);
        return *ptr;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe void Set(int index, uint value)
    {
        var ptr = (_dataPtr + (uint)index);

        *ptr = value;
    }

}


