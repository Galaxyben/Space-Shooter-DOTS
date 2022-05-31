using Unity.Entities;
using Unity.Mathematics;

namespace SpaceShooter
{
    public struct BulletSpawnOffsetComponent : IComponentData
    {
        public float3 Value;
    }
}