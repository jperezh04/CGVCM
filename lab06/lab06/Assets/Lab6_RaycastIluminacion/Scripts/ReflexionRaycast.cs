using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(LineRenderer))]
public class ReflexionRaycast : MonoBehaviour
{
    public Transform origen;
    public float distanciaMaxima = 25f;
    public int rebotes = 3;
    public LayerMask capas = ~0;

    private LineRenderer linea;

    void Start()
    {
        linea = GetComponent<LineRenderer>();

        if (origen == null)
            origen = transform;

        linea.positionCount = 0;
        linea.startWidth = 0.05f;
        linea.endWidth = 0.05f;
    }

    void Update()
    {
        DibujarRaycastConReflexion();
    }

    private void DibujarRaycastConReflexion()
    {
        if (linea == null || origen == null)
            return;

        List<Vector3> puntos = new List<Vector3>();

        Vector3 posicionActual = origen.position;
        Vector3 direccionActual = origen.forward;

        puntos.Add(posicionActual);

        for (int i = 0; i < rebotes; i++)
        {
            RaycastHit hit;

            if (Physics.Raycast(posicionActual, direccionActual, out hit, distanciaMaxima, capas))
            {
                puntos.Add(hit.point);

                // Reflexión básica: simula el rebote de un rayo sobre una superficie.
                direccionActual = Vector3.Reflect(direccionActual, hit.normal);
                posicionActual = hit.point + direccionActual * 0.03f;
            }
            else
            {
                puntos.Add(posicionActual + direccionActual * distanciaMaxima);
                break;
            }
        }

        linea.positionCount = puntos.Count;
        linea.SetPositions(puntos.ToArray());
    }
}
