using Unity.Entities;

namespace SpaceShooter
{
    [GenerateAuthoringComponent]
    public struct AsteroidAuthoringComponent : IComponentData
    {
        public Entity asteroidLargPrefab;
        public Entity asteroidSmallPrefab;
    }
}