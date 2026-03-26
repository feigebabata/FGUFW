using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Jobs;

namespace FGUFW.SimpleECS
{
    public class MovementSystem : ISystem
    {
        private Archetype _filterArchetype;

        public MovementSystem()
        {
            _filterArchetype = this.GenerateFilterArchetype
            (
                TransformAccessBuffer.MetaId,
                ComponentMeta<Velocity>.Id
            );
        }

        public void Execute(WorldBase world, ref JobHandle jobHandle)
        {
            var job = new MovementSystemJob
            {
                EntityArchetypes = world.EntityArchetypes.AsParallelReader(),
                FilterArchetype = _filterArchetype,
                DeltaTime = Time.fixedDeltaTime,
                Velocities = world.GetComponents<Velocity>().AsParallelReader()
            };

            jobHandle = job.Schedule(world.GetTransformAccessArray(),jobHandle);
            
        }

        public void Dispose()
        {
            
        }

        public struct MovementSystemJob : IJobParallelForTransform
        {
            public NativeArray<Archetype>.ReadOnly EntityArchetypes;
            public Archetype FilterArchetype;
            public float DeltaTime;
            public NativeArray<Velocity>.ReadOnly Velocities;

            public void Execute(int index, TransformAccess transform)
            {
                if (!EntityArchetypes[index].HasAll(FilterArchetype)) return;
                execute(index, transform);
            }

            private void execute(int index, TransformAccess transform)
            {
                var velocity = Velocities[index].Value;
                float3 position = transform.position;
                position += velocity * DeltaTime;
                transform.position = position;
            }

        }
    }
}