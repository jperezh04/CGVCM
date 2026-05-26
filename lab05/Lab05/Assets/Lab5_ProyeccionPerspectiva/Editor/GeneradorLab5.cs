#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.IO;

public class GeneradorLab5
{
    private const string Root = "Assets/Lab5_ProyeccionPerspectiva";

    [MenuItem("Tools/Laboratorio 5/Generar escenas")]
    public static void GenerarEscenas()
    {
        CrearCarpetas();

        Material matPiso = CrearMaterial("MAT_Piso", new Color(0.28f, 0.28f, 0.28f));
        Material matAzul = CrearMaterial("MAT_Azul", new Color(0.15f, 0.35f, 0.85f));
        Material matRojo = CrearMaterial("MAT_Rojo", new Color(0.85f, 0.2f, 0.18f));
        Material matVerde = CrearMaterial("MAT_Verde", new Color(0.2f, 0.7f, 0.35f));
        Material matAmarillo = CrearMaterial("MAT_Amarillo", new Color(1f, 0.8f, 0.15f));
        Material matMorado = CrearMaterial("MAT_Morado", new Color(0.55f, 0.25f, 0.8f));
        Material matBlanco = CrearMaterial("MAT_Blanco", new Color(0.9f, 0.9f, 0.9f));
        Material matNegro = CrearMaterial("MAT_Negro", new Color(0.08f, 0.08f, 0.08f));

        CrearEscena1(matPiso, matAzul, matRojo, matVerde, matAmarillo, matBlanco);
        CrearEscena2(matPiso, matAzul, matRojo, matVerde, matAmarillo, matMorado);
        CrearEscena3(matPiso, matAzul, matRojo, matVerde, matAmarillo, matMorado, matNegro);

        ConfigurarBuildSettings();

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("Laboratorio 5 generado correctamente. Abre las escenas en Assets/Lab5_ProyeccionPerspectiva/Scenes.");
    }

    private static void CrearCarpetas()
    {
        CrearCarpetaSiNoExiste(Root);
        CrearCarpetaSiNoExiste(Root + "/Scenes");
        CrearCarpetaSiNoExiste(Root + "/Materials");
    }

    private static void CrearCarpetaSiNoExiste(string ruta)
    {
        if (!AssetDatabase.IsValidFolder(ruta))
        {
            string padre = Path.GetDirectoryName(ruta).Replace("\\", "/");
            string nombre = Path.GetFileName(ruta);
            AssetDatabase.CreateFolder(padre, nombre);
        }
    }

    private static Material CrearMaterial(string nombre, Color color)
    {
        string ruta = Root + "/Materials/" + nombre + ".mat";
        Material mat = AssetDatabase.LoadAssetAtPath<Material>(ruta);

        Shader shader = Shader.Find("Universal Render Pipeline/Lit");
        if (shader == null) shader = Shader.Find("Standard");
        if (shader == null) shader = Shader.Find("Unlit/Color");
        if (shader == null) shader = Shader.Find("Sprites/Default");

        if (mat == null)
        {
            mat = new Material(shader);
            AssetDatabase.CreateAsset(mat, ruta);
        }
        else if (shader != null)
        {
            mat.shader = shader;
        }

        if (mat.HasProperty("_BaseColor"))
            mat.SetColor("_BaseColor", color);

        if (mat.HasProperty("_Color"))
            mat.SetColor("_Color", color);

        mat.color = color;
        EditorUtility.SetDirty(mat);
        return mat;
    }

    private static void AplicarMaterial(GameObject obj, Material mat)
    {
        Renderer r = obj.GetComponent<Renderer>();
        if (r != null && mat != null)
            r.sharedMaterial = mat;
    }

    private static void CrearEscena1(Material matPiso, Material matAzul, Material matRojo, Material matVerde, Material matAmarillo, Material matBlanco)
    {
        Scene escena = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        RenderSettings.skybox = null;
        RenderSettings.ambientLight = new Color(0.55f, 0.55f, 0.55f);

        CrearLuzDireccional();

        Camera cam = CrearCamara("Camara_Perspectiva", new Vector3(0f, 4f, -10f), new Vector3(0f, 1f, 4f), false, 55f);
        cam.tag = "MainCamera";

        Cubo("Piso", new Vector3(0f, -0.1f, 4f), new Vector3(12f, 0.2f, 12f), matPiso);
        Cubo("Pared_Fondo", new Vector3(0f, 2.5f, 10f), new Vector3(12f, 5f, 0.2f), matBlanco);

        Cubo("Referencia_Cubo_Cercano", new Vector3(-3f, 0.5f, 2f), Vector3.one, matAzul);
        Cubo("Referencia_Cubo_Medio", new Vector3(3f, 0.5f, 5f), Vector3.one, matVerde);
        Cubo("Referencia_Cubo_Lejano", new Vector3(-2f, 0.5f, 8f), Vector3.one, matRojo);

        GameObject esfera = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        esfera.name = "Objeto_Interactivo_Perspectiva";
        esfera.transform.position = new Vector3(0f, 1f, 4f);
        esfera.transform.localScale = Vector3.one;
        AplicarMaterial(esfera, matAmarillo);
        esfera.AddComponent<ObjetoPerspectiva>();

        CrearTexto("Instrucciones_Escena1",
            "ESCENA 1 - Perspectiva forzada\nClic en la esfera | W agranda | S reduce\nAl soltarla se aprecia su tamaño real.",
            new Vector3(0f, 4.5f, 1.6f),
            new Vector3(20f, 0f, 0f),
            0.08f,
            Color.black);

        EditorSceneManager.SaveScene(escena, Root + "/Scenes/Escena1_Perspectiva.unity");
    }

    private static void CrearEscena2(Material matPiso, Material matAzul, Material matRojo, Material matVerde, Material matAmarillo, Material matMorado)
    {
        Scene escena = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        RenderSettings.skybox = null;
        RenderSettings.ambientLight = new Color(0.55f, 0.55f, 0.55f);

        CrearLuzDireccional();

        Camera cam = CrearCamara("Camara_Lateral_25D", new Vector3(2f, 4f, -12f), new Vector3(2f, 1f, 0f), true, 60f);
        cam.orthographicSize = 6.2f;
        cam.tag = "MainCamera";

        Cubo("Plataforma_Principal", new Vector3(0f, -0.25f, 0f), new Vector3(13f, 0.5f, 1.2f), matPiso);
        Cubo("Plataforma_1", new Vector3(-2.5f, 1.2f, 0f), new Vector3(2.2f, 0.35f, 1.2f), matAzul);
        Cubo("Plataforma_2", new Vector3(1.5f, 2.2f, 0f), new Vector3(2.2f, 0.35f, 1.2f), matVerde);
        Cubo("Plataforma_3", new Vector3(5.0f, 1.4f, 0f), new Vector3(2.2f, 0.35f, 1.2f), matMorado);

        GameObject jugador = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        jugador.name = "Jugador_25D";
        jugador.tag = "Player";
        jugador.transform.position = new Vector3(-5f, 1.1f, 0f);
        jugador.transform.localScale = new Vector3(0.7f, 1f, 0.7f);
        AplicarMaterial(jugador, matAmarillo);
        Rigidbody rb = jugador.AddComponent<Rigidbody>();
        rb.mass = 1f;
        jugador.AddComponent<Movimiento25D>();

        CrearColeccionable("Coleccionable_1", new Vector3(-2.5f, 2f, 0f), matRojo);
        CrearColeccionable("Coleccionable_2", new Vector3(1.5f, 3f, 0f), matRojo);
        CrearColeccionable("Coleccionable_3", new Vector3(5f, 2.2f, 0f), matRojo);

        CrearTexto("Instrucciones_Escena2",
            "ESCENA 2 - 2.5D\nA/D o flechas para moverse | Espacio para saltar\nFisica 3D restringida al plano X/Y.",
            new Vector3(0f, 5.0f, 0.05f),
            new Vector3(0f, 0f, 0f),
            0.07f,
            Color.black);

        EditorSceneManager.SaveScene(escena, Root + "/Scenes/Escena2_25D.unity");
    }

    private static void CrearEscena3(Material matPiso, Material matAzul, Material matRojo, Material matVerde, Material matAmarillo, Material matMorado, Material matNegro)
    {
        Scene escena = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        RenderSettings.skybox = null;
        RenderSettings.ambientLight = new Color(0.55f, 0.55f, 0.55f);

        CrearLuzDireccional();

        Camera camPerspectiva = CrearCamara("Camara_Perspectiva", new Vector3(0f, 4.5f, -8f), new Vector3(0f, 1f, 2f), false, 60f);
        camPerspectiva.tag = "MainCamera";

        Camera camIsometrica = CrearCamara("Camara_Isometrica", new Vector3(7f, 8f, -7f), new Vector3(0f, 0.8f, 1.5f), true, 60f);
        camIsometrica.orthographicSize = 7f;
        camIsometrica.enabled = false;
        camIsometrica.tag = "Untagged";

        Cubo("Piso", new Vector3(0f, -0.1f, 2f), new Vector3(11f, 0.2f, 11f), matPiso);

        Cubo("Bloque_Alto_1", new Vector3(-3f, 1f, 2f), new Vector3(1f, 2f, 1f), matAzul);
        Cubo("Bloque_Alto_2", new Vector3(3f, 1f, 2f), new Vector3(1f, 2f, 1f), matMorado);
        Cubo("Plataforma_Meta", new Vector3(0f, 0.35f, 6f), new Vector3(2f, 0.3f, 2f), matVerde);

        GameObject puente = Cubo("Puente_visible_solo_en_isometrica", new Vector3(0f, 0.45f, 4f), new Vector3(5f, 0.25f, 0.8f), matVerde);

        GameObject cartelPerspectiva = CrearTexto("Cartel_Perspectiva",
            "Vista perspectiva activa\nPresiona C para cambiar.",
            new Vector3(0f, 3.8f, -0.7f),
            new Vector3(25f, 0f, 0f),
            0.07f,
            Color.black);

        GameObject cartelIsometrico = CrearTexto("Cartel_Isometrico",
            "Vista isometrica activa\nEl puente aparece desde esta perspectiva.",
            new Vector3(0f, 3.8f, -0.7f),
            new Vector3(25f, 0f, 0f),
            0.07f,
            Color.black);

        GameObject jugador = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        jugador.name = "Jugador_3D";
        jugador.tag = "Player";
        jugador.transform.position = new Vector3(0f, 1.1f, -2.5f);
        jugador.transform.localScale = new Vector3(0.7f, 1f, 0.7f);
        AplicarMaterial(jugador, matAmarillo);
        Rigidbody rb = jugador.AddComponent<Rigidbody>();
        rb.mass = 1f;
        jugador.AddComponent<MovimientoJugador3D>();

        GameObject puerta = Cubo("Puerta_Bloqueada", new Vector3(0f, 1f, 5.8f), new Vector3(2f, 2f, 0.25f), matRojo);

        GameObject interruptor = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        interruptor.name = "Interruptor_E";
        interruptor.transform.position = new Vector3(-3f, 0.25f, -1f);
        interruptor.transform.localScale = new Vector3(0.7f, 0.25f, 0.7f);
        AplicarMaterial(interruptor, matNegro);
        Collider col = interruptor.GetComponent<Collider>();
        col.isTrigger = true;

        ActivadorSimple activador = interruptor.AddComponent<ActivadorSimple>();
        activador.objetoADesactivar = puerta;
        activador.mensaje = "Puerta desbloqueada. Ahora puedes avanzar hacia la meta.";

        GameObject gestor = new GameObject("Gestor_Cambio_Perspectiva");
        CambioPerspectiva cambio = gestor.AddComponent<CambioPerspectiva>();
        cambio.camaraPerspectiva = camPerspectiva;
        cambio.camaraIsometrica = camIsometrica;
        cambio.objetosSoloIsometrica = new GameObject[] { puente, cartelIsometrico };
        cambio.objetosSoloPerspectiva = new GameObject[] { cartelPerspectiva };

        EditorSceneManager.SaveScene(escena, Root + "/Scenes/Escena3_CambioPerspectiva.unity");
    }

    private static GameObject Cubo(string nombre, Vector3 posicion, Vector3 escala, Material mat)
    {
        GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
        obj.name = nombre;
        obj.transform.position = posicion;
        obj.transform.localScale = escala;
        AplicarMaterial(obj, mat);
        return obj;
    }

    private static void CrearColeccionable(string nombre, Vector3 posicion, Material mat)
    {
        GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        obj.name = nombre;
        obj.transform.position = posicion;
        obj.transform.localScale = Vector3.one * 0.45f;

        AplicarMaterial(obj, mat);

        Collider col = obj.GetComponent<Collider>();
        col.isTrigger = true;

        obj.AddComponent<Coleccionable>();
    }

    private static GameObject CrearTexto(string nombre, string contenido, Vector3 posicion, Vector3 rotacionEuler, float escala, Color color)
    {
        GameObject obj = new GameObject(nombre);
        obj.transform.position = posicion;
        obj.transform.rotation = Quaternion.Euler(rotacionEuler);

        TextMesh texto = obj.AddComponent<TextMesh>();
        texto.text = contenido;
        texto.fontSize = 28;
        texto.characterSize = escala;
        texto.anchor = TextAnchor.MiddleCenter;
        texto.alignment = TextAlignment.Center;
        texto.color = color;

        return obj;
    }

    private static Camera CrearCamara(string nombre, Vector3 posicion, Vector3 mirarA, bool ortografica, float fov)
    {
        GameObject obj = new GameObject(nombre);
        Camera cam = obj.AddComponent<Camera>();

        cam.transform.position = posicion;
        cam.transform.LookAt(mirarA);
        cam.orthographic = ortografica;
        cam.fieldOfView = fov;
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = new Color(0.78f, 0.84f, 0.88f);

        return cam;
    }

    private static void CrearLuzDireccional()
    {
        GameObject luz = new GameObject("Luz_Direccional");
        Light componente = luz.AddComponent<Light>();
        componente.type = LightType.Directional;
        componente.intensity = 1.2f;
        luz.transform.rotation = Quaternion.Euler(50f, -35f, 0f);
    }

    private static void ConfigurarBuildSettings()
    {
        string[] rutas =
        {
            Root + "/Scenes/Escena1_Perspectiva.unity",
            Root + "/Scenes/Escena2_25D.unity",
            Root + "/Scenes/Escena3_CambioPerspectiva.unity"
        };

        List<EditorBuildSettingsScene> escenas = new List<EditorBuildSettingsScene>();

        foreach (string ruta in rutas)
        {
            escenas.Add(new EditorBuildSettingsScene(ruta, true));
        }

        EditorBuildSettings.scenes = escenas.ToArray();
    }
}
#endif
