using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Unity.Physics;
using Unity.Physics.Systems;

namespace SpaceShooter
{
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [UpdateAfter(typeof(ExportPhysicsWorld))]
    [UpdateBefore(typeof(EndFramePhysicsSystem))]
    public partial class CollisionSystem : SystemBase
    {
        StepPhysicsWorld m_StepPhysicsWorldSystem;
        EntityQuery m_TriggerCollisionGroup;
        public BeginSimulationEntityCommandBufferSystem m_BeginSimECB;

        protected override void OnCreate()
        {
            m_StepPhysicsWorldSystem = World.GetOrCreateSystem<StepPhysicsWorld>();

            m_BeginSimECB = World.GetOrCreateSystem<BeginSimulationEntityCommandBufferSystem>();

            m_TriggerCollisionGroup = GetEntityQuery(new EntityQueryDesc
            {
                All = new ComponentType[]
                {
                    typeof(VelocityComponent)
                }
            });
        }

        [BurstCompile]
        struct TriggerCollision : ITriggerEventsJob
        {
            [ReadOnly] public ComponentDataFromEntity<AsteroidTag> asteroidTagGroup;
            public ComponentDataFromEntity<BulletTag> bulletTagGroup;
            public ComponentDataFromEntity<PlayerTag> playerTagGroup;
            public EntityCommandBuffer CommandBuffer;

            public void Execute(TriggerEvent triggerEvent)
            {
                Entity entityA = triggerEvent.EntityA;
                Entity entityB = triggerEvent.EntityB;

                bool isBodyAAsteroid = asteroidTagGroup.HasComponent(entityA);
                bool isBodyBAsteroid = asteroidTagGroup.HasComponent(entityB);

                bool isBodyAPlayer = playerTagGroup.HasComponent(entityA);
                bool isBodyBPlayer = playerTagGroup.HasComponent(entityB);

                bool isBodyABullet = bulletTagGroup.HasComponent(entityA);
                bool isBodyBBullet = bulletTagGroup.HasComponent(entityB);

                if (isBodyAAsteroid && isBodyBAsteroid)
                {
                    return;
                }

                if(isBodyAPlayer && !isBodyBAsteroid || !isBodyAAsteroid && isBodyBPlayer)
                {
                    return;
                }
                else //If player crashed with an asteroid
                {
                    Debug.Log("Player muerto");
                    var playerEntity = isBodyAPlayer ? entityA : entityB;
                    CommandBuffer.AddComponent(playerEntity, new DestroyTag { });
                }

                if ((isBodyAAsteroid && !isBodyBBullet) || (isBodyBAsteroid && !isBodyABullet))
                {
                    return;
                }

                var asteroidEntity = isBodyAAsteroid ? entityA : entityB;
                var bulletEntity = isBodyAAsteroid ? entityB : entityA;

                CommandBuffer.AddComponent(asteroidEntity, new DestroyTag { });
                CommandBuffer.AddComponent(bulletEntity, new DestroyTag { });
            }
        }

        protected override void OnStartRunning()
        {
            base.OnStartRunning();
            this.RegisterPhysicsRuntimeSystemReadOnly();
        }

        protected override void OnUpdate()
        {
            if (m_TriggerCollisionGroup.CalculateEntityCount() == 0)
            {
                return;
            }

            Dependency = new TriggerCollision
            {
                asteroidTagGroup = GetComponentDataFromEntity<AsteroidTag>(true),
                bulletTagGroup = GetComponentDataFromEntity<BulletTag>(),
                playerTagGroup = GetComponentDataFromEntity<PlayerTag>(),
                CommandBuffer = m_BeginSimECB.CreateCommandBuffer()
            }.Schedule(m_StepPhysicsWorldSystem.Simulation, Dependency);

            m_BeginSimECB.AddJobHandleForProducer(Dependency);
        }
    }
}