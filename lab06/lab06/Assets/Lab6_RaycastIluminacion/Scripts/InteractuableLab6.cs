using UnityEngine;

public class InteractuableLab6 : MonoBehaviour
{
    public enum TipoAccion
    {
        AlternarLuz,
        AlternarObjeto,
        CambiarColorLuz,
        AplicarImpulso,
        TeletransportarJugador
    }

    [Header("Mensaje")]
    public string mensajeMirar = "Presiona E para interactuar";

    [Header("Acción")]
    public TipoAccion accion = TipoAccion.AlternarLuz;

    [Header("Luz")]
    public Light luzObjetivo;
    public Color colorA = Color.white;
    public Color colorB = Color.red;
    public float intensidadA = 2f;
    public float intensidadB = 0.4f;

    [Header("Objeto")]
    public GameObject objetoObjetivo;

    [Header("Física")]
    public Rigidbody rigidbodyObjetivo;
    public float fuerzaImpulso = 6f;

    [Header("Teletransporte")]
    public Transform puntoDestino;

    private bool estado = false;

    public void Interactuar(GameObject origen)
    {
        estado = !estado;

        switch (accion)
        {
            case TipoAccion.AlternarLuz:
                if (luzObjetivo != null)
                    luzObjetivo.enabled = !luzObjetivo.enabled;
                break;

            case TipoAccion.AlternarObjeto:
                if (objetoObjetivo != null)
                    objetoObjetivo.SetActive(!objetoObjetivo.activeSelf);
                break;

            case TipoAccion.CambiarColorLuz:
                if (luzObjetivo != null)
                {
                    luzObjetivo.color = estado ? colorA : colorB;
                    luzObjetivo.intensity = estado ? intensidadA : intensidadB;
                }
                break;

            case TipoAccion.AplicarImpulso:
                if (rigidbodyObjetivo != null)
                {
                    Vector3 direccion = (rigidbodyObjetivo.transform.position - transform.position).normalized;
                    direccion.y = 0.45f;
                    rigidbodyObjetivo.AddForce(direccion * fuerzaImpulso, ForceMode.Impulse);
                }
                break;

            case TipoAccion.TeletransportarJugador:
                if (puntoDestino != null && origen != null)
                {
                    CharacterController cc = origen.GetComponentInParent<CharacterController>();

                    if (cc != null)
                        cc.enabled = false;

                    origen.transform.root.position = puntoDestino.position;

                    if (cc != null)
                        cc.enabled = true;
                }
                break;
        }
    }
}
