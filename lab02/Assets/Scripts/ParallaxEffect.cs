using UnityEngine;

public class ParallaxEffect : MonoBehaviour
{
    public Transform cameraTransform;
    public float parallaxFactor; // qué tan rápido se mueve

    private Vector3 lastCameraPosition;

    void Start()
    {
        lastCameraPosition = cameraTransform.position;
    }

    void LateUpdate()
    {
        Vector3 deltaMovement = cameraTransform.position - lastCameraPosition;

        // Solo movemos en X (para juegos 2D horizontales)
        transform.position += new Vector3(deltaMovement.x * parallaxFactor, 0, 0);

        lastCameraPosition = cameraTransform.position;
    }
}