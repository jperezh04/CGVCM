using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Movimiento25D : MonoBehaviour
{
    public float velocidad = 6f;
    public float fuerzaSalto = 7.5f;

    private Rigidbody rb;
    private bool enSuelo = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // Movimiento 2.5D: física 3D, pero bloqueada en el eje Z.
        rb.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;
    }

    void Update()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");

        Vector3 velocidadActual = rb.linearVelocity;
        velocidadActual.x = horizontal * velocidad;
        velocidadActual.z = 0f;
        rb.linearVelocity = velocidadActual;

        if (Input.GetKeyDown(KeyCode.Space) && enSuelo)
        {
            rb.AddForce(Vector3.up * fuerzaSalto, ForceMode.Impulse);
            enSuelo = false;
        }
    }

    void OnCollisionStay(Collision collision)
    {
        foreach (ContactPoint contacto in collision.contacts)
        {
            if (Vector3.Dot(contacto.normal, Vector3.up) > 0.5f)
            {
                enSuelo = true;
                return;
            }
        }
    }

    void OnCollisionExit(Collision collision)
    {
        enSuelo = false;
    }
}
