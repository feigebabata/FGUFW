using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Jobs;

namespace FGUFW.SimpleECS
{
/*

简化版ECS :
1. 无法改变组件组合 但可以通过塞入空组件和Job中判空实现
2. entityId通过_nextEntityId自增
3. 用_entity2Indexs维护entityId对组件的索引 
4. 用EntityMasks判断entity组件组合
5. 用NativeQueue记录entity增删
6. 创建entity需要维护 EntityMasks

*/
    public abstract class WorldBase:IDisposable
    {
        internal NativeList<Archetype> EntityArchetypes;//组件类型不超过64个
        protected List<ISystem> _systems;

        private NativeHashMap<int,int> _entity2Indexs;
        private int _nextEntityId;
        private int _entityCount;

        internal Dictionary<int,IComponentBuffer> _componentBuffers;


        /// <summary>
        /// 登记要用的系统 同时决定执行顺序
        /// </summary>
        protected abstract void RegisterSystems();

        /// <summary>
        /// 登记要用的组件
        /// </summary>
        protected abstract void RegisterComponentBuffers(int entityCapacity);

        public WorldBase(int entityCapacity)
        {
            _systems = new List<ISystem>();

            EntityArchetypes = new NativeList<Archetype>(entityCapacity,Allocator.Persistent);
            _entity2Indexs = new NativeHashMap<int, int>(entityCapacity,Allocator.Persistent);
            _componentBuffers = new();

            RegisterComponentBuffers(entityCapacity);
            RegisterSystems();

            Assert.IsTrue(_componentBuffers.Count<=Archetype.Length,$"_componentBuffers.Count:{_componentBuffers.Count} 不能超过Archetype.Length:{Archetype.Length}!");
        }

        public NativeList<T> GetComponents<T>() where T:unmanaged,IComponent
        {
            var typeId = ComponentMeta<T>.Id;
            var componentBuffer = _componentBuffers[typeId] as ComponentBuffer<T>;
            return componentBuffer.List;
        }

        public TransformAccessArray GetTransformAccessArray()
        {
            var typeId = TransformAccessBuffer.MetaId;
            var componentBuffer = _componentBuffers[typeId] as TransformAccessBuffer;
            return componentBuffer.List;
        }

        public void SetComponent<T>(int entityId,T comp) where T:unmanaged,IComponent
        {
            var comp_idx = EntityIdToComponentsIndex(entityId);

            var components = GetComponents<T>();
            components[comp_idx] = comp;

            SetEntityArchetype(entityId,ComponentMeta<T>.Id);
        }

        public void SetTransformAccess(int entityId,Transform comp)
        {
            var comp_idx = EntityIdToComponentsIndex(entityId);

            var components = GetTransformAccessArray();
            components[comp_idx] = comp;

            SetEntityArchetype(entityId,TransformAccessBuffer.MetaId);
        }

        public void SetEntityArchetype(int entityId,int typeId)
        {
            var comp_idx = EntityIdToComponentsIndex(entityId);
            var entityArchetype = EntityArchetypes[comp_idx];
            entityArchetype.Add(typeId);
            EntityArchetypes[comp_idx] = entityArchetype;
        }

        public int CreateEntity(Transform transform=default)
        {
            int entityId = _nextEntityId++;
            _entity2Indexs.Add(entityId,_entityCount++);

            EntityArchetypes.Add(default);
            foreach (var componentBuffer in _componentBuffers.Values)
            {
                componentBuffer.AddDefault(transform);
            }

            if(transform!=default)
            {
                SetEntityArchetype(entityId,TransformAccessBuffer.MetaId);
            }

            return entityId;
        }

        public void DestroyEntity(int entityId)
        {
            int compIdx = EntityIdToComponentsIndex(entityId);
            
            foreach (var componentBuffer in _componentBuffers.Values)
            {
                componentBuffer.RemoveAtSwapBack(compIdx);
            }

            EntityArchetypes.RemoveAtSwapBack(compIdx);
            _entity2Indexs.Remove(entityId);

            _entityCount--;
        }



        public int EntityIdToComponentsIndex(int entityId)
        {
            Assert.IsTrue(_entity2Indexs.ContainsKey(entityId));

            return _entity2Indexs[entityId];
        }


        public void Update()
        {
            JobHandle jobHandle = default;
            foreach (var system in _systems)
            {
                system.Execute(this,ref jobHandle);
            }

            jobHandle.Complete();
        }

        public virtual void Dispose()
        {
            foreach (var componentBuffer in _componentBuffers.Values)
            {
                componentBuffer.Dispose();
            }
            foreach (var system in _systems)
            {
                system.Dispose();
            }

            EntityArchetypes.Dispose();
            _entity2Indexs.Dispose();
        }
    }




}
