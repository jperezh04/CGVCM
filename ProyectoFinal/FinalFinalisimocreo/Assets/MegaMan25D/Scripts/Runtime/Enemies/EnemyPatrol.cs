using UnityEngine;

namespace MegaMan25D
{
    [RequireComponent(typeof(Rigidbody))]
    public sealed class EnemyPatrol : MonoBehaviour
    {
        public float speed = 2.2f;
        public float patrolDistance = 3f;
        public Transform chaseTarget;
        public float chaseDistance = 7f;
        public int contactDamage = 1;
        public float contactCooldown = 0.8f;

        private Rigidbody body;
        private float originX;
        private float direction = -1f;
        private float nextContactDamageTime;

        private void Awake()
        {
            body = GetComponent<Rigidbody>();
            originX = transform.position.x;
        }

        private void FixedUpdate()
        {
            if (chaseTarget != null &&
                Mathf.Abs(chaseTarget.position.x - transform.position.x) <= chaseDistance)
            {
                direction = Mathf.Sign(chaseTarget.position.x - transform.position.x);
            }
            else
            {
                if (transform.position.x <= originX - patrolDistance) direction = 1f;
                if (transform.position.x >= originX + patrolDistance) direction = -1f;
            }

            Vector3 velocity = body.linearVelocity;
            velocity.x = direction * speed;
            velocity.z = 0f;
            body.linearVelocity = velocity;
        }

        private void OnCollisionStay(Collision collision)
        {
            if (Time.time < nextContactDamageTime)
            {
                return;
            }

            Damageable target = collision.collider.GetComponentInParent<Damageable>();
            if (target == null || target.gameObject == gameObject)
            {
                return;
            }

            if (target.GetComponent<PlayerMotor>() == null &&
                target.GetComponent<RideChaserController>() == null &&
                target.GetComponent<AirVehicleController>() == null)
            {
                return;
            }

            nextContactDamageTime = Time.time + contactCooldown;
            target.TakeDamage(contactDamage);
        }
    }
}
