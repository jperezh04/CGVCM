using UnityEngine;

public class CursorInitializer : MonoBehaviour
{
    [SerializeField] private bool showCursor = true;
    [SerializeField] private CursorLockMode lockMode = CursorLockMode.None;

    private void Awake()
    {
        Cursor.lockState = lockMode;
        Cursor.visible = showCursor;
    }
}
