using UnityEngine;

public class CambioPerspectiva : MonoBehaviour
{
    public Camera camaraPerspectiva;
    public Camera camaraIsometrica;

    [Header("Objetos que dependen de la perspectiva")]
    public GameObject[] objetosSoloIsometrica;
    public GameObject[] objetosSoloPerspectiva;

    private bool usandoIsometrica = false;

    void Start()
    {
        AplicarCambio();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            usandoIsometrica = !usandoIsometrica;
            AplicarCambio();
        }
    }

    private void AplicarCambio()
    {
        if (camaraPerspectiva != null)
        {
            camaraPerspectiva.enabled = !usandoIsometrica;
            camaraPerspectiva.tag = usandoIsometrica ? "Untagged" : "MainCamera";
        }

        if (camaraIsometrica != null)
        {
            camaraIsometrica.enabled = usandoIsometrica;
            camaraIsometrica.tag = usandoIsometrica ? "MainCamera" : "Untagged";
        }

        foreach (GameObject obj in objetosSoloIsometrica)
        {
            if (obj != null)
                obj.SetActive(usandoIsometrica);
        }

        foreach (GameObject obj in objetosSoloPerspectiva)
        {
            if (obj != null)
                obj.SetActive(!usandoIsometrica);
        }

        Debug.Log(usandoIsometrica ? "Vista isométrica activada" : "Vista en perspectiva activada");
    }
}
