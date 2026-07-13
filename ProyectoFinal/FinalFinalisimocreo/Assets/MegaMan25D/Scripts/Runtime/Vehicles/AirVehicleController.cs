using UnityEngine;

namespace MegaMan25D
{
    [RequireComponent(typeof(Rigidbody))]
    public sealed class AirVehicleController : MonoBehaviour
    {
        public bool autoForward = true;
        public float forwardSpeed = 8f;
        public float horizontalBoost = 5f;
        public float verticalSpeed = 7f;
        public float acceleration = 18f;
        public Vector2 verticalLimits = new Vector2(-3.5f, 4.5f);

        private Rigidbody body;

        private void Awake()
        {
            body = GetComponent<Rigidbody>();
            body.useGravity = false;
        }

        private void FixedUpdate()
        {
            float horizontal = GameInput.Horizontal;
            float vertical = GameInput.Vertical;

            float targetX = autoForward
                ? forwardSpeed + horizontal * horizontalBoost
                : horizontal * (forwardSpeed + horizontalBoost);

            Vector3 targetVelocity = new Vector3(
                targetX,
                vertical * verticalSpeed,
                0f
            );

            body.linearVelocity = Vector3.MoveTowards(
                body.linearVelocity,
                targetVelocity,
                acceleration * Time.fixedDeltaTime
            );

            Vector3 position = body.position;
            position.y = Mathf.Clamp(position.y, verticalLimits.x, verticalLimits.y);
            position.z = 0f;
            body.position = position;
        }
    }
}
