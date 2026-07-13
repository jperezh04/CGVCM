using UnityEngine;

namespace MegaMan25D
{
    public sealed class EnemyTurret : MonoBehaviour
    {
        public Transform target;
        public Transform muzzle;
        public Projectile projectilePrefab;
        public float detectionRange = 12f;
        public float fireInterval = 1.25f;
        public float projectileSpeed = 11f;
        public int damage = 1;

        private float nextFireTime;

        private void Update()
        {
            if (target == null || projectilePrefab == null || muzzle == null)
            {
                return;
            }

            Vector3 difference = target.position - muzzle.position;
            if (difference.sqrMagnitude > detectionRange * detectionRange)
            {
                return;
            }

            if (Time.time < nextFireTime)
            {
                return;
            }

            nextFireTime = Time.time + Mathf.Max(0.15f, fireInterval);
            Vector3 direction = difference.normalized;
            Projectile projectile = Instantiate(projectilePrefab, muzzle.position, Quaternion.identity);
            projectile.Launch(transform, direction * projectileSpeed, damage);
        }
    }
}
