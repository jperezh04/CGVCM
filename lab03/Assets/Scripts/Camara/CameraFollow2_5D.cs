using UnityEngine;

public class CameraFollow2_5D : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform player;

    [Header("Limits (X Axis)")]
    [SerializeField] private float minX;
    [SerializeField] private float maxX;

    [Header("Limits (Y Axis)")]
    [SerializeField] private float minY;
    [SerializeField] private float maxY;

    [Header("Camera Settings")]
    [SerializeField] private float fixedZ = -10f;
    [SerializeField] private float smoothSpeed = 5f;

    [Header("Camera Tilt (X Rotation)")]
    [SerializeField] private float minTiltX = -8f;
    [SerializeField] private float maxTiltX = 8f;
    [SerializeField] private float rotationSmoothSpeed = 4f;

    private Quaternion initialRotation;

    private void Awake()
    {
        initialRotation = transform.rotation;
    }
    private void LateUpdate()
    {
        if (player == null) return;

        HandlePosition();
        HandleRotation();
    }

    private void HandlePosition()
    {
        float targetX = Mathf.Clamp(player.position.x, minX, maxX);
        float targetY = Mathf.Clamp(player.position.y, minY, maxY);

        Vector3 targetPosition = new Vector3(
            targetX,
            targetY,
            fixedZ
        );

        transform.position = Vector3.Lerp(
            transform.position,
            targetPosition,
            smoothSpeed * Time.deltaTime
        );
    }

    private void HandleRotation()
    {
        // Normaliza la altura del jugador (0 ? 1)
        float heightNormalized = Mathf.InverseLerp(minY, maxY, player.position.y);

        // Calcula la inclinación deseada en X
        float targetTiltX = Mathf.Lerp(minTiltX, maxTiltX, heightNormalized);

        Quaternion targetRotation = Quaternion.Euler(
            targetTiltX,
            initialRotation.eulerAngles.y,
            initialRotation.eulerAngles.z
        );

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            rotationSmoothSpeed * Time.deltaTime
        );
    }
}