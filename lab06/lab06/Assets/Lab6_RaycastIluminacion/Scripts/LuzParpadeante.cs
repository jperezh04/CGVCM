using UnityEngine;

[RequireComponent(typeof(Light))]
public class LuzParpadeante : MonoBehaviour
{
    public float intensidadMinima = 0.2f;
    public float intensidadMaxima = 2.5f;
    public float velocidad = 8f;
    public bool usarRuido = true;

    private Light luz;
    private float semilla;

    void Start()
    {
        luz = GetComponent<Light>();
        semilla = Random.Range(0f, 100f);
    }

    void Update()
    {
        if (luz == null)
            return;

        float valor;

        if (usarRuido)
            valor = Mathf.PerlinNoise(Time.time * velocidad, semilla);
        else
            valor = Mathf.Abs(Mathf.Sin(Time.time * velocidad));

        luz.intensity = Mathf.Lerp(intensidadMinima, intensidadMaxima, valor);
    }
}
