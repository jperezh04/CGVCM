using UnityEngine;

/// <summary>
/// Cámara básica para prototipo 2.5D. Sigue al player en X/Y y mantiene Z fijo.
/// Luego se puede reemplazar por Cinemachine.
/// </summary>
public class SimplePlatformerCamera : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 offset = new Vector3(0f, 1.2f, -10f);
    [SerializeField] private float smoothTime = 0.12f;

    private Vector3 velocity;

    private void LateUpdate()
    {
        if (target == null)
            return;

        Vector3 desiredPosition = target.position + offset;
        transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, smoothTime);
    }
}
