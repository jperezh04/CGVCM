using UnityEngine;

public class RotadorObjeto : MonoBehaviour
{
    public Vector3 velocidadRotacion = new Vector3(0f, 45f, 0f);

    void Update()
    {
        transform.Rotate(velocidadRotacion * Time.deltaTime);
    }
}
