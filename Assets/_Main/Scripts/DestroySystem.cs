using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

namespace SpaceShooter
{
    public partial class DestroySystem : SystemBase
    {
        private BeginSimulationEntityCommandBufferSystem m_BeginSimEcb;

        protected override void OnCreate()
        {
            m_BeginSimEcb = World.GetOrCreateSystem<BeginSimulationEntityCommandBufferSystem>();
        }

        protected override void OnUpdate()
        {
            var commandBuffer = m_BeginSimEcb.CreateCommandBuffer().AsParallelWriter();

            Entities
                .WithAll<DestroyTag>()
                .ForEach((Entity entity, int nativeThreadIndex) =>
                {
                    commandBuffer.DestroyEntity(nativeThreadIndex, entity);
                }).ScheduleParallel();

            m_BeginSimEcb.AddJobHandleForProducer(Dependency);
        }
    }
}