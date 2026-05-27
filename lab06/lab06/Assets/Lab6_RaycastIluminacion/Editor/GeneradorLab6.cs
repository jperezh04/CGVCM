#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.IO;

public class GeneradorLab6
{
    private const string Root = "Assets/Lab6_RaycastIluminacion";

    [MenuItem("Tools/Laboratorio 6/Generar habitaciones")]
    public static void GenerarHabitaciones()
    {
        CrearCarpetas();

        Material matPiso = CrearMaterial("MAT_Piso", new Color(0.22f, 0.22f, 0.22f));
        Material matPared = CrearMaterial("MAT_Pared", new Color(0.78f, 0.76f, 0.70f));
        Material matTecho = CrearMaterial("MAT_Techo", new Color(0.68f, 0.68f, 0.68f));
        Material matComodidad = CrearMaterial("MAT_Comodidad", new Color(0.95f, 0.70f, 0.38f));
        Material matTristeza = CrearMaterial("MAT_Tristeza", new Color(0.20f, 0.32f, 0.70f));
        Material matMiedo = CrearMaterial("MAT_Miedo", new Color(0.45f, 0.05f, 0.05f));
        Material matCalma = CrearMaterial("MAT_Calma", new Color(0.25f, 0.70f, 0.55f));
        Material matNegro = CrearMaterial("MAT_Negro", new Color(0.05f, 0.05f, 0.05f));
        Material matBlanco = CrearMaterial("MAT_Blanco", new Color(0.92f, 0.92f, 0.90f));
        Material matRojo = CrearMaterial("MAT_Rojo", new Color(0.90f, 0.10f, 0.05f));
        Material matAzul = CrearMaterial("MAT_Azul", new Color(0.15f, 0.35f, 1f));
        Material matVerde = CrearMaterial("MAT_Verde", new Color(0.10f, 0.85f, 0.35f));
        Material matAmarillo = CrearMaterial("MAT_Amarillo", new Color(1f, 0.85f, 0.10f));

        Material matLinea = CrearMaterial("MAT_Linea_Raycast", new Color(1f, 0.05f, 0.03f));
        ConfigurarMaterialEmisivo(matLinea, new Color(1f, 0.05f, 0.03f), 2.5f);

        Scene escena = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        RenderSettings.skybox = null;
        RenderSettings.ambientLight = new Color(0.06f, 0.06f, 0.07f);

        CrearLuzDireccionalSuave();

        GameObject jugador = CrearJugador();

        CrearHabitacion("Habitacion_Comodidad", new Vector3(0f, 0f, 0f), true, true, matPiso, matPared, matTecho);
        CrearHabitacion("Habitacion_Tristeza", new Vector3(0f, 0f, 8f), true, true, matPiso, matPared, matTecho);
        CrearHabitacion("Habitacion_Miedo", new Vector3(0f, 0f, 16f), true, true, matPiso, matPared, matTecho);
        CrearHabitacion("Habitacion_Calma", new Vector3(0f, 0f, 24f), true, false, matPiso, matPared, matTecho);

        CrearComodidad(new Vector3(0f, 0f, 0f), matComodidad, matBlanco, matNegro, matAmarillo);
        CrearTristeza(new Vector3(0f, 0f, 8f), matTristeza, matAzul, matBlanco, matNegro);
        CrearMiedo(new Vector3(0f, 0f, 16f), matMiedo, matRojo, matNegro, matAmarillo, matLinea);
        CrearCalma(new Vector3(0f, 0f, 24f), matCalma, matVerde, matBlanco, matNegro);

        CrearTextoMundo(
            "Cartel_Inicial",
            "LAB 6 - Raycast, iluminacion y fisica\nWASD: mover | Mouse: mirar | E: interactuar | Shift: correr\nBusca botones, lamparas y objetos interactivos.",
            new Vector3(0f, 2.7f, -3.6f),
            new Vector3(18f, 0f, 0f),
            0.09f,
            Color.black
        );

        EditorSceneManager.SaveScene(escena, Root + "/Scenes/Lab6_Habitaciones_Iluminacion.unity");

        EditorBuildSettings.scenes = new EditorBuildSettingsScene[]
        {
            new EditorBuildSettingsScene(Root + "/Scenes/Lab6_Habitaciones_Iluminacion.unity", true)
        };

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("Laboratorio 6 generado correctamente en Assets/Lab6_RaycastIluminacion/Scenes.");
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

    private static void ConfigurarMaterialEmisivo(Material mat, Color color, float intensidad)
    {
        if (mat == null)
            return;

        Color emisivo = color * intensidad;

        if (mat.HasProperty("_EmissionColor"))
        {
            mat.EnableKeyword("_EMISSION");
            mat.SetColor("_EmissionColor", emisivo);
        }
    }

    private static void AplicarMaterial(GameObject obj, Material mat)
    {
        Renderer r = obj.GetComponent<Renderer>();

        if (r != null && mat != null)
            r.sharedMaterial = mat;
    }

    private static GameObject CrearJugador()
    {
        GameObject jugador = new GameObject("Jugador_CharacterController");
        jugador.tag = "Player";
        jugador.transform.position = new Vector3(0f, 1.1f, -2.5f);

        CharacterController cc = jugador.AddComponent<CharacterController>();
        cc.height = 1.8f;
        cc.radius = 0.35f;
        cc.center = new Vector3(0f, 0.9f, 0f);

        GameObject camObj = new GameObject("Camara_Jugador");
        camObj.transform.SetParent(jugador.transform);
        camObj.transform.localPosition = new Vector3(0f, 1.65f, 0f);
        camObj.transform.localRotation = Quaternion.identity;

        Camera cam = camObj.AddComponent<Camera>();
        cam.tag = "MainCamera";
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = new Color(0.08f, 0.08f, 0.09f);
        cam.fieldOfView = 65f;

        camObj.AddComponent<AudioListener>();

        ControladorPrimeraPersona controlador = jugador.AddComponent<ControladorPrimeraPersona>();
        controlador.camaraJugador = cam;

        InteraccionRaycast interaccion = camObj.AddComponent<InteraccionRaycast>();
        interaccion.camara = cam;
        interaccion.distanciaInteraccion = 4f;

        GameObject linternaObj = new GameObject("Linterna_Jugador");
        linternaObj.transform.SetParent(camObj.transform);
        linternaObj.transform.localPosition = new Vector3(0f, -0.1f, 0.25f);
        linternaObj.transform.localRotation = Quaternion.identity;

        Light linterna = linternaObj.AddComponent<Light>();
        linterna.type = LightType.Spot;
        linterna.range = 12f;
        linterna.spotAngle = 38f;
        linterna.intensity = 0f;
        linterna.color = new Color(0.95f, 0.95f, 0.78f);

        return jugador;
    }

    private static void CrearHabitacion(string nombre, Vector3 centro, bool puertaFrontal, bool puertaPosterior, Material matPiso, Material matPared, Material matTecho)
    {
        GameObject contenedor = new GameObject(nombre);

        float size = 8f;
        float altura = 4f;
        float grosor = 0.2f;
        float puertaAncho = 2.2f;
        float puertaAlto = 2.6f;

        GameObject piso = Cubo("Piso_" + nombre, centro + new Vector3(0f, -0.1f, 0f), new Vector3(size, 0.2f, size), matPiso);
        piso.transform.SetParent(contenedor.transform);

        GameObject techo = Cubo("Techo_" + nombre, centro + new Vector3(0f, altura, 0f), new Vector3(size, 0.2f, size), matTecho);
        techo.transform.SetParent(contenedor.transform);

        GameObject izq = Cubo("Pared_Izquierda_" + nombre, centro + new Vector3(-size/2f, altura/2f, 0f), new Vector3(grosor, altura, size), matPared);
        izq.transform.SetParent(contenedor.transform);

        GameObject der = Cubo("Pared_Derecha_" + nombre, centro + new Vector3(size/2f, altura/2f, 0f), new Vector3(grosor, altura, size), matPared);
        der.transform.SetParent(contenedor.transform);

        CrearParedConPuerta("Pared_Frontal_" + nombre, centro + new Vector3(0f, 0f, -size/2f), true, puertaFrontal, size, altura, grosor, puertaAncho, puertaAlto, matPared, contenedor.transform);
        CrearParedConPuerta("Pared_Posterior_" + nombre, centro + new Vector3(0f, 0f, size/2f), false, puertaPosterior, size, altura, grosor, puertaAncho, puertaAlto, matPared, contenedor.transform);
    }

    private static void CrearParedConPuerta(string nombre, Vector3 basePos, bool frontal, bool conPuerta, float size, float altura, float grosor, float puertaAncho, float puertaAlto, Material mat, Transform padre)
    {
        float z = basePos.z;
        float rot = 0f;

        if (!conPuerta)
        {
            GameObject pared = Cubo(nombre, new Vector3(basePos.x, altura/2f, z), new Vector3(size, altura, grosor), mat);
            pared.transform.SetParent(padre);
            return;
        }

        float lateralAncho = (size - puertaAncho) / 2f;

        GameObject izq = Cubo(nombre + "_LadoIzq", new Vector3(basePos.x - (puertaAncho/2f + lateralAncho/2f), altura/2f, z), new Vector3(lateralAncho, altura, grosor), mat);
        GameObject der = Cubo(nombre + "_LadoDer", new Vector3(basePos.x + (puertaAncho/2f + lateralAncho/2f), altura/2f, z), new Vector3(lateralAncho, altura, grosor), mat);
        GameObject top = Cubo(nombre + "_Superior", new Vector3(basePos.x, puertaAlto + (altura - puertaAlto)/2f, z), new Vector3(puertaAncho, altura - puertaAlto, grosor), mat);

        izq.transform.SetParent(padre);
        der.transform.SetParent(padre);
        top.transform.SetParent(padre);
    }

    private static void CrearComodidad(Vector3 c, Material matComodidad, Material matBlanco, Material matNegro, Material matAmarillo)
    {
        Light luzCentral = CrearLuz("Luz_Comodidad_Central", c + new Vector3(0f, 3.4f, 0f), LightType.Point, new Color(1f, 0.72f, 0.42f), 2.8f, 8f);

        Cubo("Sofa_Comodidad_Base", c + new Vector3(-2.3f, 0.35f, 1.3f), new Vector3(2.5f, 0.7f, 0.9f), matComodidad);
        Cubo("Sofa_Comodidad_Respaldo", c + new Vector3(-2.3f, 0.95f, 1.75f), new Vector3(2.5f, 1.0f, 0.25f), matComodidad);
        Cubo("Mesa_Comodidad", c + new Vector3(0.8f, 0.35f, 1.2f), new Vector3(1.5f, 0.25f, 1f), matBlanco);
        Cubo("Alfombra_Comodidad", c + new Vector3(0f, 0.02f, 0f), new Vector3(3f, 0.04f, 2.5f), matComodidad);

        Light lampara = CrearLuz("Lampara_Comodidad", c + new Vector3(2.8f, 1.5f, -1.7f), LightType.Point, new Color(1f, 0.82f, 0.50f), 4.2f, 5f);
        GameObject baseLampara = Cubo("Base_Lampara_Comodidad", c + new Vector3(2.8f, 0.6f, -1.7f), new Vector3(0.25f, 1.2f, 0.25f), matNegro);
        GameObject pantalla = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        pantalla.name = "Pantalla_Lampara_Comodidad";
        pantalla.transform.position = c + new Vector3(2.8f, 1.5f, -1.7f);
        pantalla.transform.localScale = new Vector3(0.8f, 0.45f, 0.8f);
        AplicarMaterial(pantalla, matAmarillo);

        GameObject boton = CrearBoton("Boton_Lampara_Comodidad", c + new Vector3(3.85f, 1.3f, -0.8f), matAmarillo);
        InteractuableLab6 inter = boton.AddComponent<InteractuableLab6>();
        inter.accion = InteractuableLab6.TipoAccion.AlternarLuz;
        inter.luzObjetivo = lampara;
        inter.mensajeMirar = "Presiona E para prender/apagar la lampara calida";

        CrearTextoMundo("Texto_Comodidad",
            "Habitacion: Comodidad\nLuz calida + baja intensidad + objetos familiares.",
            c + new Vector3(0f, 2.4f, -3.7f),
            new Vector3(16f, 0f, 0f),
            0.07f,
            Color.black);
    }

    private static void CrearTristeza(Vector3 c, Material matTristeza, Material matAzul, Material matBlanco, Material matNegro)
    {
        Light luzAzul = CrearLuz("Luz_Tristeza_Azul", c + new Vector3(0f, 3.1f, -1.5f), LightType.Point, new Color(0.25f, 0.42f, 1f), 2.0f, 7f);

        Cubo("Cama_Tristeza_Base", c + new Vector3(-2.2f, 0.35f, 1.2f), new Vector3(2.5f, 0.45f, 1.5f), matTristeza);
        Cubo("Almohada_Tristeza", c + new Vector3(-3.0f, 0.75f, 1.2f), new Vector3(0.55f, 0.28f, 1.2f), matBlanco);
        Cubo("Ventana_Tristeza", c + new Vector3(0f, 2.2f, 3.88f), new Vector3(2.2f, 1.2f, 0.08f), matAzul);

        for (int i = 0; i < 9; i++)
        {
            float x = -1.8f + i * 0.45f;
            GameObject lluvia = Cubo("Linea_Lluvia_" + i, c + new Vector3(x, 2.2f, 3.77f), new Vector3(0.03f, 1.1f, 0.03f), matAzul);
            lluvia.transform.rotation = Quaternion.Euler(0f, 0f, -15f);
        }

        GameObject boton = CrearBoton("Boton_Color_Tristeza", c + new Vector3(3.85f, 1.3f, 7.2f - 8f), matAzul);
        boton.transform.position = c + new Vector3(3.85f, 1.3f, -0.8f);
        InteractuableLab6 inter = boton.AddComponent<InteractuableLab6>();
        inter.accion = InteractuableLab6.TipoAccion.CambiarColorLuz;
        inter.luzObjetivo = luzAzul;
        inter.colorA = new Color(0.1f, 0.2f, 0.8f);
        inter.colorB = new Color(0.45f, 0.45f, 0.65f);
        inter.intensidadA = 2.2f;
        inter.intensidadB = 0.5f;
        inter.mensajeMirar = "Presiona E para cambiar la intensidad emocional de la luz";

        CrearTextoMundo("Texto_Tristeza",
            "Habitacion: Tristeza\nTonos azules, poca luz y sombras suaves.",
            c + new Vector3(0f, 2.4f, -3.7f),
            new Vector3(16f, 0f, 0f),
            0.07f,
            Color.black);
    }

    private static void CrearMiedo(Vector3 c, Material matMiedo, Material matRojo, Material matNegro, Material matAmarillo, Material matLinea)
    {
        Light luzRoja = CrearLuz("Luz_Miedo_Parpadeante", c + new Vector3(0f, 3.0f, 0f), LightType.Point, new Color(1f, 0.05f, 0.03f), 2.5f, 8f);
        luzRoja.gameObject.AddComponent<LuzParpadeante>();

        Cubo("Caja_Miedo_1", c + new Vector3(-2.5f, 0.5f, 1.2f), new Vector3(1f, 1f, 1f), matNegro);
        Cubo("Caja_Miedo_2", c + new Vector3(2.5f, 0.5f, -1.3f), new Vector3(1f, 1f, 1f), matMiedo);

        GameObject cajaFisica = Cubo("Caja_Fisica_Empujable", c + new Vector3(0f, 0.55f, 1.8f), new Vector3(0.9f, 0.9f, 0.9f), matRojo);
        Rigidbody rb = cajaFisica.AddComponent<Rigidbody>();
        rb.mass = 1.5f;

        GameObject botonCaja = CrearBoton("Boton_Raycast_Empujar_Caja", c + new Vector3(-3.85f, 1.3f, 0.5f), matRojo);
        InteractuableLab6 interCaja = botonCaja.AddComponent<InteractuableLab6>();
        interCaja.accion = InteractuableLab6.TipoAccion.AplicarImpulso;
        interCaja.rigidbodyObjetivo = rb;
        interCaja.fuerzaImpulso = 7f;
        interCaja.mensajeMirar = "Presiona E para empujar la caja usando raycast + fisica";

        GameObject laser = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        laser.name = "Emisor_Raycast_Reflexivo";
        laser.transform.position = c + new Vector3(-3.2f, 1.1f, -2.7f);
        laser.transform.rotation = Quaternion.Euler(90f, 25f, 0f);
        laser.transform.localScale = new Vector3(0.12f, 0.6f, 0.12f);
        AplicarMaterial(laser, matAmarillo);

        GameObject espejo = Cubo("Superficie_Reflexiva_Simulada", c + new Vector3(2.7f, 1.2f, 1.2f), new Vector3(0.15f, 2f, 2f), matNegro);
        espejo.transform.rotation = Quaternion.Euler(0f, -25f, 0f);

        LineRenderer lr = laser.AddComponent<LineRenderer>();
        lr.sharedMaterial = matLinea;
        lr.startWidth = 0.05f;
        lr.endWidth = 0.05f;
        ReflexionRaycast reflexion = laser.AddComponent<ReflexionRaycast>();
        reflexion.origen = laser.transform;
        reflexion.rebotes = 3;
        reflexion.distanciaMaxima = 18f;

        CrearTextoMundo("Texto_Miedo",
            "Habitacion: Miedo\nLuz roja parpadeante, sombras fuertes,\nraycast visible y caja con fisica.",
            c + new Vector3(0f, 2.4f, -3.7f),
            new Vector3(16f, 0f, 0f),
            0.07f,
            Color.white);
    }

    private static void CrearCalma(Vector3 c, Material matCalma, Material matVerde, Material matBlanco, Material matNegro)
    {
        Light luzSuave = CrearLuz("Luz_Calma_Suave", c + new Vector3(0f, 3.2f, 0f), LightType.Point, new Color(0.55f, 1f, 0.75f), 1.6f, 7f);

        Cubo("Mesa_Calma", c + new Vector3(0f, 0.35f, 0.5f), new Vector3(2f, 0.25f, 1.2f), matBlanco);

        GameObject esfera = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        esfera.name = "Cristal_Calma_Interactivo";
        esfera.transform.position = c + new Vector3(0f, 1.1f, 0.5f);
        esfera.transform.localScale = Vector3.one * 0.6f;
        AplicarMaterial(esfera, matVerde);
        esfera.AddComponent<RotadorObjeto>();

        Light luzCristal = CrearLuz("Luz_Cristal_Calma", esfera.transform.position, LightType.Point, new Color(0.35f, 1f, 0.70f), 0f, 5f);

        GameObject boton = CrearBoton("Boton_Cristal_Calma", c + new Vector3(3.85f, 1.3f, -0.8f), matVerde);
        InteractuableLab6 inter = boton.AddComponent<InteractuableLab6>();
        inter.accion = InteractuableLab6.TipoAccion.CambiarColorLuz;
        inter.luzObjetivo = luzCristal;
        inter.colorA = new Color(0.35f, 1f, 0.70f);
        inter.colorB = new Color(0.1f, 0.25f, 0.15f);
        inter.intensidadA = 3.5f;
        inter.intensidadB = 0f;
        inter.mensajeMirar = "Presiona E para activar la luz del cristal";

        CrearTextoMundo("Texto_Calma",
            "Habitacion: Calma\nIluminacion verde suave, objeto interactivo y ambiente estable.",
            c + new Vector3(0f, 2.4f, -3.7f),
            new Vector3(16f, 0f, 0f),
            0.07f,
            Color.black);
    }

    private static GameObject CrearBoton(string nombre, Vector3 posicion, Material mat)
    {
        GameObject boton = Cubo(nombre, posicion, new Vector3(0.45f, 0.25f, 0.12f), mat);
        boton.transform.rotation = Quaternion.Euler(0f, 90f, 0f);
        return boton;
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

    private static Light CrearLuz(string nombre, Vector3 posicion, LightType tipo, Color color, float intensidad, float rango)
    {
        GameObject obj = new GameObject(nombre);
        obj.transform.position = posicion;

        Light luz = obj.AddComponent<Light>();
        luz.type = tipo;
        luz.color = color;
        luz.intensity = intensidad;
        luz.range = rango;

        if (tipo == LightType.Spot)
            luz.spotAngle = 45f;

        return luz;
    }

    private static void CrearLuzDireccionalSuave()
    {
        GameObject obj = new GameObject("Luz_Direccional_Suave");
        Light luz = obj.AddComponent<Light>();
        luz.type = LightType.Directional;
        luz.color = new Color(0.7f, 0.75f, 0.8f);
        luz.intensity = 0.18f;
        obj.transform.rotation = Quaternion.Euler(45f, -35f, 0f);
    }

    private static GameObject CrearTextoMundo(string nombre, string contenido, Vector3 posicion, Vector3 rotacionEuler, float escala, Color color)
    {
        GameObject obj = new GameObject(nombre);
        obj.transform.position = posicion;
        obj.transform.rotation = Quaternion.Euler(rotacionEuler);

        TextMesh texto = obj.AddComponent<TextMesh>();
        texto.text = contenido;
        texto.fontSize = 32;
        texto.characterSize = escala;
        texto.anchor = TextAnchor.MiddleCenter;
        texto.alignment = TextAlignment.Center;
        texto.color = color;

        return obj;
    }
}
#endif
