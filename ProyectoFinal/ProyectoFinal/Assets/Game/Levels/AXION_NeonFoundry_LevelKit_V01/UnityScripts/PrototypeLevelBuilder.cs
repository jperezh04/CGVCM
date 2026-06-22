using UnityEngine;

/// <summary>
/// Genera una sala prototipo 2.5D con plataformas, paredes, hazards, checkpoint y puerta de jefe.
/// Está pensado para un player con Rigidbody2D + BoxCollider2D moviéndose en X/Y.
/// Colócalo en un GameObject vacío llamado LevelPrototype y presiona Play.
/// </summary>
public class PrototypeLevelBuilder : MonoBehaviour
{
    [Header("General")]
    [SerializeField] private Transform levelRoot;
    [SerializeField] private float depth = 1.0f;

    [Header("Materials")]
    [SerializeField] private Material floorMaterial;
    [SerializeField] private Material wallMaterial;
    [SerializeField] private Material energyMaterial;
    [SerializeField] private Material hazardMaterial;
    [SerializeField] private Material backgroundMaterial;

    private void Awake()
    {
        if (levelRoot == null)
            levelRoot = transform;

        BuildTestRoom();
    }

    private void BuildTestRoom()
    {
        // Suelo principal
        CreateSolid("Ground_Main", new Vector2(0f, -1f), new Vector2(16f, 1f), floorMaterial);

        // Paredes laterales para probar wall slide / wall jump
        CreateSolid("Wall_Left", new Vector2(-7.5f, 1.5f), new Vector2(1f, 6f), wallMaterial);
        CreateSolid("Wall_Right", new Vector2(7.5f, 1.5f), new Vector2(1f, 6f), wallMaterial);

        // Plataformas de prueba: salto, dash y doble salto
        CreateSolid("Platform_Short_Left", new Vector2(-3.5f, 1.0f), new Vector2(3f, 0.4f), floorMaterial);
        CreateSolid("Platform_Mid_Right", new Vector2(2.3f, 2.2f), new Vector2(3.5f, 0.4f), floorMaterial);
        CreateSolid("Platform_High", new Vector2(-1.0f, 3.5f), new Vector2(2.5f, 0.4f), floorMaterial);

        // Zona de peligro para probar knockback / daño
        CreateHazard("Spike_Test", new Vector2(0.2f, -0.35f), new Vector2(2.3f, 0.35f));

        // Checkpoint visual
        CreateVisualCube("Checkpoint_Beacon", new Vector3(-6f, 0.35f, 0f), new Vector3(0.35f, 1.7f, depth), energyMaterial);

        // Puerta de jefe visual
        CreateVisualCube("Boss_Door_Frame", new Vector3(6.55f, 1.05f, 0f), new Vector3(0.55f, 3.3f, depth), wallMaterial);
        CreateVisualCube("Boss_Door_Energy", new Vector3(6.45f, 1.05f, -0.05f), new Vector3(0.12f, 2.6f, depth + 0.05f), energyMaterial);

        // Fondo 2.5D: paneles detrás del gameplay
        CreateBackgroundPanel("BG_Panel_01", new Vector3(-3f, 1.5f, 1.6f), new Vector3(4f, 2.5f, 0.1f));
        CreateBackgroundPanel("BG_Panel_02", new Vector3(2.5f, 1.3f, 1.7f), new Vector3(5f, 2.0f, 0.1f));
    }

    private GameObject CreateSolid(string objectName, Vector2 center, Vector2 size, Material material)
    {
        GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
        obj.name = objectName;
        obj.transform.SetParent(levelRoot);
        obj.transform.position = new Vector3(center.x, center.y, 0f);
        obj.transform.localScale = new Vector3(size.x, size.y, depth);

        if (material != null)
            obj.GetComponent<MeshRenderer>().material = material;

        // El collider 3D de Unity no sirve para Rigidbody2D. Lo quitamos y usamos BoxCollider2D.
        Destroy(obj.GetComponent<BoxCollider>());
        BoxCollider2D collider = obj.AddComponent<BoxCollider2D>();
        collider.size = Vector2.one;
        collider.usedByComposite = false;

        obj.layer = LayerMask.NameToLayer("Ground");
        return obj;
    }

    private GameObject CreateHazard(string objectName, Vector2 center, Vector2 size)
    {
        GameObject obj = CreateSolid(objectName, center, size, hazardMaterial);
        obj.layer = LayerMask.NameToLayer("Hazard");

        // Para un prototipo, trigger es mejor: detecta daño sin bloquear movimiento.
        BoxCollider2D collider = obj.GetComponent<BoxCollider2D>();
        collider.isTrigger = true;

        return obj;
    }

    private GameObject CreateVisualCube(string objectName, Vector3 center, Vector3 size, Material material)
    {
        GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
        obj.name = objectName;
        obj.transform.SetParent(levelRoot);
        obj.transform.position = center;
        obj.transform.localScale = size;

        if (material != null)
            obj.GetComponent<MeshRenderer>().material = material;

        Destroy(obj.GetComponent<BoxCollider>());
        return obj;
    }

    private GameObject CreateBackgroundPanel(string objectName, Vector3 center, Vector3 size)
    {
        GameObject obj = CreateVisualCube(objectName, center, size, backgroundMaterial);
        return obj;
    }
}
