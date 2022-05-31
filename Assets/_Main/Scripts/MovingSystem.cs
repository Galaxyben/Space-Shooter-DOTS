using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace SpaceShooter
{
    // Update In Group, le digo que OnUpdate corra en un Fixed Step
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    public partial class MovingSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            var deltaTime = Time.DeltaTime;

            var downleftCorner = Camera.main.ScreenToWorldPoint(new Vector3(0, 0));
            var toprightCorner = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height));

            Entities
                .WithoutBurst()
                .WithName("Player")
                .ForEach((ref Translation translation, ref Rotation rotation, ref Player player) =>
                {
                    if(Input.GetKey(player.forwardInput))
                    {
                        Quaternion currentRotation = rotation.Value;
                        float3 directon = new float3(math.cos(math.radians(currentRotation.eulerAngles.z)), math.sin(math.radians(currentRotation.eulerAngles.z)), 0);

                        player.velocity = new Vector2(player.velocity.x + player.acceleration * directon.x * deltaTime, player.velocity.y + player.acceleration * directon.y * deltaTime);
                    }
                    if(Input.GetKey(player.rotateLeftInput))
                    {
                        rotation.Value = math.mul(rotation.Value, quaternion.RotateZ(math.radians(player.rotationSpeed * deltaTime)));
                    }
                    if(Input.GetKey(player.rotateRightInput))
                    {
                        rotation.Value = math.mul(rotation.Value, quaternion.RotateZ(math.radians(-player.rotationSpeed * deltaTime)));
                    }
                    float3 velocity = new float3(player.velocity.x, player.velocity.y, 0);

                    if (translation.Value.x > toprightCorner.x)
                        translation.Value = new float3(downleftCorner.x, translation.Value.y, 0);
                    if (translation.Value.x < downleftCorner.x)
                        translation.Value = new float3(toprightCorner.x, translation.Value.y, 0);

                    if (translation.Value.y > toprightCorner.y)
                        translation.Value = new float3(translation.Value.x, downleftCorner.y, 0);
                    if (translation.Value.y < downleftCorner.y)
                        translation.Value = new float3(translation.Value.x, toprightCorner.y, 0);

                    translation.Value += velocity * deltaTime;
                }).Run();
        }
    }
}