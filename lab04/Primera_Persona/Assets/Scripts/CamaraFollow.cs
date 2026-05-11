using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Ajustes de mouse")]
    [SerializeField] private float sensibilidad = 150f;
    [SerializeField] private Transform player;

    [Header("Posiciones de cámara")]
    [SerializeField] private Vector3 posicionPrimeraPersona = new Vector3(0f, 1.6f, 0f);
    [SerializeField] private Vector3 posicionTerceraPersona = new Vector3(0f, 2f, -4f);

    [Header("Suavizado")]
    [SerializeField] private float suavizadoCamara = 8f;

    [Header("Estado")]
    [SerializeField] private bool terceraPersona = false;

    private float rotacionVertical = 0f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (player == null)
        {
            player = transform.parent;
        }

        // Inicia en primera persona
        transform.localPosition = posicionPrimeraPersona;
    }

    void Update()
    {
        if (player == null) return;

        // Cambiar entre primera y tercera persona con TAB
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            terceraPersona = !terceraPersona;
        }

        // Movimiento del mouse
        float mouseX = Input.GetAxis("Mouse X") * sensibilidad * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * sensibilidad * Time.deltaTime;

        // Mirar arriba y abajo
        rotacionVertical -= mouseY;
        rotacionVertical = Mathf.Clamp(rotacionVertical, -80f, 80f);

        // La cámara rota en X
        transform.localRotation = Quaternion.Euler(rotacionVertical, 0f, 0f);

        // El Player rota en Y
        player.Rotate(Vector3.up * mouseX);

        // Elegir posición según el modo de cámara
        Vector3 posicionObjetivo;

        if (terceraPersona)
        {
            posicionObjetivo = posicionTerceraPersona;
        }
        else
        {
            posicionObjetivo = posicionPrimeraPersona;
        }

        // Mover la cámara suavemente
        transform.localPosition = Vector3.Lerp(
            transform.localPosition,
            posicionObjetivo,
            suavizadoCamara * Time.deltaTime
        );
    }
}