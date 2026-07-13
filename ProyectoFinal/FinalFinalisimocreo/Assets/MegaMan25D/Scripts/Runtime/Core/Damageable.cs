using System;
using UnityEngine;
using UnityEngine.Events;

namespace MegaMan25D
{
    public sealed class Damageable : MonoBehaviour
    {
        [Min(1)] public int maxHealth = 8;
        public bool destroyOnDeath;
        public bool respawnOnDeath = true;
        public Transform respawnPoint;
        public float invulnerabilitySeconds = 0.15f;
        public UnityEvent onDamaged;
        public UnityEvent onDeath;
        public UnityEvent onRespawn;

        public event Action<Damageable> Damaged;
        public event Action<Damageable> Died;
        public event Action<Damageable> Respawned;

        public int CurrentHealth { get; private set; }
        public bool IsDead { get; private set; }
        public float Health01 => maxHealth <= 0 ? 0f : Mathf.Clamp01((float)CurrentHealth / maxHealth);

        private float nextDamageTime;
        private Vector3 initialPosition;
        private Quaternion initialRotation;

        private void Awake()
        {
            CurrentHealth = Mathf.Max(1, maxHealth);
            initialPosition = transform.position;
            initialRotation = transform.rotation;
        }

        public void TakeDamage(int amount)
        {
            if (IsDead || Time.time < nextDamageTime || amount <= 0)
            {
                return;
            }

            nextDamageTime = Time.time + invulnerabilitySeconds;
            CurrentHealth = Mathf.Max(0, CurrentHealth - amount);
            onDamaged?.Invoke();
            Damaged?.Invoke(this);

            if (CurrentHealth <= 0)
            {
                Die();
            }
        }

        public void Kill()
        {
            if (IsDead)
            {
                return;
            }

            CurrentHealth = 0;
            Die();
        }

        public void RestoreFullHealth()
        {
            IsDead = false;
            CurrentHealth = Mathf.Max(1, maxHealth);
        }

        public void SetRespawnPoint(Transform point)
        {
            respawnPoint = point;
        }

        private void Die()
        {
            if (IsDead)
            {
                return;
            }

            IsDead = true;
            onDeath?.Invoke();
            Died?.Invoke(this);

            if (destroyOnDeath)
            {
                Destroy(gameObject);
                return;
            }

            if (respawnOnDeath)
            {
                Respawn();
            }
        }

        private void Respawn()
        {
            IsDead = false;
            CurrentHealth = Mathf.Max(1, maxHealth);

            transform.SetPositionAndRotation(
                respawnPoint != null ? respawnPoint.position : initialPosition,
                respawnPoint != null ? respawnPoint.rotation : initialRotation
            );

            Rigidbody body = GetComponent<Rigidbody>();
            if (body != null)
            {
                body.linearVelocity = Vector3.zero;
                body.angularVelocity = Vector3.zero;
            }

            onRespawn?.Invoke();
            Respawned?.Invoke(this);
        }
    }
}
