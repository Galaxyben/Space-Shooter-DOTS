using Unity.Entities;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Unity.Burst;

namespace SpaceShooter
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial class InputSpawnSystem : SystemBase
    {
        private BeginSimulationEntityCommandBufferSystem m_BeginSimECB;
        private Entity m_BulletPrefab;

        protected override void OnCreate()
        {
            m_BeginSimECB = World.GetOrCreateSystem<BeginSimulationEntityCommandBufferSystem>();
            RequireSingletonForUpdate<GameSettingsComponent>();
        }

        protected override void OnUpdate()
        {
            if (m_BulletPrefab == Entity.Null)
            {
                m_BulletPrefab = GetSingleton<Bullet>().bulletPrefab;
                return;
            }

            byte shoot = 0;

            if (Input.GetKeyDown("space"))
            {
                shoot = 1;
            }

            var commandBuffer = m_BeginSimECB.CreateCommandBuffer().AsParallelWriter();
            var gameSettings = GetSingleton<GameSettingsComponent>();
            var bulletPrefab = m_BulletPrefab;

            Entities
                .WithAll<PlayerTag>()
                .ForEach((Entity e, int nativeThreadIndex, in Translation position, in Rotation rotation, in Player player, in BulletSpawnOffsetComponent bulletOffset) => {

                    if (shoot != 1) return;

                    var bulletEntity = commandBuffer.Instantiate(nativeThreadIndex, bulletPrefab);
                    Debug.Log("InstantiateBullet");

                    var newPosition = new Translation { Value = position.Value + math.mul(rotation.Value, bulletOffset.Value).xyz };
                    commandBuffer.SetComponent(nativeThreadIndex, bulletEntity, newPosition);

                    var vel = new VelocityComponent { Value = (gameSettings.bulletVelocity * math.mul(rotation.Value, new float3(1, 0, 0)).xyz) + new float3(player.velocity.x, player.velocity.y, 0) };
                    commandBuffer.SetComponent(nativeThreadIndex, bulletEntity, vel);

                }).ScheduleParallel();

            m_BeginSimECB.AddJobHandleForProducer(Dependency);
        }
    }
}