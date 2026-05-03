using UnityEngine;

public class ObjetoMovible : MonoBehaviour
{
    [Header("Configuración de Detección")]
    public Transform jugador;
    public float distanciaDeteccion = 5f;
    private bool detectado = false;

    [Header("Movimiento de Emboscada")]
    public Transform puntoObjetivo; // A donde se mueve cuando te ve
    public float velocidadEmboscada = 10f;

    [Header("Movimiento Perpetuo (Post-Detección)")]
    public float amplitud = 2f; // Qué tanto se mueve
    public float frecuencia = 1f; // Qué tan rápido oscila
    private Vector3 posicionInicialPostEmboscada;

    void Update()
    {
        if (jugador == null) return;

        float distancia = Vector3.Distance(transform.position, jugador.position);

        // 1. Detectar si el jugador está cerca
        if (distancia < distanciaDeteccion)
        {
            detectado = true;
        }

        if (detectado)
        {
            AtacarOMoverse();
        }
    }

    void AtacarOMoverse()
    {
        // 2. Moverse a la ubicación específica (Emboscada)
        if (Vector3.Distance(transform.position, puntoObjetivo.position) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, puntoObjetivo.position, velocidadEmboscada * Time.deltaTime);
            posicionInicialPostEmboscada = puntoObjetivo.position;
        }
        else
        {
            // 3. Una vez en el sitio, movimiento perpetuo (Oscilación)
            float nuevoY = posicionInicialPostEmboscada.y + Mathf.Sin(Time.time * frecuencia) * amplitud;
            transform.position = new Vector3(transform.position.x, nuevoY, transform.position.z);
        }
    }

    // Dibujar el rango de detección en el Editor para facilitar el ajuste
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, distanciaDeteccion);
    }
}