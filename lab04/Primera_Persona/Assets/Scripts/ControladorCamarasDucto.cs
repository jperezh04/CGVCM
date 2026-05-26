using UnityEngine;
using Unity.Cinemachine;

public class ControladorCamarasDucto : MonoBehaviour
{
    [Header("Configuración de Cámaras")]
    public CinemachineCamera camaraDucto; 
    public CinemachineCamera camaraPrincipal;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Entrando al ducto: Cambiando a Primera Persona");
            
            camaraDucto.Priority = 20;
            camaraPrincipal.Priority = 10;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Saliendo del ducto: Volviendo a Tercera Persona");
            
            camaraDucto.Priority = 5;
            camaraPrincipal.Priority = 10;
        }
    }
}