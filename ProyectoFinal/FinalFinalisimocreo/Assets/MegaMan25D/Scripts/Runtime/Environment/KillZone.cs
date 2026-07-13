using UnityEngine;

namespace MegaMan25D
{
    [RequireComponent(typeof(Collider))]
    public sealed class KillZone : MonoBehaviour
    {
        private void Reset()
        {
            Collider zone = GetComponent<Collider>();
            zone.isTrigger = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            Damageable damageable = other.GetComponentInParent<Damageable>();
            if (damageable != null)
            {
                damageable.Kill();
            }
        }
    }
}
