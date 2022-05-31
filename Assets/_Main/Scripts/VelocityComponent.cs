using Unity.Entities;
using Unity.Mathematics;

namespace SpaceShooter
{
    [GenerateAuthoringComponent]
    public struct VelocityComponent : IComponentData
    {
        public float3 Value;
    }
}