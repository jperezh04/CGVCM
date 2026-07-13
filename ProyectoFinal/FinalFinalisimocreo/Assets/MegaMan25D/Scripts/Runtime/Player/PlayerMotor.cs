using UnityEngine;

namespace MegaMan25D
{
    [RequireComponent(typeof(Rigidbody))]
    public sealed class PlayerMotor : MonoBehaviour
    {
        public float moveSpeed = 7.5f;
        public float groundAcceleration = 55f;
        public float airAcceleration = 24f;
        public float jumpVelocity = 10.5f;
        public float coyoteTime = 0.12f;
        public Transform visualRoot;

        public float FacingSign { get; private set; } = 1f;
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
            float horizontal = GameInput.Horizontal;

            if (horizontal > 0.01f)
            {
                SetFacing(1f);
            }
            else if (horizontal < -0.01f)
            {
                SetFacing(-1f);
            }

            if (GameInput.JumpPressedThisFrame)
            {
                jumpQueued = true;
            }
        }

        private void FixedUpdate()
        {
            float horizontal = GameInput.Horizontal;
            Vector3 velocity = body.linearVelocity;
            float acceleration = IsGrounded ? groundAcceleration : airAcceleration;

            velocity.x = Mathf.MoveTowards(
                velocity.x,
                horizontal * moveSpeed,
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
                if (collision.GetContact(i).normal.y > 0.55f)
                {
                    lastGroundedTime = Time.time;
                    return;
                }
            }
        }

        private void SetFacing(float sign)
        {
            FacingSign = Mathf.Sign(sign);

            if (visualRoot == null)
            {
                return;
            }

            Vector3 scale = visualRoot.localScale;
            scale.x = Mathf.Abs(scale.x) * FacingSign;
            visualRoot.localScale = scale;
        }
    }
}
