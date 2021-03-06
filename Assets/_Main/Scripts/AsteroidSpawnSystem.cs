using System.Diagnostics;
using Unity.Entities;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Unity.Burst;

namespace SpaceShooter
{
    public partial class AsteroidSpawnSystem : SystemBase
    {
        private EntityQuery m_AsteroidQuery;
        private BeginSimulationEntityCommandBufferSystem m_BeginSimECB;
        private EntityQuery m_GameSettingsQuery;
        private Entity m_Prefab;

        protected override void OnCreate()
        {
            m_AsteroidQuery = GetEntityQuery(ComponentType.ReadWrite<AsteroidTag>());
            m_BeginSimECB = World.GetOrCreateSystem<BeginSimulationEntityCommandBufferSystem>();
            m_GameSettingsQuery = GetEntityQuery(ComponentType.ReadWrite<GameSettingsComponent>());
            RequireForUpdate(m_GameSettingsQuery);
        }

        protected override void OnUpdate()
        {
            if (m_Prefab == Entity.Null)
            {
                m_Prefab = GetSingleton<AsteroidAuthoringComponent>().asteroidLargPrefab;
                return;
            }
            var settings = GetSingleton<GameSettingsComponent>();
            var commandBuffer = m_BeginSimECB.CreateCommandBuffer();
            var count = m_AsteroidQuery.CalculateEntityCountWithoutFiltering();
            var asteroidPrefab = m_Prefab;
            var rand = new Unity.Mathematics.Random((uint)Stopwatch.GetTimestamp());

            Job
            .WithCode(() =>
            {
                for (int i = count; i < settings.numAsteroids; ++i)
                {
                    var padding = 0.1f;

                    var xPosition = rand.NextFloat(-1f * ((settings.levelWidth) / 2 - padding), (settings.levelWidth) / 2 - padding);
                    var yPosition = rand.NextFloat(-1f * ((settings.levelHeight) / 2 - padding), (settings.levelHeight) / 2 - padding);

                    var pos = new Translation { Value = new float3(xPosition, yPosition, 0) };
                    var e = commandBuffer.Instantiate(asteroidPrefab);

                    commandBuffer.SetComponent(e, pos);
                    var randomVel = new Vector3(rand.NextFloat(-1f, 1f), rand.NextFloat(-1f, 1f), 0);
                    randomVel.Normalize();
                    randomVel = randomVel * settings.asteroidVelocity;
                    var vel = new VelocityComponent { Value = new float3(randomVel.x, randomVel.y, 0) };
                    commandBuffer.SetComponent(e, vel);
                }
            }).Schedule();

            m_BeginSimECB.AddJobHandleForProducer(Dependency);
        }
    }
}