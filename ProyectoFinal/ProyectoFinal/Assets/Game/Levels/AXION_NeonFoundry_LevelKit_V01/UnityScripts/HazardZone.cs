using UnityEngine;

/// <summary>
/// Script simple para hazards. Requiere que el objeto tenga Collider2D con IsTrigger activado.
/// Más adelante lo conectamos con PlayerHealth y knockback real.
/// </summary>
public class HazardZone : MonoBehaviour
{
    [SerializeField] private int damage = 1;
    [SerializeField] private Vector2 knockback = new Vector2(8f, 6f);

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        Debug.Log($"Player recibió {damage} de daño. Knockback sugerido: {knockback}");
    }
}
