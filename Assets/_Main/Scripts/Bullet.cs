using Unity.Entities;

namespace SpaceShooter
{
    [GenerateAuthoringComponent]
    public struct Bullet : IComponentData
    {
        public Entity bulletPrefab;
    }
}