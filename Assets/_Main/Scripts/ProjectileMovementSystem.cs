using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Unity.Burst;
using System.Diagnostics;

namespace SpaceShooter
{
    public partial class ProjectileMovementSystem : SystemBase
    {

        protected override void OnUpdate()
        {
            var deltaTime = Time.DeltaTime;
            var downleftCorner = Camera.main.ScreenToWorldPoint(new Vector3(0, 0));
            var toprightCorner = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height));
            Entities
            .ForEach((ref Translation translation, in VelocityComponent velocity) =>
            {
                if (translation.Value.x > toprightCorner.x)
                    translation.Value = new float3(downleftCorner.x, translation.Value.y, 0);
                if (translation.Value.x < downleftCorner.x)
                    translation.Value = new float3(toprightCorner.x, translation.Value.y, 0);

                if (translation.Value.y > toprightCorner.y)
                    translation.Value = new float3(translation.Value.x, downleftCorner.y, 0);
                if (translation.Value.y < downleftCorner.y)
                    translation.Value = new float3(translation.Value.x, toprightCorner.y, 0);

                translation.Value.xyz += velocity.Value * deltaTime;
            }).ScheduleParallel();
        }
    }
}