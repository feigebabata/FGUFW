using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Jobs;
using Unity.Collections.LowLevel.Unsafe;

namespace FGUFW.SimpleECS
{    
    public interface IComponent{}

    public interface IComponentBuffer : IDisposable
    {
        void AddDefault(Transform transform=default);
        void RemoveAtSwapBack(int index);
    }
    

    // 静态元数据缓存 组件类型转int 可能会对存档有影响
    public static class ComponentMeta<T> where T : unmanaged, IComponent
    {
        public static readonly int Id;
        public static readonly bool IsTag;

        static ComponentMeta()
        {
            Id = ComponentIDCounter.GetNextId();
            IsTag = UnsafeUtility.SizeOf<T>() <= 1; // 自动识别标签
        }
    }

    internal static class ComponentIDCounter
    {
        private static int _counter = 0;
        public static int GetNextId() => _counter++;
    }

    public class TransformAccessBuffer : IComponentBuffer
    {
        public const int MetaId = Archetype.Length-1; //TransformAccess算是特殊组件 用Archetype最后一位表示
        
        public TransformAccessArray List;

        public TransformAccessBuffer(int initialCapacity)
        {
            List = new TransformAccessArray(initialCapacity);
        }

        public void AddDefault(Transform transform)
        {
            List.Add(transform);
        }

        public void RemoveAtSwapBack(int index)
        {
            //可能需要做池处理 移除屏幕外
            List.RemoveAtSwapBack(index);
        }

        public void Dispose()
        {
            List.Dispose();
        }
    }

    public class ComponentBuffer<T> : IComponentBuffer where T : unmanaged, IComponent
    {
        public NativeList<T> List;
        public ComponentBuffer(int initialCapacity)
        {
            if (!ComponentMeta<T>.IsTag)
            {
                List = new NativeList<T>(initialCapacity, Allocator.Persistent);
            }
        }

        public void AddDefault(Transform transform=default)
        {
            if (!ComponentMeta<T>.IsTag && List.IsCreated)
            {
                List.Add(default);
            }
        }

        public void RemoveAtSwapBack(int index)
        {
            if (!ComponentMeta<T>.IsTag && List.IsCreated)
            {
                List.RemoveAtSwapBack(index);
            }
        }

        public void Dispose()
        {
            if (List.IsCreated) List.Dispose();
        }
    }
}

