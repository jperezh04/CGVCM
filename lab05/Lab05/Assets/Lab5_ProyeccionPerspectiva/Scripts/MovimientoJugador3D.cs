using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class MovimientoJugador3D : MonoBehaviour
{
    public float velocidad = 5f;
    public float fuerzaSalto = 7f;

    private Rigidbody rb;
    private bool enSuelo = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation;
    }

    void Update()
    {
        Camera camara = Camera.main;

        Vector3 adelante = Vector3.forward;
        Vector3 derecha = Vector3.right;

        if (camara != null)
        {
            adelante = camara.transform.forward;
            derecha = camara.transform.right;

            adelante.y = 0f;
            derecha.y = 0f;

            adelante.Normalize();
            derecha.Normalize();
        }

        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        Vector3 direccion = (derecha * h + adelante * v).normalized;
        Vector3 velocidadNueva = direccion * velocidad;
        velocidadNueva.y = rb.linearVelocity.y;

        rb.linearVelocity = velocidadNueva;

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
