using UnityEngine;

namespace MegaMan25D
{
    [RequireComponent(typeof(Rigidbody))]
    public sealed class Projectile : MonoBehaviour
    {
        public float lifetime = 2.5f;
        public int damage = 1;

        private Transform ownerRoot;
        private Rigidbody body;

        private void Awake()
        {
            body = GetComponent<Rigidbody>();
        }

        public void Launch(Transform owner, Vector3 velocity, int damageAmount)
        {
            ownerRoot = owner != null ? owner.root : null;
            damage = Mathf.Max(1, damageAmount);
            body.linearVelocity = velocity;
            Destroy(gameObject, Mathf.Max(0.1f, lifetime));
        }

        private void OnTriggerEnter(Collider other)
        {
            if (ownerRoot != null && other.transform.root == ownerRoot)
            {
                return;
            }

            Damageable damageable = other.GetComponentInParent<Damageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(damage);
            }

            Destroy(gameObject);
        }
    }
}
