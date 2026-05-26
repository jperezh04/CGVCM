using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ObjetoPerspectiva : MonoBehaviour
{
    [Header("Escala")]
    public float velocidadEscala = 1.2f;
    public float escalaMinima = 0.35f;
    public float escalaMaxima = 3.5f;

    [Header("Movimiento con mouse")]
    public float velocidadProfundidad = 4f;
    public float distanciaMinimaCamara = 2f;
    public float distanciaMaximaCamara = 18f;

    private Camera camara;
    private bool seleccionado = false;

    private float distanciaConCamara = 5f;

    // Escala que se ve mientras estás agarrando el objeto.
    private Vector3 escalaVisible;

    // Escala que se prepara con W/S, pero no se muestra hasta soltar.
    private Vector3 escalaPendiente;

    private Renderer renderizador;
    private Color colorOriginal;

    void Start()
    {
        camara = Camera.main;
        renderizador = GetComponent<Renderer>();

        if (renderizador != null)
        {
            colorOriginal = renderizador.material.color;
        }
    }

    void Update()
    {
        if (!seleccionado || camara == null)
        {
            return;
        }

        MoverConMouse();
        PrepararEscalaSinMostrar();

        if (Input.GetMouseButtonUp(0))
        {
            SoltarObjeto();
        }
    }

    private void MoverConMouse()
    {
        /*
         * Movimiento con mouse:
         * - El mouse mueve el objeto en la vista de la cámara.
         * - La rueda del mouse lo acerca o aleja, o sea, cambia la profundidad.
         */

        float rueda = Input.mouseScrollDelta.y;

        if (Mathf.Abs(rueda) > 0.001f)
        {
            distanciaConCamara -= rueda * velocidadProfundidad * Time.deltaTime * 10f;
            distanciaConCamara = Mathf.Clamp(
                distanciaConCamara,
                distanciaMinimaCamara,
                distanciaMaximaCamara
            );
        }

        Ray rayo = camara.ScreenPointToRay(Input.mousePosition);
        transform.position = rayo.GetPoint(distanciaConCamara);
    }

    private void PrepararEscalaSinMostrar()
    {
        float cambioEscala = 0f;

        if (Input.GetKey(KeyCode.W))
        {
            cambioEscala += velocidadEscala * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.S))
        {
            cambioEscala -= velocidadEscala * Time.deltaTime;
        }

        if (Mathf.Abs(cambioEscala) > 0.001f)
        {
            float nuevaEscala = Mathf.Clamp(
                escalaPendiente.x + cambioEscala,
                escalaMinima,
                escalaMaxima
            );

            escalaPendiente = Vector3.one * nuevaEscala;

            // Mientras se mantiene agarrado, se conserva el tamaño visible.
            transform.localScale = escalaVisible;
        }
    }

    void OnMouseDown()
    {
        camara = Camera.main;

        if (camara == null)
        {
            return;
        }

        seleccionado = true;

        distanciaConCamara = Vector3.Distance(
            camara.transform.position,
            transform.position
        );

        escalaVisible = transform.localScale;
        escalaPendiente = transform.localScale;

        if (renderizador != null)
        {
            renderizador.material.color = Color.yellow;
        }
    }

    private void SoltarObjeto()
    {
        seleccionado = false;

        // Recién aquí se muestra el cambio real de tamaño.
        transform.localScale = escalaPendiente;

        if (renderizador != null)
        {
            renderizador.material.color = colorOriginal;
        }
    }

    void OnMouseUp()
    {
        if (seleccionado)
        {
            SoltarObjeto();
        }
    }
}