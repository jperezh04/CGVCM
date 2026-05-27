using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class ControladorPrimeraPersona : MonoBehaviour
{
    [Header("Movimiento")]
    public float velocidad = 4.5f;
    public float velocidadCorrer = 7f;
    public float gravedad = -18f;
    public float fuerzaSalto = 6f;

    [Header("Mouse")]
    public Camera camaraJugador;
    public float sensibilidadMouse = 2f;
    public float limiteVertical = 80f;

    private CharacterController controller;
    private Vector3 velocidadVertical;
    private float rotacionX = 0f;

    void Start()
    {
        controller = GetComponent<CharacterController>();

        if (camaraJugador == null)
            camaraJugador = GetComponentInChildren<Camera>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        MoverJugador();
        MoverCamara();

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        if (Input.GetMouseButtonDown(0))
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    private void MoverJugador()
    {
        bool enSuelo = controller.isGrounded;

        if (enSuelo && velocidadVertical.y < 0f)
            velocidadVertical.y = -2f;

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        float velocidadActual = Input.GetKey(KeyCode.LeftShift) ? velocidadCorrer : velocidad;

        Vector3 movimiento = transform.right * x + transform.forward * z;
        controller.Move(movimiento * velocidadActual * Time.deltaTime);

        if (Input.GetKeyDown(KeyCode.Space) && enSuelo)
            velocidadVertical.y = Mathf.Sqrt(fuerzaSalto * -2f * gravedad);

        velocidadVertical.y += gravedad * Time.deltaTime;
        controller.Move(velocidadVertical * Time.deltaTime);
    }

    private void MoverCamara()
    {
        if (camaraJugador == null)
            return;

        float mouseX = Input.GetAxis("Mouse X") * sensibilidadMouse;
        float mouseY = Input.GetAxis("Mouse Y") * sensibilidadMouse;

        rotacionX -= mouseY;
        rotacionX = Mathf.Clamp(rotacionX, -limiteVertical, limiteVertical);

        camaraJugador.transform.localRotation = Quaternion.Euler(rotacionX, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }
}
