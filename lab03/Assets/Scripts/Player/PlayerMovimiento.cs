using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovimiento : MonoBehaviour
{
    [Header("Movimiento")]
    public float velocidadMaxima = 60f;
    public float aceleracion = 25f;
    public float friccion = 10f;
    public float velocidadGiro = 90f;

    [Header("Inclinación visual")]
    public Transform modeloNave; // Asignar el hijo ModeloNave
    public float rollMax = 30f;
    public float pitchMax = 15f;
    public float suavizado = 6f;

    [Header("Altura fija")]
    public bool usarAlturaFija = true;
    private float alturaInicial;

    private Rigidbody rb;
    private float velocidad;
    private float rollActual;
    private float pitchActual;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.isKinematic = false; // Permitimos física real
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        // Solo rotaremos manualmente sobre Y, física manejará colisiones

        alturaInicial = transform.position.y;
    }

    void Update()
    {
        ControlVelocidad();
        RotacionVisual();
    }

    void FixedUpdate()
    {
        // Dirección horizontal plana
        Vector3 direccionPlana = new Vector3(transform.forward.x, 0f, transform.forward.z).normalized;

        // Movimiento usando Rigidbody para respetar colisiones
        Vector3 movimiento = direccionPlana * velocidad * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + movimiento);

        // Mantener altura relativa al inicio si está activada
        if (usarAlturaFija)
        {
            Vector3 pos = rb.position;
            rb.MovePosition(new Vector3(pos.x, alturaInicial, pos.z));
        }

        // Limitar velocidad máxima
        velocidad = Mathf.Clamp(velocidad, -velocidadMaxima * 0.3f, velocidadMaxima);
    }

    void ControlVelocidad()
    {
        if (Input.GetKey(KeyCode.W))
            velocidad += aceleracion * Time.deltaTime;
        else if (Input.GetKey(KeyCode.S))
            velocidad -= aceleracion * Time.deltaTime;
        else
            velocidad = Mathf.MoveTowards(velocidad, 0, friccion * Time.deltaTime);
    }

    void RotacionVisual()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        // Giro real (YAW) del Player
        transform.Rotate(0f, h * velocidadGiro * Time.deltaTime, 0f, Space.World);

        // Giro visual del ModeloNave
        if (modeloNave != null)
        {
            float rollObjetivo = -h * rollMax;
            float pitchObjetivo = -v * pitchMax;

            rollActual = Mathf.Lerp(rollActual, rollObjetivo, Time.deltaTime * suavizado);
            pitchActual = Mathf.Lerp(pitchActual, pitchObjetivo, Time.deltaTime * suavizado);

            modeloNave.localRotation = Quaternion.Euler(pitchActual, 0f, rollActual);
        }
    }
}