using System.Collections;
using System.Collections.Generic;
using FGUFW.SimpleECS;
using Unity.Mathematics;
using UnityEngine;

public class SimpleECSTest : MonoBehaviour
{
    public GameObject Cube;
    private WorldBase _world;

    // Start is called before the first frame update
    void Start()
    {
        _world = new SimpleWorld(100);

        var entityId = _world.CreateEntity(Cube.transform);
        _world.SetComponent(entityId,new Velocity(){Value=new float3(0,0.25f,0)});
    }

    // Update is called once per frame
    void Update()
    {
        _world.Update();
    }

    void OnDestroy()
    {
        _world.Dispose();
    }

    public class SimpleWorld : WorldBase
    {
        public SimpleWorld(int entityCapacity) : base(entityCapacity)
        {
        }

        protected override void RegisterComponentBuffers(int entityCapacity)
        {
            _componentBuffers.Add(ComponentMeta<Velocity>.Id,new ComponentBuffer<Velocity>(entityCapacity));
            _componentBuffers.Add(TransformAccessBuffer.MetaId,new TransformAccessBuffer(entityCapacity));
        }

        protected override void RegisterSystems()
        {
            _systems.Add(new MovementSystem());
        }
    }

}
