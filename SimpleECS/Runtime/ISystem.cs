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
    public interface ISystem:IDisposable
    {
        void Execute(WorldBase world,ref JobHandle jobHandle);
    }

    public static class ISystemExtensions
    {
        public static Archetype GenerateFilterArchetype(this ISystem self,params int[] typeIds)
        {
            Archetype archetype = new();
            foreach (var t_id in typeIds)
            {
                archetype.Add(t_id);
            }
            return archetype;
        }
    }
}