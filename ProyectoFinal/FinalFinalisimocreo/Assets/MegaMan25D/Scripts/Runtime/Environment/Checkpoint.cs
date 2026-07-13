using UnityEngine;

namespace MegaMan25D
{
    [RequireComponent(typeof(Collider))]
    public sealed class Checkpoint : MonoBehaviour
    {
        public Transform respawnAnchor;

        private void Reset()
        {
            Collider trigger = GetComponent<Collider>();
            trigger.isTrigger = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            Damageable damageable = other.GetComponentInParent<Damageable>();
            if (damageable != null)
            {
                damageable.SetRespawnPoint(respawnAnchor != null ? respawnAnchor : transform);
            }
        }
    }
}
