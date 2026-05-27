using UnityEngine;

public class InteraccionRaycast : MonoBehaviour
{
    public Camera camara;
    public float distanciaInteraccion = 4f;
    public KeyCode teclaInteraccion = KeyCode.E;

    private string mensajeActual = "";

    void Start()
    {
        if (camara == null)
            camara = GetComponent<Camera>();

        if (camara == null)
            camara = Camera.main;
    }

    void Update()
    {
        mensajeActual = "";

        if (camara == null)
            return;

        Ray rayo = camara.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        RaycastHit hit;

        if (Physics.Raycast(rayo, out hit, distanciaInteraccion))
        {
            InteractuableLab6 interactuable = hit.collider.GetComponentInParent<InteractuableLab6>();

            if (interactuable != null)
            {
                mensajeActual = interactuable.mensajeMirar;

                if (Input.GetKeyDown(teclaInteraccion))
                {
                    interactuable.Interactuar(gameObject);
                }
            }
        }
    }

    void OnGUI()
    {
        GUIStyle estiloCentro = new GUIStyle(GUI.skin.label);
        estiloCentro.alignment = TextAnchor.MiddleCenter;
        estiloCentro.fontSize = 22;
        estiloCentro.normal.textColor = Color.white;

        GUI.Label(
            new Rect(Screen.width / 2 - 10, Screen.height / 2 - 10, 20, 20),
            "+",
            estiloCentro
        );

        if (!string.IsNullOrEmpty(mensajeActual))
        {
            GUIStyle estiloMensaje = new GUIStyle(GUI.skin.label);
            estiloMensaje.alignment = TextAnchor.MiddleCenter;
            estiloMensaje.fontSize = 18;
            estiloMensaje.normal.textColor = Color.white;

            GUI.Label(
                new Rect(Screen.width / 2 - 260, Screen.height - 90, 520, 40),
                mensajeActual,
                estiloMensaje
            );
        }
    }
}
