using UnityEngine;

namespace MegaMan25D
{
    [RequireComponent(typeof(Rigidbody))]
    public sealed class EnemyFlyer : MonoBehaviour
    {
        public Transform target;
        public float horizontalSpeed = 2.5f;
        public float verticalAmplitude = 1.5f;
        public float verticalFrequency = 1.4f;
        public float chaseDistance = 10f;

        private Rigidbody body;
        private Vector3 origin;

        private void Awake()
        {
            body = GetComponent<Rigidbody>();
            body.useGravity = false;
            origin = transform.position;
        }

        private void FixedUpdate()
        {
            float direction = -1f;

            if (target != null &&
                Mathf.Abs(target.position.x - transform.position.x) <= chaseDistance)
            {
                direction = Mathf.Sign(target.position.x - transform.position.x);
            }

            float targetY = origin.y + Mathf.Sin(Time.time * verticalFrequency) * verticalAmplitude;
            float verticalVelocity = (targetY - transform.position.y) * 2.5f;
            body.linearVelocity = new Vector3(direction * horizontalSpeed, verticalVelocity, 0f);
        }
    }
}
