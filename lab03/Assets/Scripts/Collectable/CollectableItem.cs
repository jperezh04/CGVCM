using UnityEngine;

public class CollectableItem : MonoBehaviour
{
    public enum CollectableType
    {
        Fuel,
        Health
    }

    [Header("Collectable Settings")]
    [SerializeField] private CollectableType type;
    [SerializeField] private float amount = 25f;

    [Header("Audio")]
    [SerializeField] private AudioClip pickupSound;

    [Header("Animation")]
    [SerializeField] private Animator animator;
    [SerializeField] private string pickupTrigger = "Pickup";

    [Header("Despawn")]
    [SerializeField] private float destroyDelay = 0.2f;

    private AudioSource audioSource;
    private bool collected = false;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (collected) return;

        SpaceshipController ship = other.GetComponent<SpaceshipController>();
        if (ship == null || !ship.IsAlive()) return;

        ApplyEffect(ship);
        PlayFeedback();
        collected = true;

        // Desactivar colisión y visual inmediatamente
        GetComponent<Collider>().enabled = false;
        GetComponentInChildren<Renderer>().enabled = false;

        Destroy(gameObject, destroyDelay);
    }

    private void ApplyEffect(SpaceshipController ship)
    {
        switch (type)
        {
            case CollectableType.Fuel:
                ship.RefillFuel(amount);
                break;

            case CollectableType.Health:
                ship.Heal(amount);
                break;
        }
    }

    private void PlayFeedback()
    {
        if (animator != null)
        {
            animator.SetTrigger(pickupTrigger);
        }

        if (audioSource != null && pickupSound != null)
        {
            audioSource.PlayOneShot(pickupSound);
        }
    }
}
