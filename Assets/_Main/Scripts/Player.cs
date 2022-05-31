using Unity.Entities;
using UnityEngine;

namespace SpaceShooter
{
    [GenerateAuthoringComponent]
    public struct Player : IComponentData
    {
        public Vector2 velocity;
        public float acceleration;
        public float rotationSpeed;
        public Vector2 bulletSpawnoOffset;
        public KeyCode forwardInput;
        public KeyCode rotateRightInput;
        public KeyCode rotateLeftInput;
        public KeyCode shootInput;
        public Entity bullet;

    }
}