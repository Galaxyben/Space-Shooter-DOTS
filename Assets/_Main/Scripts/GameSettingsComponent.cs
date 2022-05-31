using Unity.Entities;

namespace SpaceShooter
{
    public struct GameSettingsComponent : IComponentData
    {
        public float asteroidVelocity;
        public float bulletVelocity;
        public int numAsteroids;
        public int levelWidth;
        public int levelHeight;
    }
}