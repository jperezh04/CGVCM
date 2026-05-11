using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Ajustes")]
    [SerializeField] private float sensibilidad = 100f;
    [SerializeField] private Transform player;

    [Header("Estados")]
    [SerializeField] private float rotacionVertical = 0f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        float valorX = Input.GetAxis("Mouse X") * sensibilidad * Time.deltaTime;
        float valorY = Input.GetAxis("Mouse Y") * sensibilidad * Time.deltaTime;

        rotacionVertical -= valorY;
        rotacionVertical = Mathf.Clamp(rotacionVertical, -80f, 80f);

        // La cámara mira arriba y abajo
        transform.localRotation = Quaternion.Euler(rotacionVertical, 0f, 0f);

        // El jugador gira izquierda y derecha
        if (player != null)
        {
            player.Rotate(Vector3.up * valorX);
        }
        else
        {
            Debug.LogWarning("CameraFollow: asigna el Player en el Inspector.");
        }
    }
}