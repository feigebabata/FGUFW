using UnityEngine;
using System;
using Unity.Mathematics;

namespace FGUFW
{
    /*
    栈上数据结构 
    用于减少GC
    用stackalloc生成的数组模拟各种数据结构
    */

    /// <summary>
    /// 栈上队列
    /// </summary>
    public unsafe struct StackMemory_Queue<T> where T:unmanaged
    {
        private T* _array;
        public int Capacity{get;private set;}
        public int Count{get;private set;}

        public StackMemory_Queue(T* array,int length)
        {
            _array = array;
            Capacity = length;
            Count = 0;
        }

        /// <summary>
        /// 入队
        /// </summary>
        public void Enqueue(T t)
        {
            if(Count==Capacity)
            {
                throw new IndexOutOfRangeException("入队失败 容量不够!");
            }
            _array[Count++] = t;
        }

        /// <summary>
        /// 出队
        /// </summary>
        public T Dequeue()
        {
            if(Count==0)
            {
                throw new IndexOutOfRangeException("出队失败 队列已空!");
            }
            Count--;
            T t = _array[0];
            for (int i = 0; i < Count; i++)
            {
                _array[i] = _array[i+1];
            }
            return t;
        }

        public bool Contains(T t)
        {
            for (int i = 0; i < Count; i++)
            {
                if(_array[i].Equals(t))return true;
            }
            return false;
        }

    }

}