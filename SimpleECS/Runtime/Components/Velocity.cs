using Unity.Mathematics;
using UnityEngine;

namespace FGUFW.SimpleECS
{
    public struct Velocity:IComponent
    {
        public float3 Value;
    }
}