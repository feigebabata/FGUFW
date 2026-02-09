using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Jobs;

namespace FGUFW.SimpleECS
{
    public class MovementSystem : SystemBase
    {
        public override Type[] GetComponents()
        {
            return new Type[]
            {
                typeof(Velocity)
            };
        }

        public override void Execute(WorldBase world, ref JobHandle jobHandle)
        {
            var job = new MovementSystemJob
            {
                EntityMasks = world.EntityMasks,
                FilterMask = FilterMask,
                DeltaTime = Time.fixedDeltaTime,
                Velocities = world.GetComponents<Velocity>()
            };

            jobHandle = job.Schedule(world.GetTransformAccessArray(),jobHandle);
            
        }

        public override void Dispose()
        {
            
        }

        public struct MovementSystemJob : IJobParallelForTransform
        {
            public NativeList<long> EntityMasks;
            public long FilterMask;
            public float DeltaTime;
            public NativeList<Velocity> Velocities;

            public void Execute(int index, TransformAccess transform)
            {
                var entityMask = EntityMasks[index];
                if((entityMask&FilterMask) != FilterMask)return;

                var velocity = Velocities[index];
                transform.position += velocity.Value*DeltaTime;
            }
        }
    }
}