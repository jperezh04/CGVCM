using UnityEngine;

namespace MegaMan25D
{
    public sealed class WeaponController : MonoBehaviour
    {
        public Projectile projectilePrefab;
        public Transform muzzle;
        public float projectileSpeed = 20f;
        public float fireCooldown = 0.16f;
        public int damage = 1;
        public bool alwaysShootRight;

        private float nextFireTime;

        private void Update()
        {
            if (!GameInput.FireHeld || Time.time < nextFireTime)
            {
                return;
            }

            Fire();
        }

        public void Fire()
        {
            if (projectilePrefab == null || muzzle == null)
            {
                return;
            }

            nextFireTime = Time.time + Mathf.Max(0.02f, fireCooldown);
            float sign = ResolveFacingSign();
            Projectile shot = Instantiate(
                projectilePrefab,
                muzzle.position,
                Quaternion.identity
            );

            shot.Launch(transform, Vector3.right * sign * projectileSpeed, damage);
        }

        private float ResolveFacingSign()
        {
            if (alwaysShootRight)
            {
                return 1f;
            }

            PlayerMotor player = GetComponent<PlayerMotor>();
            if (player != null)
            {
                return player.FacingSign;
            }

            return transform.lossyScale.x < 0f ? -1f : 1f;
        }
    }
}
