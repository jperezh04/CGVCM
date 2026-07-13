using UnityEngine;

namespace MegaMan25D
{
    public enum BossMovementMode
    {
        Ground,
        Air
    }

    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(Damageable))]
    public sealed class BossController : MonoBehaviour
    {
        public string bossDisplayName = "Maverick";
        public BossMovementMode movementMode = BossMovementMode.Ground;
        public Transform target;
        public Transform muzzle;
        public Projectile projectilePrefab;

        [Header("Arena")]
        public Vector2 horizontalBounds = new Vector2(-8f, 8f);
        public Vector2 verticalBounds = new Vector2(-2f, 4f);

        [Header("Movement")]
        public float moveSpeed = 4f;
        public float airVerticalSpeed = 4f;
        public float dashSpeed = 12f;
        public float jumpVelocity = 9f;

        [Header("Combat")]
        public float actionInterval = 1.6f;
        public float projectileSpeed = 12f;
        public int projectileDamage = 1;
        public int contactDamage = 2;
        public float contactCooldown = 0.7f;

        public bool IsActive { get; private set; }

        private Rigidbody body;
        private Damageable damageable;
        private float nextActionTime;
        private float dashUntil;
        private Vector3 dashDirection;
        private float nextContactTime;
        private bool grounded;

        private void Awake()
        {
            body = GetComponent<Rigidbody>();
            damageable = GetComponent<Damageable>();
            body.useGravity = movementMode == BossMovementMode.Ground;
        }

        public void ConfigureArena(Vector2 xBounds, Vector2 yBounds)
        {
            horizontalBounds = xBounds;
            verticalBounds = yBounds;
        }

        public void Activate(Transform playerTarget)
        {
            target = playerTarget;
            IsActive = true;
            nextActionTime = Time.time + 0.75f;
        }

        public void Deactivate()
        {
            IsActive = false;
            body.linearVelocity = Vector3.zero;
        }

        private void FixedUpdate()
        {
            if (!IsActive || target == null || damageable.IsDead)
            {
                return;
            }

            if (Time.time < dashUntil)
            {
                body.linearVelocity = dashDirection * dashSpeed;
                ClampToArena();
                return;
            }

            if (movementMode == BossMovementMode.Air)
            {
                UpdateAirMovement();
            }
            else
            {
                UpdateGroundMovement();
            }

            if (Time.time >= nextActionTime)
            {
                PerformAction();
            }

            ClampToArena();
        }

        private void UpdateGroundMovement()
        {
            Vector3 velocity = body.linearVelocity;
            float sign = Mathf.Sign(target.position.x - transform.position.x);
            velocity.x = sign * moveSpeed;
            velocity.z = 0f;
            body.linearVelocity = velocity;
        }

        private void UpdateAirMovement()
        {
            Vector3 difference = target.position - transform.position;
            Vector3 desired = new Vector3(
                Mathf.Sign(difference.x) * moveSpeed,
                Mathf.Clamp(difference.y, -1f, 1f) * airVerticalSpeed,
                0f
            );

            body.linearVelocity = Vector3.MoveTowards(
                body.linearVelocity,
                desired,
                12f * Time.fixedDeltaTime
            );
        }

        private void PerformAction()
        {
            float healthFactor = damageable.Health01;
            float adjustedInterval = Mathf.Lerp(actionInterval * 0.55f, actionInterval, healthFactor);
            nextActionTime = Time.time + Mathf.Max(0.35f, adjustedInterval);

            int action = Random.Range(0, 3);

            if (action == 0)
            {
                FireSpread(healthFactor < 0.5f ? 5 : 3);
                return;
            }

            if (action == 1)
            {
                Vector3 difference = target.position - transform.position;
                dashDirection = movementMode == BossMovementMode.Air
                    ? difference.normalized
                    : new Vector3(Mathf.Sign(difference.x), 0f, 0f);

                dashUntil = Time.time + (healthFactor < 0.5f ? 0.55f : 0.38f);
                return;
            }

            if (movementMode == BossMovementMode.Ground && grounded)
            {
                Vector3 velocity = body.linearVelocity;
                velocity.y = jumpVelocity;
                velocity.x = Mathf.Sign(target.position.x - transform.position.x) * moveSpeed * 1.5f;
                body.linearVelocity = velocity;
                grounded = false;
            }
            else
            {
                FireAimedShot();
            }
        }

        private void FireSpread(int count)
        {
            if (projectilePrefab == null || muzzle == null || target == null)
            {
                return;
            }

            Vector3 baseDirection = (target.position - muzzle.position).normalized;
            float totalAngle = count <= 3 ? 26f : 48f;

            for (int i = 0; i < count; i++)
            {
                float t = count <= 1 ? 0.5f : (float)i / (count - 1);
                float angle = Mathf.Lerp(-totalAngle * 0.5f, totalAngle * 0.5f, t);
                Vector3 direction = Quaternion.AngleAxis(angle, Vector3.forward) * baseDirection;
                Projectile shot = Instantiate(projectilePrefab, muzzle.position, Quaternion.identity);
                shot.Launch(transform, direction * projectileSpeed, projectileDamage);
            }
        }

        private void FireAimedShot()
        {
            if (projectilePrefab == null || muzzle == null || target == null)
            {
                return;
            }

            Vector3 direction = (target.position - muzzle.position).normalized;
            Projectile shot = Instantiate(projectilePrefab, muzzle.position, Quaternion.identity);
            shot.Launch(transform, direction * projectileSpeed * 1.2f, projectileDamage);
        }

        private void ClampToArena()
        {
            Vector3 position = body.position;
            position.x = Mathf.Clamp(position.x, horizontalBounds.x, horizontalBounds.y);

            if (movementMode == BossMovementMode.Air)
            {
                position.y = Mathf.Clamp(position.y, verticalBounds.x, verticalBounds.y);
            }

            position.z = 0f;
            body.position = position;
        }

        private void OnCollisionEnter(Collision collision)
        {
            EvaluateGround(collision);
            DamageTarget(collision.collider);
        }

        private void OnCollisionStay(Collision collision)
        {
            EvaluateGround(collision);
            DamageTarget(collision.collider);
        }

        private void EvaluateGround(Collision collision)
        {
            if (movementMode != BossMovementMode.Ground)
            {
                return;
            }

            for (int i = 0; i < collision.contactCount; i++)
            {
                if (collision.GetContact(i).normal.y > 0.5f)
                {
                    grounded = true;
                    return;
                }
            }
        }

        private void DamageTarget(Collider other)
        {
            if (Time.time < nextContactTime)
            {
                return;
            }

            Damageable otherDamageable = other.GetComponentInParent<Damageable>();
            if (otherDamageable == null || otherDamageable == damageable)
            {
                return;
            }

            bool isPlayer =
                otherDamageable.GetComponent<PlayerMotor>() != null ||
                otherDamageable.GetComponent<RideChaserController>() != null ||
                otherDamageable.GetComponent<AirVehicleController>() != null;

            if (!isPlayer)
            {
                return;
            }

            nextContactTime = Time.time + contactCooldown;
            otherDamageable.TakeDamage(contactDamage);
        }
    }
}
