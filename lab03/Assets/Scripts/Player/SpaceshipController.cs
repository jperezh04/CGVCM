using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody))]
public class SpaceshipController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float thrustForce = 10f;
    [SerializeField] private float rotationSpeed = 100f;
    [SerializeField] private float verticalThrustForce = 15f;
    [SerializeField] private float maxVelocity = 20f;

    [Header("Axis Settings")]
    [SerializeField] private Vector3 forwardAxis = Vector3.right; // Eje de movimiento adelante/atrás
    [SerializeField] private Vector3 rotationAxis = Vector3.forward; // Eje de rotación (Z para 2.5D)

    [Header("Fuel Settings")]
    [SerializeField] private float maxFuel = 100f;
    [SerializeField] private float fuelConsumptionRate = 5f;
    [SerializeField] private float verticalFuelConsumption = 10f;

    [Header("Health Settings")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float damagePerCollision = 20f;

    [Header("UI References")]
    [SerializeField] private Slider fuelSlider;
    [SerializeField] private Slider healthSlider;

    [Header("Collision Tag")]
    [SerializeField] private string dangerousTag = "Dangerous";

    // Private variables
    private Rigidbody rb;
    private float currentFuel;
    private float currentHealth;
    private bool isAlive = true;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // Initialize stats
        currentFuel = maxFuel;
        currentHealth = maxHealth;

        // Update UI
        UpdateUI();
    }

    void Update()
    {
        if (!isAlive) return;

        HandleRotation();
        HandleMovement();
        LimitVelocity();
        UpdateUI();

        // Check death condition
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void HandleRotation()
    {
        float rotation = 0f;

        // W: girar hacia arriba
        if (Input.GetKey(KeyCode.W))
        {
            rotation = rotationSpeed * Time.deltaTime;
        }

        // S: girar hacia abajo
        if (Input.GetKey(KeyCode.S))
        {
            rotation = -rotationSpeed * Time.deltaTime;
        }

        // Rotar en el eje Z para 2.5D
        transform.Rotate(rotationAxis, rotation);
    }

    void HandleMovement()
    {
        if (currentFuel <= 0)
        {
            // Sin combustible, no puede moverse
            return;
        }

        bool isMoving = false;

        // D: Mover hacia adelante (derecha en dirección local)
        if (Input.GetKey(KeyCode.D))
        {
            Vector3 force = transform.TransformDirection(forwardAxis) * thrustForce;
            rb.AddForce(force);
            isMoving = true;
        }

        // A: Mover hacia atrás (izquierda en dirección local)
        if (Input.GetKey(KeyCode.A))
        {
            Vector3 force = transform.TransformDirection(forwardAxis) * -thrustForce;
            rb.AddForce(force);
            isMoving = true;
        }

        // Space: Impulso vertical hacia arriba
        if (Input.GetKey(KeyCode.Space))
        {
            rb.AddForce(Vector3.up * verticalThrustForce);
            ConsumeFuel(verticalFuelConsumption * Time.deltaTime);
            isMoving = true;
        }

        // Consumir combustible si hay movimiento horizontal
        if (isMoving && !Input.GetKey(KeyCode.Space))
        {
            ConsumeFuel(fuelConsumptionRate * Time.deltaTime);
        }
    }

    void LimitVelocity()
    {
        if (rb.linearVelocity.magnitude > maxVelocity)
        {
            rb.linearVelocity = rb.linearVelocity.normalized * maxVelocity;
        }
    }

    void ConsumeFuel(float amount)
    {
        currentFuel = Mathf.Max(0, currentFuel - amount);
    }

    void TakeDamage(float damage)
    {
        currentHealth = Mathf.Max(0, currentHealth - damage);
    }

    void UpdateUI()
    {
        if (fuelSlider != null)
        {
            fuelSlider.value = currentFuel / maxFuel;
        }

        if (healthSlider != null)
        {
            healthSlider.value = currentHealth / maxHealth;
        }
    }

    void Die()
    {
        isAlive = false;
        Debug.Log("Nave destruida!");
        // Aquí puedes agregar efectos de muerte, game over, etc.
        // Ejemplo: Destroy(gameObject);
        if (gameOverMenu != null)
        {
            gameOverMenu.TriggerGameOver();
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag(dangerousTag))
        {
            TakeDamage(damagePerCollision);
            Debug.Log($"¡Colisión peligrosa! Vida restante: {currentHealth}");
        }
    }

    [Header("Box Limites")]
    [SerializeField] private BoxCollider levelBounds;

    private void LateUpdate()
    {
        ClampToBounds();
    }

    private void ClampToBounds()
    {
        if (levelBounds == null) return;

        Bounds b = levelBounds.bounds;
        Vector3 pos = transform.position;

        pos.x = Mathf.Clamp(pos.x, b.min.x, b.max.x);
        pos.y = Mathf.Clamp(pos.y, b.min.y, b.max.y);

        transform.position = pos;
    }

    // Método público para recargar combustible (útil para power-ups)
    public void RefillFuel(float amount)
    {
        currentFuel = Mathf.Min(maxFuel, currentFuel + amount);
    }

    // Método público para curar (útil para power-ups)
    public void Heal(float amount)
    {
        currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
    }

    // Getters públicos
    public float GetCurrentFuel() => currentFuel;
    public float GetCurrentHealth() => currentHealth;
    public bool IsAlive() => isAlive;

    [SerializeField] private GameOverMenuController gameOverMenu;

}