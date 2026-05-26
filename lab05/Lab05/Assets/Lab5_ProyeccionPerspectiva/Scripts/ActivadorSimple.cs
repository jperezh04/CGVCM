using UnityEngine;

public class ActivadorSimple : MonoBehaviour
{
    public KeyCode teclaInteraccion = KeyCode.E;
    public GameObject objetoAActivar;
    public GameObject objetoADesactivar;
    public string mensaje = "Interacción realizada";

    private bool jugadorCerca = false;

    void Update()
    {
        if (jugadorCerca && Input.GetKeyDown(teclaInteraccion))
        {
            if (objetoAActivar != null)
                objetoAActivar.SetActive(true);

            if (objetoADesactivar != null)
                objetoADesactivar.SetActive(false);

            Debug.Log(mensaje);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            jugadorCerca = true;
            Debug.Log("Presiona " + teclaInteraccion + " para interactuar.");
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            jugadorCerca = false;
    }
}
