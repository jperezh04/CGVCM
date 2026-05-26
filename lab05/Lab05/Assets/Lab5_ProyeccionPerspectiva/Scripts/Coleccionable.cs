using UnityEngine;

public class Coleccionable : MonoBehaviour
{
    public float velocidadRotacion = 90f;

    void Update()
    {
        transform.Rotate(Vector3.up * velocidadRotacion * Time.deltaTime, Space.World);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Coleccionable obtenido: " + gameObject.name);
            Destroy(gameObject);
        }
    }
}
