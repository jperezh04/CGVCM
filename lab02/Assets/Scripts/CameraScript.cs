using UnityEngine;

public class CameraScript : MonoBehaviour
{
    public GameObject John;
    void Update()
    {
        if (John == null) return;
        Vector3 position = transform.position;
        position.x = John.transform.position.x;
        transform.position = position;  
    }
}
