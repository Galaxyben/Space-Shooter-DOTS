using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace SpaceShooter
{
    public class SetGameSettingsSystem : UnityEngine.MonoBehaviour, IConvertGameObjectToEntity
    {
        public float asteroidVelocity = 10f;
        public float bulletVelocity = 20f;
        public int levelheight = 1080;
        public int levelwidth = 1920;
        public int numAsteroids = 4;
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            var settings = default(GameSettingsComponent);

            settings.asteroidVelocity = asteroidVelocity;
            settings.bulletVelocity = bulletVelocity;
            settings.levelHeight = levelheight;
            settings.levelWidth = levelwidth;
            settings.numAsteroids = numAsteroids;
            dstManager.AddComponentData(entity, settings);
        }
    }
}