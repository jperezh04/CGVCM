using UnityEngine;

namespace MegaMan25D
{
    [RequireComponent(typeof(Rigidbody))]
    public sealed class RideChaserController : MonoBehaviour
    {
        public bool autoRun = true;
        public float cruiseSpeed = 11f;
        public float boostSpeed = 6f;
        public float brakeAmount = 7f;
        public float acceleration = 28f;
        public float jumpVelocity = 9f;
        public float coyoteTime = 0.12f;

        public bool IsGrounded => Time.time - lastGroundedTime <= coyoteTime;

        private Rigidbody body;
        private float lastGroundedTime = -100f;
        private bool jumpQueued;

        private void Awake()
        {
            body = GetComponent<Rigidbody>();
        }

        private void Update()
        {
            if (GameInput.JumpPressedThisFrame)
            {
                jumpQueued = true;
            }
        }

        private void FixedUpdate()
        {
            float input = GameInput.Horizontal;
            float targetSpeed;

            if (autoRun)
            {
                targetSpeed = cruiseSpeed;
                targetSpeed += Mathf.Max(0f, input) * boostSpeed;
                targetSpeed -= Mathf.Max(0f, -input) * brakeAmount;
                targetSpeed = Mathf.Max(2f, targetSpeed);
            }
            else
            {
                targetSpeed = input * (cruiseSpeed + boostSpeed);
            }

            Vector3 velocity = body.linearVelocity;
            velocity.x = Mathf.MoveTowards(
                velocity.x,
                targetSpeed,
                acceleration * Time.fixedDeltaTime
            );
            velocity.z = 0f;

            if (jumpQueued && IsGrounded)
            {
                velocity.y = jumpVelocity;
                lastGroundedTime = -100f;
            }

            jumpQueued = false;
            body.linearVelocity = velocity;
        }

        private void OnCollisionEnter(Collision collision)
        {
            EvaluateGroundContacts(collision);
        }

        private void OnCollisionStay(Collision collision)
        {
            EvaluateGroundContacts(collision);
        }

        private void EvaluateGroundContacts(Collision collision)
        {
            for (int i = 0; i < collision.contactCount; i++)
            {
                if (collision.GetContact(i).normal.y > 0.5f)
                {
                    lastGroundedTime = Time.time;
                    return;
                }
            }
        }
    }
}
