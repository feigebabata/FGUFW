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
        internal NativeList<long> EntityMasks;//组件类型不超过64个
        private List<SystemBase> _systems;
        private int _nextEntityId;
        private NativeHashMap<int,int> _entity2Indexs;
        private int _entityCount;
        private List<Type> _registerCompTypes;

        public abstract NativeList<T> GetComponents<T>() where T:unmanaged,IComponent;

        /// <summary>
        /// 登记要用的系统
        /// </summary>
        public abstract void RegisterSystems();

        /// <summary>
        /// 登记所有的组件类型 不能漏
        /// </summary>
        /// <returns></returns>
        public abstract List<Type> RegisterComponentTypes();

        public void Initial(int Capacity)
        {
            _systems = new List<SystemBase>();

            EntityMasks = new NativeList<long>(Capacity,Allocator.Persistent);
            _entity2Indexs = new NativeHashMap<int, int>(Capacity,Allocator.Persistent);

            RegisterSystems();

            _registerCompTypes = RegisterComponentTypes();

            Assert.IsTrue(_registerCompTypes.Count<=64,$"compTypes.Count:{_registerCompTypes.Count} 不能超过64!");

            foreach (var system in _systems)
            {
                var c_types = system.GetComponents();
                long filterMask = 0;
                foreach (var c_t in c_types)
                {
                    int bit_idx = _registerCompTypes.IndexOf(c_t);

                    Assert.IsTrue(bit_idx!=-1,$"未登记组件类型:{c_t.FullName}!");

                    filterMask &= 1<<bit_idx;
                }
                system.FilterMask = filterMask;
            }
        }

        public int CreateEntity()
        {
            int entityId = _nextEntityId++;
            _entity2Indexs.Add(entityId,_entityCount);
            _entityCount++;

            EntityMasks.Add(default);

            return entityId;
        }

        protected void onAddComponent<T>(int entityId) where T:unmanaged,IComponent
        {
            var comp_idx = entityIdToComponentsIndex(entityId);
            var compType = typeof(T);

            var bit_idx = _registerCompTypes.IndexOf(compType);
            EntityMasks[comp_idx] = Bit64Helper.Add(EntityMasks[comp_idx],bit_idx);
        }

        protected void onRemoveComponent<T>(int entityId) where T:unmanaged,IComponent
        {
            var comp_idx = entityIdToComponentsIndex(entityId);
            var compType = typeof(T);

            var bit_idx = _registerCompTypes.IndexOf(compType);
            
            EntityMasks[comp_idx] = Bit64Helper.Remove(EntityMasks[comp_idx],bit_idx);
        }

        protected int entityIdToComponentsIndex(int entityId)
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
            foreach (var system in _systems)
            {
                system.Dispose();
            }

            EntityMasks.Dispose();
            _entity2Indexs.Dispose();
        }

        public virtual TransformAccessArray GetTransformAccessArray()
        {
            return default;
        }
    }

    public abstract class SystemBase:IDisposable
    {
        internal long FilterMask;

        public abstract Type[] GetComponents();

        public abstract void Execute(WorldBase world,ref JobHandle jobHandle);    

        public abstract void Dispose();
    }

    public interface IComponent{}

}
