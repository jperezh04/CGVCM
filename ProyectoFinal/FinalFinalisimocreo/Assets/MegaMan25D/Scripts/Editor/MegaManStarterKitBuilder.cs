using System;
using System.IO;
using System.Linq;
using MegaMan25D;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

[InitializeOnLoad]
public static class MegaManCompletePackageAutoInstaller
{
    private const string MarkerPath =
        "Assets/MegaMan25D/Generated_CompletePackage/INSTALL_COMPLETE_FULL.txt";
    private const string SessionKey = "MegaMan25D_CompletePackage_Offered";

    static MegaManCompletePackageAutoInstaller()
    {
        EditorApplication.delayCall += OfferInstallation;
    }

    private static void OfferInstallation()
    {
        if (SessionState.GetBool(SessionKey, false))
        {
            return;
        }

        SessionState.SetBool(SessionKey, true);

        if (File.Exists(MarkerPath))
        {
            return;
        }

        bool generate = EditorUtility.DisplayDialog(
            "MegaMan 2.5D — Complete Package",
            "¿Generar las nueve stages, prefabs, campaña larga y peleas de jefe?",
            "Generar paquete completo",
            "Más tarde"
        );

        if (generate)
        {
            MegaManStarterKitBuilder.BuildAll(false, false);
        }
    }
}

public static class MegaManStarterKitBuilder
{
    private const string Root = "Assets/MegaMan25D/Generated_CompletePackage";
    private const string Materials = Root + "/Materials";
    private const string Prefabs = Root + "/Prefabs";
    private const string Scenes = Root + "/Scenes";
    private const string MarkerPath = Root + "/INSTALL_COMPLETE_FULL.txt";

    [MenuItem("Tools/MegaMan 2.5D/Generate Complete Package")]
    public static void GenerateFromMenu()
    {
        BuildAll(true, true);
    }

    [MenuItem("Tools/MegaMan 2.5D/Open Complete Package Folder")]
    public static void SelectGeneratedFolder()
    {
        UnityEngine.Object folder = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(Root);
        if (folder != null)
        {
            Selection.activeObject = folder;
            EditorGUIUtility.PingObject(folder);
        }
    }

    public static void BuildAll(bool openFirstScene, bool askBeforeOverwrite)
    {
        try
        {
            if (AssetDatabase.IsValidFolder(Root))
            {
                if (askBeforeOverwrite)
                {
                    bool overwrite = EditorUtility.DisplayDialog(
                        "Rebuild Complete Package",
                        "Esto reemplazará únicamente Generated_CompletePackage. " +
                        "Los assets guardados fuera de esa carpeta no serán eliminados.",
                        "Rebuild",
                        "Cancelar"
                    );

                    if (!overwrite)
                    {
                        return;
                    }
                }

                AssetDatabase.DeleteAsset(Root);
                AssetDatabase.Refresh();
            }

            EnsureFolder("Assets", "MegaMan25D");
            EnsureFolder("Assets/MegaMan25D", "Generated_CompletePackage");
            EnsureFolder(Root, "Materials");
            EnsureFolder(Root, "Prefabs");
            EnsureFolder(Root, "Scenes");

            Material blue = CreateMaterial(Materials + "/PlayerBlue.mat", new Color(0.04f, 0.18f, 0.88f));
            Material cyan = CreateMaterial(Materials + "/BusterCyan.mat", new Color(0.05f, 0.85f, 1f));
            Material red = CreateMaterial(Materials + "/EnemyRed.mat", new Color(0.85f, 0.06f, 0.08f));
            Material dark = CreateMaterial(Materials + "/EnvironmentDark.mat", new Color(0.18f, 0.20f, 0.27f));
            Material metal = CreateMaterial(Materials + "/FactoryMetal.mat", new Color(0.32f, 0.36f, 0.43f));
            Material yellow = CreateMaterial(Materials + "/VehicleYellow.mat", new Color(1f, 0.65f, 0.05f));
            Material green = CreateMaterial(Materials + "/CheckpointGreen.mat", new Color(0.15f, 0.9f, 0.35f));
            Material purple = CreateMaterial(Materials + "/BossPurple.mat", new Color(0.58f, 0.08f, 0.82f));
            Material orange = CreateMaterial(Materials + "/BossOrange.mat", new Color(1f, 0.28f, 0.04f));

            Projectile playerProjectile = BuildProjectilePrefab(
                "BusterProjectile",
                cyan,
                0.32f,
                2.5f
            );

            Projectile enemyProjectile = BuildProjectilePrefab(
                "EnemyProjectile",
                red,
                0.38f,
                4f
            );

            GameObject playerPrefab = BuildPlayerPrefab(blue, cyan, playerProjectile);
            GameObject ridePrefab = BuildRidePrefab(yellow, cyan, playerProjectile);
            GameObject airPrefab = BuildAirPrefab(cyan, blue, playerProjectile);
            GameObject patrolPrefab = BuildPatrolEnemyPrefab(red);
            GameObject turretPrefab = BuildTurretPrefab(red, enemyProjectile);
            GameObject flyerPrefab = BuildFlyerPrefab(red);
            GameObject groundBossPrefab = BuildBossPrefab(
                "Boss_Ground",
                BossMovementMode.Ground,
                purple,
                enemyProjectile,
                42
            );
            GameObject airBossPrefab = BuildBossPrefab(
                "Boss_Air",
                BossMovementMode.Air,
                orange,
                enemyProjectile,
                52
            );

            BuildTrainingScene(
                playerPrefab,
                patrolPrefab,
                dark,
                green
            );

            BuildIntroPlatformerScene(
                playerPrefab,
                patrolPrefab,
                turretPrefab,
                dark,
                green
            );

            BuildIntroRideScene(
                ridePrefab,
                patrolPrefab,
                turretPrefab,
                dark,
                green
            );

            BuildIntroAirScene(
                airPrefab,
                flyerPrefab,
                turretPrefab,
                dark,
                green
            );

            BuildHighwayScene(
                playerPrefab,
                patrolPrefab,
                turretPrefab,
                groundBossPrefab,
                dark,
                green
            );

            BuildFactoryScene(
                playerPrefab,
                patrolPrefab,
                turretPrefab,
                groundBossPrefab,
                metal,
                green
            );

            BuildRideScene(
                ridePrefab,
                patrolPrefab,
                turretPrefab,
                groundBossPrefab,
                dark,
                green
            );

            BuildAirScene(
                airPrefab,
                flyerPrefab,
                turretPrefab,
                airBossPrefab,
                dark,
                green
            );

            BuildBossRushScene(
                playerPrefab,
                groundBossPrefab,
                airBossPrefab,
                metal,
                green
            );

            ConfigureBuildSettings();

            File.WriteAllText(
                MarkerPath,
                "MegaMan 2.5D Complete Package generated successfully.\n" +
                DateTime.Now.ToString("u") + "\n"
            );

            AssetDatabase.ImportAsset(MarkerPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            if (openFirstScene)
            {
                EditorSceneManager.OpenScene(Scenes + "/00_TrainingStage.unity");
                EditorUtility.DisplayDialog(
                    "MegaMan 2.5D",
                    "Paquete completo generado. Se abrió 00_TrainingStage.",
                    "OK"
                );
            }
        }
        catch (Exception exception)
        {
            Debug.LogException(exception);
            EditorUtility.DisplayDialog(
                "MegaMan 2.5D — Error",
                exception.Message,
                "OK"
            );
        }
    }

    private static Projectile BuildProjectilePrefab(
        string prefabName,
        Material material,
        float visualScale,
        float lifetime)
    {
        GameObject root = new GameObject(prefabName);

        SphereCollider collider = root.AddComponent<SphereCollider>();
        collider.isTrigger = true;
        collider.radius = 0.5f;

        Rigidbody body = root.AddComponent<Rigidbody>();
        body.useGravity = false;
        body.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        body.constraints = RigidbodyConstraints.FreezePositionZ |
                           RigidbodyConstraints.FreezeRotation;

        Projectile projectile = root.AddComponent<Projectile>();
        projectile.lifetime = lifetime;
        projectile.damage = 1;

        CreatePrimitiveChild(
            PrimitiveType.Sphere,
            root.transform,
            "ProjectileVisual",
            Vector3.zero,
            Vector3.one * visualScale,
            material
        );

        GameObject prefab = SavePrefab(root, Prefabs + "/" + prefabName + ".prefab");
        return prefab.GetComponent<Projectile>();
    }

    private static GameObject BuildPlayerPrefab(
        Material blue,
        Material cyan,
        Projectile projectile)
    {
        GameObject root = CreateDynamicRoot("Player_Platformer", false, 1.2f);
        CapsuleCollider collider = root.AddComponent<CapsuleCollider>();
        collider.height = 1.8f;
        collider.radius = 0.38f;

        Damageable damageable = root.AddComponent<Damageable>();
        damageable.maxHealth = 10;
        damageable.destroyOnDeath = false;
        damageable.respawnOnDeath = true;

        Transform visualAnchor = NewChild(root.transform, "VisualAnchor");
        GameObject placeholder = new GameObject("PlaceholderVisual");
        placeholder.transform.SetParent(root.transform, false);

        CreatePrimitiveChild(
            PrimitiveType.Capsule,
            placeholder.transform,
            "Body",
            Vector3.zero,
            new Vector3(0.78f, 0.9f, 0.78f),
            blue
        );

        CreatePrimitiveChild(
            PrimitiveType.Cube,
            placeholder.transform,
            "ArmCannon",
            new Vector3(0.58f, 0.2f, 0f),
            new Vector3(0.55f, 0.22f, 0.3f),
            cyan
        );

        AssetVisualSlot slot = root.AddComponent<AssetVisualSlot>();
        slot.visualAnchor = visualAnchor;
        slot.placeholderRoot = placeholder;

        PlayerMotor motor = root.AddComponent<PlayerMotor>();
        motor.visualRoot = placeholder.transform;

        Transform muzzle = NewChild(root.transform, "Muzzle");
        muzzle.localPosition = new Vector3(0.88f, 0.2f, 0f);

        WeaponController weapon = root.AddComponent<WeaponController>();
        weapon.projectilePrefab = projectile;
        weapon.muzzle = muzzle;
        weapon.projectileSpeed = 21f;

        return SavePrefab(root, Prefabs + "/Player_Platformer.prefab");
    }

    private static GameObject BuildRidePrefab(
        Material yellow,
        Material cyan,
        Projectile projectile)
    {
        GameObject root = CreateDynamicRoot("RideChaser_Player", false, 1.4f);

        BoxCollider collider = root.AddComponent<BoxCollider>();
        collider.size = new Vector3(1.8f, 0.75f, 0.8f);

        Damageable damageable = root.AddComponent<Damageable>();
        damageable.maxHealth = 12;
        damageable.destroyOnDeath = false;
        damageable.respawnOnDeath = true;

        root.AddComponent<RideChaserController>();

        Transform visualAnchor = NewChild(root.transform, "VisualAnchor");
        GameObject placeholder = new GameObject("PlaceholderVisual");
        placeholder.transform.SetParent(root.transform, false);

        CreatePrimitiveChild(
            PrimitiveType.Cube,
            placeholder.transform,
            "BikeBody",
            Vector3.zero,
            new Vector3(1.8f, 0.55f, 0.65f),
            yellow
        );

        CreatePrimitiveChild(
            PrimitiveType.Sphere,
            placeholder.transform,
            "FrontWheel",
            new Vector3(0.65f, -0.42f, 0f),
            new Vector3(0.55f, 0.55f, 0.28f),
            cyan
        );

        CreatePrimitiveChild(
            PrimitiveType.Sphere,
            placeholder.transform,
            "RearWheel",
            new Vector3(-0.65f, -0.42f, 0f),
            new Vector3(0.55f, 0.55f, 0.28f),
            cyan
        );

        AssetVisualSlot slot = root.AddComponent<AssetVisualSlot>();
        slot.visualAnchor = visualAnchor;
        slot.placeholderRoot = placeholder;

        Transform muzzle = NewChild(root.transform, "Muzzle");
        muzzle.localPosition = new Vector3(1.15f, 0.2f, 0f);

        WeaponController weapon = root.AddComponent<WeaponController>();
        weapon.projectilePrefab = projectile;
        weapon.muzzle = muzzle;
        weapon.alwaysShootRight = true;
        weapon.projectileSpeed = 25f;

        return SavePrefab(root, Prefabs + "/RideChaser_Player.prefab");
    }

    private static GameObject BuildAirPrefab(
        Material cyan,
        Material blue,
        Projectile projectile)
    {
        GameObject root = CreateDynamicRoot("AirVehicle_Player", true, 1.1f);

        BoxCollider collider = root.AddComponent<BoxCollider>();
        collider.size = new Vector3(1.6f, 0.65f, 0.8f);

        Damageable damageable = root.AddComponent<Damageable>();
        damageable.maxHealth = 12;
        damageable.destroyOnDeath = false;
        damageable.respawnOnDeath = true;

        root.AddComponent<AirVehicleController>();

        Transform visualAnchor = NewChild(root.transform, "VisualAnchor");
        GameObject placeholder = new GameObject("PlaceholderVisual");
        placeholder.transform.SetParent(root.transform, false);

        CreatePrimitiveChild(
            PrimitiveType.Cube,
            placeholder.transform,
            "ShipBody",
            Vector3.zero,
            new Vector3(1.6f, 0.5f, 0.55f),
            blue
        );

        CreatePrimitiveChild(
            PrimitiveType.Cube,
            placeholder.transform,
            "Wing",
            new Vector3(-0.1f, 0f, 0f),
            new Vector3(0.8f, 0.18f, 1.45f),
            cyan
        );

        AssetVisualSlot slot = root.AddComponent<AssetVisualSlot>();
        slot.visualAnchor = visualAnchor;
        slot.placeholderRoot = placeholder;

        Transform muzzle = NewChild(root.transform, "Muzzle");
        muzzle.localPosition = new Vector3(1.05f, 0f, 0f);

        WeaponController weapon = root.AddComponent<WeaponController>();
        weapon.projectilePrefab = projectile;
        weapon.muzzle = muzzle;
        weapon.alwaysShootRight = true;
        weapon.projectileSpeed = 26f;

        return SavePrefab(root, Prefabs + "/AirVehicle_Player.prefab");
    }

    private static GameObject BuildPatrolEnemyPrefab(Material red)
    {
        GameObject root = CreateDynamicRoot("Enemy_Patrol", false, 1f);

        CapsuleCollider collider = root.AddComponent<CapsuleCollider>();
        collider.height = 1.5f;
        collider.radius = 0.38f;

        Damageable damageable = root.AddComponent<Damageable>();
        damageable.maxHealth = 3;
        damageable.destroyOnDeath = true;
        damageable.respawnOnDeath = false;

        EnemyPatrol patrol = root.AddComponent<EnemyPatrol>();
        patrol.speed = 2.3f;
        patrol.patrolDistance = 2.8f;

        AddVisualSlot(
            root,
            PrimitiveType.Capsule,
            "EnemyBody",
            new Vector3(0.8f, 0.75f, 0.8f),
            red
        );

        return SavePrefab(root, Prefabs + "/Enemy_Patrol.prefab");
    }

    private static GameObject BuildTurretPrefab(
        Material red,
        Projectile projectile)
    {
        GameObject root = new GameObject("Enemy_Turret");

        BoxCollider collider = root.AddComponent<BoxCollider>();
        collider.size = new Vector3(1f, 1f, 1f);

        Damageable damageable = root.AddComponent<Damageable>();
        damageable.maxHealth = 4;
        damageable.destroyOnDeath = true;
        damageable.respawnOnDeath = false;

        Transform muzzle = NewChild(root.transform, "Muzzle");
        muzzle.localPosition = new Vector3(-0.7f, 0.2f, 0f);

        EnemyTurret turret = root.AddComponent<EnemyTurret>();
        turret.muzzle = muzzle;
        turret.projectilePrefab = projectile;
        turret.fireInterval = 1.1f;

        AddVisualSlot(
            root,
            PrimitiveType.Cube,
            "TurretBody",
            new Vector3(1f, 1f, 1f),
            red
        );

        return SavePrefab(root, Prefabs + "/Enemy_Turret.prefab");
    }

    private static GameObject BuildFlyerPrefab(Material red)
    {
        GameObject root = CreateDynamicRoot("Enemy_Flyer", true, 0.8f);

        SphereCollider collider = root.AddComponent<SphereCollider>();
        collider.radius = 0.55f;

        Damageable damageable = root.AddComponent<Damageable>();
        damageable.maxHealth = 3;
        damageable.destroyOnDeath = true;
        damageable.respawnOnDeath = false;

        root.AddComponent<EnemyFlyer>();

        AddVisualSlot(
            root,
            PrimitiveType.Sphere,
            "FlyerBody",
            new Vector3(1.1f, 0.7f, 1.1f),
            red
        );

        return SavePrefab(root, Prefabs + "/Enemy_Flyer.prefab");
    }

    private static GameObject BuildBossPrefab(
        string prefabName,
        BossMovementMode movementMode,
        Material material,
        Projectile projectile,
        int health)
    {
        GameObject root = CreateDynamicRoot(
            prefabName,
            movementMode == BossMovementMode.Air,
            3f
        );

        CapsuleCollider collider = root.AddComponent<CapsuleCollider>();
        collider.height = 3f;
        collider.radius = 0.75f;

        Damageable damageable = root.AddComponent<Damageable>();
        damageable.maxHealth = health;
        damageable.destroyOnDeath = true;
        damageable.respawnOnDeath = false;
        damageable.invulnerabilitySeconds = 0.08f;

        Transform muzzle = NewChild(root.transform, "Muzzle");
        muzzle.localPosition = new Vector3(-1.05f, 0.35f, 0f);

        BossController boss = root.AddComponent<BossController>();
        boss.bossDisplayName =
            movementMode == BossMovementMode.Air ? "STORM WYVERN" : "IRON MINOTAUR";
        boss.movementMode = movementMode;
        boss.muzzle = muzzle;
        boss.projectilePrefab = projectile;
        boss.moveSpeed = movementMode == BossMovementMode.Air ? 4.8f : 4.2f;
        boss.dashSpeed = movementMode == BossMovementMode.Air ? 14f : 12f;
        boss.projectileSpeed = movementMode == BossMovementMode.Air ? 14f : 12f;

        AddVisualSlot(
            root,
            PrimitiveType.Capsule,
            "BossBody",
            new Vector3(1.45f, 1.5f, 1.45f),
            material
        );

        return SavePrefab(root, Prefabs + "/" + prefabName + ".prefab");
    }


    private static void BuildTrainingScene(
        GameObject playerPrefab,
        GameObject patrolPrefab,
        Material environment,
        Material checkpointMaterial)
    {
        Scene scene = NewScene(LevelMode.Platformer, "00 — Training Stage");
        GameObject player = InstantiatePrefab(
            playerPrefab,
            new Vector3(-8f, -0.55f, 0f)
        );

        CreateCamera(player.transform, false);
        CreateSegmentedGround(-12f, 48f, -1.8f, environment, 12f);

        CreateWorldLabel(
            "MOVE: A / D OR ARROWS",
            new Vector3(-5f, 1.5f, 0f)
        );
        CreateWorldLabel(
            "JUMP: SPACE / W / UP",
            new Vector3(5f, 2.1f, 0f)
        );
        CreateWorldLabel(
            "FIRE: J / X / CTRL / CLICK",
            new Vector3(18f, 2.1f, 0f)
        );

        CreatePlatform(
            "TrainingStep_A",
            new Vector3(5f, -0.25f, 0f),
            new Vector3(3f, 0.45f, 3f),
            environment
        );

        CreatePlatform(
            "TrainingStep_B",
            new Vector3(11f, 0.9f, 0f),
            new Vector3(3f, 0.45f, 3f),
            environment
        );

        CreateMovingPlatform(
            "TrainingMovingPlatform",
            new Vector3(25f, 0.2f, 0f),
            new Vector3(3.5f, 0.4f, 3f),
            new Vector3(0f, 3.5f, 0f),
            environment
        );

        GameObject target = InstantiatePrefab(
            patrolPrefab,
            new Vector3(18f, -0.55f, 0f)
        );
        target.name = "TrainingTarget";
        EnemyPatrol targetPatrol = target.GetComponent<EnemyPatrol>();
        targetPatrol.chaseTarget = player.transform;
        targetPatrol.speed = 0f;

        CreateCheckpoint(
            new Vector3(31f, -0.95f, 0f),
            checkpointMaterial
        );
        CreateKillZone(
            new Vector3(18f, -9f, 0f),
            new Vector3(80f, 2f, 8f)
        );

        CreateStageExit(
            "TrainingExit",
            new Vector3(45f, -0.2f, 0f),
            "01_Intro_Platformer",
            checkpointMaterial,
            false
        );

        CreateLegacyBootstrapperSuppressor();
        EditorSceneManager.SaveScene(
            scene,
            Scenes + "/00_TrainingStage.unity"
        );
    }

    private static void BuildIntroPlatformerScene(
        GameObject playerPrefab,
        GameObject patrolPrefab,
        GameObject turretPrefab,
        Material environment,
        Material checkpointMaterial)
    {
        Scene scene = NewScene(LevelMode.Platformer, "01 — Intro Platformer");
        GameObject player = InstantiatePrefab(
            playerPrefab,
            new Vector3(-8f, -0.55f, 0f)
        );

        CreateCamera(player.transform, false);
        CreateSegmentedGround(-12f, 80f, -1.8f, environment, 14f);

        for (int i = 0; i < 8; i++)
        {
            float x = 4f + i * 8f;
            float y = i % 2 == 0 ? 0.25f : 1.65f;
            CreatePlatform(
                "IntroPlatform_" + i,
                new Vector3(x, y, 0f),
                new Vector3(4f, 0.4f, 3f),
                environment
            );
        }

        PopulateGroundEnemies(
            player.transform,
            patrolPrefab,
            5f,
            62f,
            9.5f
        );
        PopulateTurrets(
            player.transform,
            turretPrefab,
            20f,
            62f,
            20f,
            2.6f
        );

        CreateCheckpoint(
            new Vector3(34f, -0.95f, 0f),
            checkpointMaterial
        );
        CreateKillZone(
            new Vector3(34f, -9f, 0f),
            new Vector3(110f, 2f, 8f)
        );

        CreateStageExit(
            "IntroPlatformerExit",
            new Vector3(75f, -0.2f, 0f),
            "02_Intro_RideChaser",
            checkpointMaterial,
            false
        );

        CreateLegacyBootstrapperSuppressor();
        EditorSceneManager.SaveScene(
            scene,
            Scenes + "/01_Intro_Platformer.unity"
        );
    }

    private static void BuildIntroRideScene(
        GameObject ridePrefab,
        GameObject patrolPrefab,
        GameObject turretPrefab,
        Material environment,
        Material checkpointMaterial)
    {
        Scene scene = NewScene(LevelMode.RideChaser, "02 — Intro Ride Chaser");
        GameObject rider = InstantiatePrefab(
            ridePrefab,
            new Vector3(-8f, -0.75f, 0f)
        );

        CreateCamera(rider.transform, true);
        CreateSegmentedGround(-12f, 105f, -1.9f, environment, 15f);

        for (int i = 0; i < 7; i++)
        {
            float x = 12f + i * 12f;
            float height = 0.7f + (i % 3) * 0.45f;
            CreatePlatform(
                "IntroRideObstacle_" + i,
                new Vector3(x, -1.55f + height * 0.5f, 0f),
                new Vector3(1.1f, height, 3f),
                environment
            );
        }

        PopulateGroundEnemies(
            rider.transform,
            patrolPrefab,
            20f,
            82f,
            15f
        );
        PopulateTurrets(
            rider.transform,
            turretPrefab,
            30f,
            80f,
            25f,
            1.5f
        );

        CreateCheckpoint(
            new Vector3(48f, -1.05f, 0f),
            checkpointMaterial
        );
        CreateKillZone(
            new Vector3(46f, -9f, 0f),
            new Vector3(140f, 2f, 8f)
        );

        CreateStageExit(
            "IntroRideExit",
            new Vector3(100f, -0.2f, 0f),
            "03_Intro_AirMission",
            checkpointMaterial,
            false
        );

        CreateLegacyBootstrapperSuppressor();
        EditorSceneManager.SaveScene(
            scene,
            Scenes + "/02_Intro_RideChaser.unity"
        );
    }

    private static void BuildIntroAirScene(
        GameObject airPrefab,
        GameObject flyerPrefab,
        GameObject turretPrefab,
        Material environment,
        Material checkpointMaterial)
    {
        Scene scene = NewScene(LevelMode.AirMission, "03 — Intro Air Mission");
        GameObject ship = InstantiatePrefab(
            airPrefab,
            new Vector3(-7f, 0f, 0f)
        );

        CreateCamera(ship.transform, true);

        CreatePlatform(
            "IntroAirLowerBoundary",
            new Vector3(50f, -5.2f, 0f),
            new Vector3(130f, 0.45f, 4f),
            environment
        );
        CreatePlatform(
            "IntroAirUpperBoundary",
            new Vector3(50f, 5.8f, 0f),
            new Vector3(130f, 0.45f, 4f),
            environment
        );

        for (int i = 0; i < 9; i++)
        {
            float x = 9f + i * 10f;
            float y = i % 2 == 0 ? 2.3f : -2.1f;
            CreatePlatform(
                "IntroAirObstacle_" + i,
                new Vector3(x, y, 0f),
                new Vector3(1.7f, 1.7f + (i % 3) * 0.4f, 2.8f),
                environment
            );
        }

        PopulateFlyers(
            ship.transform,
            flyerPrefab,
            12f,
            88f,
            12f
        );
        PopulateTurrets(
            ship.transform,
            turretPrefab,
            25f,
            82f,
            28f,
            4.3f
        );

        CreateCheckpoint(
            new Vector3(48f, 0f, 0f),
            checkpointMaterial
        );
        CreateKillZone(
            new Vector3(50f, -10f, 0f),
            new Vector3(150f, 2f, 8f)
        );
        CreateKillZone(
            new Vector3(50f, 10f, 0f),
            new Vector3(150f, 2f, 8f)
        );

        CreateStageExit(
            "IntroAirExit",
            new Vector3(104f, 0f, 0f),
            "04_Highway_Assault",
            checkpointMaterial,
            false
        );

        CreateLegacyBootstrapperSuppressor();
        EditorSceneManager.SaveScene(
            scene,
            Scenes + "/03_Intro_AirMission.unity"
        );
    }

    private static void BuildHighwayScene(
        GameObject playerPrefab,
        GameObject patrolPrefab,
        GameObject turretPrefab,
        GameObject bossPrefab,
        Material environment,
        Material checkpointMaterial)
    {
        Scene scene = NewScene(LevelMode.Platformer, "04 — Highway Assault");
        GameObject player = InstantiatePrefab(playerPrefab, new Vector3(-8f, -0.55f, 0f));
        BossHealthBar bar = CreateBossHealthBar();
        CreateCamera(player.transform, false);

        CreateSegmentedGround(-12f, 132f, -1.8f, environment, 18f);

        for (int i = 0; i < 13; i++)
        {
            float x = 3f + i * 8.5f;
            float y = i % 3 == 0 ? 1.8f : (i % 3 == 1 ? 0.2f : 3f);
            CreatePlatform(
                "HighwayPlatform_" + i,
                new Vector3(x, y, 0f),
                new Vector3(4.5f, 0.4f, 3f),
                environment
            );
        }

        CreateMovingPlatform(
            "MovingPlatform_Highway",
            new Vector3(58f, 1.2f, 0f),
            new Vector3(4f, 0.4f, 3f),
            new Vector3(0f, 4f, 0f),
            environment
        );

        PopulateGroundEnemies(player.transform, patrolPrefab, 4f, 112f, 7.5f);
        PopulateTurrets(player.transform, turretPrefab, 20f, 108f, 18f, 2.2f);

        CreateCheckpoint(new Vector3(32f, -0.95f, 0f), checkpointMaterial);
        CreateCheckpoint(new Vector3(70f, -0.95f, 0f), checkpointMaterial);
        CreateCheckpoint(new Vector3(108f, -0.95f, 0f), checkpointMaterial);
        CreateKillZone(new Vector3(65f, -9f, 0f), new Vector3(170f, 2f, 8f));

        CreateBossArena(
            "HighwayBossArena",
            126f,
            player.transform,
            bossPrefab,
            bar,
            environment,
            false
        );

        CreateStageExit(
            "HighwayExit",
            new Vector3(141f, -0.2f, 0f),
            "05_Factory_Core",
            checkpointMaterial,
            false
        );

        CreateLegacyBootstrapperSuppressor();
        EditorSceneManager.SaveScene(scene, Scenes + "/04_Highway_Assault.unity");
    }

    private static void BuildFactoryScene(
        GameObject playerPrefab,
        GameObject patrolPrefab,
        GameObject turretPrefab,
        GameObject bossPrefab,
        Material environment,
        Material checkpointMaterial)
    {
        Scene scene = NewScene(LevelMode.Platformer, "05 — Factory Core");
        GameObject player = InstantiatePrefab(playerPrefab, new Vector3(-8f, -0.55f, 0f));
        BossHealthBar bar = CreateBossHealthBar();
        CreateCamera(player.transform, true);

        CreateSegmentedGround(-12f, 152f, -1.8f, environment, 16f);

        for (int i = 0; i < 16; i++)
        {
            float x = 5f + i * 8f;
            float y = (i % 4) * 1.25f - 0.2f;
            CreatePlatform(
                "FactoryCatwalk_" + i,
                new Vector3(x, y, 0f),
                new Vector3(5f, 0.35f, 3f),
                environment
            );
        }

        CreateMovingPlatform(
            "FactoryLift_A",
            new Vector3(35f, -0.2f, 0f),
            new Vector3(3.5f, 0.4f, 3f),
            new Vector3(0f, 5f, 0f),
            environment
        );

        CreateMovingPlatform(
            "FactoryLift_B",
            new Vector3(82f, 3.5f, 0f),
            new Vector3(3.5f, 0.4f, 3f),
            new Vector3(7f, 0f, 0f),
            environment
        );

        CreateMovingPlatform(
            "FactoryLift_C",
            new Vector3(118f, 0.4f, 0f),
            new Vector3(3.5f, 0.4f, 3f),
            new Vector3(0f, 4.5f, 0f),
            environment
        );

        PopulateGroundEnemies(player.transform, patrolPrefab, 5f, 130f, 6.5f);
        PopulateTurrets(player.transform, turretPrefab, 12f, 132f, 14f, 3f);

        CreateCheckpoint(new Vector3(38f, -0.95f, 0f), checkpointMaterial);
        CreateCheckpoint(new Vector3(78f, -0.95f, 0f), checkpointMaterial);
        CreateCheckpoint(new Vector3(122f, -0.95f, 0f), checkpointMaterial);
        CreateKillZone(new Vector3(75f, -10f, 0f), new Vector3(190f, 2f, 8f));

        CreateBossArena(
            "FactoryBossArena",
            146f,
            player.transform,
            bossPrefab,
            bar,
            environment,
            false
        );

        CreateStageExit(
            "FactoryExit",
            new Vector3(161f, -0.2f, 0f),
            "06_RideChaser_Canyon",
            checkpointMaterial,
            false
        );

        CreateLegacyBootstrapperSuppressor();
        EditorSceneManager.SaveScene(scene, Scenes + "/05_Factory_Core.unity");
    }

    private static void BuildRideScene(
        GameObject ridePrefab,
        GameObject patrolPrefab,
        GameObject turretPrefab,
        GameObject bossPrefab,
        Material environment,
        Material checkpointMaterial)
    {
        Scene scene = NewScene(LevelMode.RideChaser, "06 — Ride Chaser Canyon");
        GameObject rider = InstantiatePrefab(ridePrefab, new Vector3(-8f, -0.75f, 0f));
        BossHealthBar bar = CreateBossHealthBar();
        CreateCamera(rider.transform, true);

        CreateSegmentedGround(-12f, 220f, -1.9f, environment, 20f);

        for (int i = 0; i < 14; i++)
        {
            float x = 12f + i * 13f;
            float height = i % 3 == 0 ? 1.2f : (i % 3 == 1 ? 2f : 0.8f);
            CreatePlatform(
                "RideObstacle_" + i,
                new Vector3(x, -1.35f + height * 0.5f, 0f),
                new Vector3(1.2f, height, 3f),
                environment
            );
        }

        PopulateGroundEnemies(rider.transform, patrolPrefab, 18f, 188f, 15f);
        PopulateTurrets(rider.transform, turretPrefab, 28f, 190f, 25f, 1.5f);

        CreateCheckpoint(new Vector3(55f, -1.05f, 0f), checkpointMaterial);
        CreateCheckpoint(new Vector3(112f, -1.05f, 0f), checkpointMaterial);
        CreateCheckpoint(new Vector3(172f, -1.05f, 0f), checkpointMaterial);
        CreateKillZone(new Vector3(110f, -10f, 0f), new Vector3(260f, 2f, 8f));

        CreateBossArena(
            "RideBossArena",
            210f,
            rider.transform,
            bossPrefab,
            bar,
            environment,
            false
        );

        CreateStageExit(
            "RideCanyonExit",
            new Vector3(225f, -0.2f, 0f),
            "07_SkyFortress",
            checkpointMaterial,
            false
        );

        CreateLegacyBootstrapperSuppressor();
        EditorSceneManager.SaveScene(scene, Scenes + "/06_RideChaser_Canyon.unity");
    }

    private static void BuildAirScene(
        GameObject airPrefab,
        GameObject flyerPrefab,
        GameObject turretPrefab,
        GameObject bossPrefab,
        Material environment,
        Material checkpointMaterial)
    {
        Scene scene = NewScene(LevelMode.AirMission, "07 — Sky Fortress");
        GameObject ship = InstantiatePrefab(airPrefab, new Vector3(-7f, 0f, 0f));
        BossHealthBar bar = CreateBossHealthBar();
        CreateCamera(ship.transform, true);

        CreatePlatform(
            "LowerBoundary",
            new Vector3(108f, -5.2f, 0f),
            new Vector3(250f, 0.45f, 4f),
            environment
        );

        CreatePlatform(
            "UpperBoundary",
            new Vector3(108f, 5.8f, 0f),
            new Vector3(250f, 0.45f, 4f),
            environment
        );

        for (int i = 0; i < 18; i++)
        {
            float x = 8f + i * 11f;
            float y = i % 2 == 0 ? 2.3f : -2.1f;
            float height = 1.4f + (i % 3) * 0.55f;
            CreatePlatform(
                "SkyObstacle_" + i,
                new Vector3(x, y, 0f),
                new Vector3(2.2f, height, 2.8f),
                environment
            );
        }

        PopulateFlyers(ship.transform, flyerPrefab, 10f, 192f, 9f);
        PopulateTurrets(ship.transform, turretPrefab, 22f, 186f, 22f, 4.3f);

        CreateCheckpoint(new Vector3(52f, 0f, 0f), checkpointMaterial);
        CreateCheckpoint(new Vector3(108f, 0f, 0f), checkpointMaterial);
        CreateCheckpoint(new Vector3(168f, 0f, 0f), checkpointMaterial);
        CreateKillZone(new Vector3(108f, -10f, 0f), new Vector3(260f, 2f, 8f));
        CreateKillZone(new Vector3(108f, 10f, 0f), new Vector3(260f, 2f, 8f));

        CreateBossArena(
            "AirBossArena",
            210f,
            ship.transform,
            bossPrefab,
            bar,
            environment,
            true
        );

        CreateStageExit(
            "SkyFortressExit",
            new Vector3(225f, 0f, 0f),
            "08_BossRush_Laboratory",
            checkpointMaterial,
            false
        );

        CreateLegacyBootstrapperSuppressor();
        EditorSceneManager.SaveScene(scene, Scenes + "/07_SkyFortress.unity");
    }

    private static void BuildBossRushScene(
        GameObject playerPrefab,
        GameObject groundBossPrefab,
        GameObject airBossPrefab,
        Material environment,
        Material checkpointMaterial)
    {
        Scene scene = NewScene(LevelMode.Platformer, "08 — Boss Rush Laboratory");
        GameObject player = InstantiatePrefab(playerPrefab, new Vector3(-8f, -0.55f, 0f));
        BossHealthBar bar = CreateBossHealthBar();
        CreateCamera(player.transform, true);

        CreateSegmentedGround(-12f, 124f, -1.8f, environment, 16f);
        CreateKillZone(new Vector3(62f, -10f, 0f), new Vector3(160f, 2f, 8f));

        CreateCheckpoint(new Vector3(0f, -0.95f, 0f), checkpointMaterial);
        CreateBossArena(
            "BossRush_Arena_01",
            22f,
            player.transform,
            groundBossPrefab,
            bar,
            environment,
            false
        );

        CreateCheckpoint(new Vector3(42f, -0.95f, 0f), checkpointMaterial);
        CreateBossArena(
            "BossRush_Arena_02",
            62f,
            player.transform,
            airBossPrefab,
            bar,
            environment,
            true
        );

        CreateCheckpoint(new Vector3(82f, -0.95f, 0f), checkpointMaterial);
        GameObject finalBoss = CreateBossArena(
            "BossRush_Arena_03",
            104f,
            player.transform,
            groundBossPrefab,
            bar,
            environment,
            false
        );

        Damageable finalHealth = finalBoss.GetComponent<Damageable>();
        finalHealth.maxHealth = 65;

        BossController finalController = finalBoss.GetComponent<BossController>();
        finalController.bossDisplayName = "OMEGA MINOTAUR";
        finalController.actionInterval = 1.15f;
        finalController.dashSpeed = 15f;

        CreateStageExit(
            "CampaignCompleteExit",
            new Vector3(119f, -0.2f, 0f),
            string.Empty,
            checkpointMaterial,
            true
        );

        CreateLegacyBootstrapperSuppressor();
        EditorSceneManager.SaveScene(scene, Scenes + "/08_BossRush_Laboratory.unity");
    }

    private static Scene NewScene(LevelMode mode, string title)
    {
        Scene scene = EditorSceneManager.NewScene(
            NewSceneSetup.EmptyScene,
            NewSceneMode.Single
        );

        GameObject root = new GameObject("LEVEL_ROOT");
        LevelDefinition definition = root.AddComponent<LevelDefinition>();
        definition.mode = mode;
        definition.levelDisplayName = title;
        definition.designerNotes =
            "Escena larga y editable. Duplica esta escena antes de hacer cambios mayores.";

        GameObject lightObject = new GameObject("Directional Light");
        Light light = lightObject.AddComponent<Light>();
        light.type = LightType.Directional;
        light.intensity = 1.15f;
        lightObject.transform.rotation = Quaternion.Euler(35f, -35f, 0f);
        RenderSettings.ambientLight = new Color(0.34f, 0.36f, 0.42f);

        return scene;
    }

    private static BossHealthBar CreateBossHealthBar()
    {
        GameObject ui = new GameObject("BossHealthBar");
        return ui.AddComponent<BossHealthBar>();
    }

    private static GameObject CreateBossArena(
        string arenaName,
        float centerX,
        Transform player,
        GameObject bossPrefab,
        BossHealthBar healthBar,
        Material environment,
        bool airArena)
    {
        GameObject arenaRoot = new GameObject(arenaName);

        if (!airArena)
        {
            GameObject floor = CreatePlatform(
                arenaName + "_Floor",
                new Vector3(centerX, -1.8f, 0f),
                new Vector3(27f, 0.55f, 4f),
                environment
            );
            floor.transform.SetParent(arenaRoot.transform);
        }

        GameObject entranceDoor = CreatePlatform(
            arenaName + "_EntranceDoor",
            new Vector3(centerX - 12f, 1f, 0f),
            new Vector3(0.8f, 6f, 4f),
            environment
        );
        entranceDoor.transform.SetParent(arenaRoot.transform);
        entranceDoor.SetActive(false);

        GameObject exitDoor = CreatePlatform(
            arenaName + "_ExitDoor",
            new Vector3(centerX + 12f, 1f, 0f),
            new Vector3(0.8f, 6f, 4f),
            environment
        );
        exitDoor.transform.SetParent(arenaRoot.transform);

        GameObject bossObject = InstantiatePrefab(
            bossPrefab,
            new Vector3(centerX + 3f, airArena ? 1f : 0f, 0f)
        );
        bossObject.name = arenaName + "_Boss";
        bossObject.transform.SetParent(arenaRoot.transform);

        BossController boss = bossObject.GetComponent<BossController>();
        boss.target = player;
        boss.ConfigureArena(
            new Vector2(centerX - 9.5f, centerX + 9.5f),
            new Vector2(-3.6f, 4.2f)
        );

        GameObject triggerObject = new GameObject(arenaName + "_Trigger");
        triggerObject.transform.SetParent(arenaRoot.transform);
        triggerObject.transform.position = new Vector3(centerX - 10.5f, 0f, 0f);

        BoxCollider trigger = triggerObject.AddComponent<BoxCollider>();
        trigger.isTrigger = true;
        trigger.size = new Vector3(1.5f, 7f, 4f);

        BossArena arena = triggerObject.AddComponent<BossArena>();
        arena.boss = boss;
        arena.entranceDoor = entranceDoor;
        arena.exitDoor = exitDoor;
        arena.healthBar = healthBar;
        arena.cameraHorizontalBounds = new Vector2(centerX - 6.5f, centerX + 6.5f);

        return bossObject;
    }


    private static void ConfigureBuildSettings()
    {
        string[] scenePaths =
        {
            Scenes + "/00_TrainingStage.unity",
            Scenes + "/01_Intro_Platformer.unity",
            Scenes + "/02_Intro_RideChaser.unity",
            Scenes + "/03_Intro_AirMission.unity",
            Scenes + "/04_Highway_Assault.unity",
            Scenes + "/05_Factory_Core.unity",
            Scenes + "/06_RideChaser_Canyon.unity",
            Scenes + "/07_SkyFortress.unity",
            Scenes + "/08_BossRush_Laboratory.unity"
        };

        EditorBuildSettings.scenes = scenePaths
            .Select(path => new EditorBuildSettingsScene(path, true))
            .ToArray();
    }

    private static void CreateStageExit(
        string name,
        Vector3 position,
        string nextSceneName,
        Material material,
        bool completeCampaign)
    {
        GameObject root = new GameObject(name);
        root.transform.position = position;

        BoxCollider trigger = root.AddComponent<BoxCollider>();
        trigger.isTrigger = true;
        trigger.size = new Vector3(1.4f, 4.5f, 4f);

        StageExit exit = root.AddComponent<StageExit>();
        exit.nextSceneName = nextSceneName;
        exit.completeCampaign = completeCampaign;

        CreatePrimitiveChild(
            PrimitiveType.Cube,
            root.transform,
            "ExitVisual",
            Vector3.zero,
            new Vector3(0.35f, 3.5f, 0.35f),
            material
        );
    }

    private static void CreateWorldLabel(string text, Vector3 position)
    {
        GameObject labelObject = new GameObject("Label_" + text);
        labelObject.transform.position = position;

        TextMesh label = labelObject.AddComponent<TextMesh>();
        label.text = text;
        label.fontSize = 48;
        label.characterSize = 0.12f;
        label.anchor = TextAnchor.MiddleCenter;
        label.alignment = TextAlignment.Center;
        label.color = Color.white;
    }

    private static void CreateSegmentedGround(
        float startX,
        float endX,
        float y,
        Material material,
        float segmentLength)
    {
        float length = endX - startX;
        int count = Mathf.CeilToInt(length / segmentLength);

        for (int i = 0; i < count; i++)
        {
            float segmentStart = startX + i * segmentLength;
            float actualLength = Mathf.Min(segmentLength, endX - segmentStart);
            float center = segmentStart + actualLength * 0.5f;

            CreatePlatform(
                "GroundSegment_" + i,
                new Vector3(center, y, 0f),
                new Vector3(actualLength, 0.55f, 4f),
                material
            );
        }
    }

    private static void PopulateGroundEnemies(
        Transform player,
        GameObject prefab,
        float startX,
        float endX,
        float spacing)
    {
        int index = 0;

        for (float x = startX; x <= endX; x += spacing)
        {
            GameObject enemy = InstantiatePrefab(
                prefab,
                new Vector3(x, -0.55f, 0f)
            );

            enemy.name = "PatrolEnemy_" + index++;
            EnemyPatrol patrol = enemy.GetComponent<EnemyPatrol>();
            patrol.chaseTarget = player;
        }
    }

    private static void PopulateTurrets(
        Transform player,
        GameObject prefab,
        float startX,
        float endX,
        float spacing,
        float y)
    {
        int index = 0;

        for (float x = startX; x <= endX; x += spacing)
        {
            GameObject turretObject = InstantiatePrefab(
                prefab,
                new Vector3(x, y, 0f)
            );

            turretObject.name = "Turret_" + index++;
            EnemyTurret turret = turretObject.GetComponent<EnemyTurret>();
            turret.target = player;
        }
    }

    private static void PopulateFlyers(
        Transform player,
        GameObject prefab,
        float startX,
        float endX,
        float spacing)
    {
        int index = 0;

        for (float x = startX; x <= endX; x += spacing)
        {
            float y = index % 2 == 0 ? 2.5f : -1.8f;
            GameObject flyerObject = InstantiatePrefab(
                prefab,
                new Vector3(x, y, 0f)
            );

            flyerObject.name = "Flyer_" + index++;
            EnemyFlyer flyer = flyerObject.GetComponent<EnemyFlyer>();
            flyer.target = player;
        }
    }

    private static void CreateMovingPlatform(
        string name,
        Vector3 position,
        Vector3 scale,
        Vector3 travelOffset,
        Material material)
    {
        GameObject platform = CreatePlatform(name, position, scale, material);
        Rigidbody body = platform.AddComponent<Rigidbody>();
        body.isKinematic = true;
        body.interpolation = RigidbodyInterpolation.Interpolate;

        MovingPlatform movement = platform.AddComponent<MovingPlatform>();
        movement.localOffset = travelOffset;
        movement.travelSeconds = 2.8f;
    }

    private static void CreateCamera(Transform target, bool clampVertical)
    {
        GameObject cameraObject = new GameObject("Main Camera");
        cameraObject.tag = "MainCamera";

        Camera camera = cameraObject.AddComponent<Camera>();
        camera.orthographic = true;
        camera.orthographicSize = 5.4f;
        camera.backgroundColor = new Color(0.08f, 0.11f, 0.2f);

        cameraObject.transform.position =
            target.position + new Vector3(2.5f, 1.2f, -10f);

        cameraObject.AddComponent<AudioListener>();

        SideScrollerCamera follow = cameraObject.AddComponent<SideScrollerCamera>();
        follow.target = target;
        follow.clampVertical = clampVertical;
        follow.verticalLimits = new Vector2(-1.5f, 3.5f);
    }

    private static GameObject CreateDynamicRoot(
        string name,
        bool noGravity,
        float mass)
    {
        GameObject root = new GameObject(name);
        Rigidbody body = root.AddComponent<Rigidbody>();
        body.useGravity = !noGravity;
        body.mass = mass;
        body.interpolation = RigidbodyInterpolation.Interpolate;
        body.collisionDetectionMode = CollisionDetectionMode.Continuous;
        body.constraints = RigidbodyConstraints.FreezePositionZ |
                           RigidbodyConstraints.FreezeRotation;
        return root;
    }

    private static void AddVisualSlot(
        GameObject root,
        PrimitiveType primitive,
        string visualName,
        Vector3 visualScale,
        Material material)
    {
        Transform anchor = NewChild(root.transform, "VisualAnchor");
        GameObject placeholder = new GameObject("PlaceholderVisual");
        placeholder.transform.SetParent(root.transform, false);

        CreatePrimitiveChild(
            primitive,
            placeholder.transform,
            visualName,
            Vector3.zero,
            visualScale,
            material
        );

        AssetVisualSlot slot = root.AddComponent<AssetVisualSlot>();
        slot.visualAnchor = anchor;
        slot.placeholderRoot = placeholder;
    }

    private static GameObject CreatePlatform(
        string name,
        Vector3 position,
        Vector3 scale,
        Material material)
    {
        GameObject platform = GameObject.CreatePrimitive(PrimitiveType.Cube);
        platform.name = name;
        platform.transform.position = position;
        platform.transform.localScale = scale;
        platform.GetComponent<Renderer>().sharedMaterial = material;
        return platform;
    }

    private static void CreateCheckpoint(Vector3 position, Material material)
    {
        GameObject root = new GameObject("Checkpoint");
        root.transform.position = position;

        BoxCollider trigger = root.AddComponent<BoxCollider>();
        trigger.isTrigger = true;
        trigger.size = new Vector3(1f, 3f, 3f);

        Checkpoint checkpoint = root.AddComponent<Checkpoint>();
        Transform anchor = NewChild(root.transform, "RespawnAnchor");
        anchor.localPosition = new Vector3(0f, 1f, 0f);
        checkpoint.respawnAnchor = anchor;

        CreatePrimitiveChild(
            PrimitiveType.Cube,
            root.transform,
            "CheckpointVisual",
            new Vector3(0f, 0.75f, 0f),
            new Vector3(0.25f, 2.5f, 0.25f),
            material
        );
    }

    private static void CreateKillZone(Vector3 position, Vector3 size)
    {
        GameObject zone = new GameObject("KillZone");
        zone.transform.position = position;

        BoxCollider trigger = zone.AddComponent<BoxCollider>();
        trigger.isTrigger = true;
        trigger.size = size;

        zone.AddComponent<KillZone>();
    }

    private static GameObject CreatePrimitiveChild(
        PrimitiveType primitiveType,
        Transform parent,
        string name,
        Vector3 localPosition,
        Vector3 localScale,
        Material material)
    {
        GameObject child = GameObject.CreatePrimitive(primitiveType);
        child.name = name;
        child.transform.SetParent(parent, false);
        child.transform.localPosition = localPosition;
        child.transform.localScale = localScale;

        Collider collider = child.GetComponent<Collider>();
        if (collider != null)
        {
            UnityEngine.Object.DestroyImmediate(collider);
        }

        Renderer renderer = child.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.sharedMaterial = material;
        }

        return child;
    }

    private static Transform NewChild(Transform parent, string name)
    {
        GameObject child = new GameObject(name);
        child.transform.SetParent(parent, false);
        return child.transform;
    }

    private static GameObject InstantiatePrefab(GameObject prefab, Vector3 position)
    {
        GameObject instance = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
        instance.transform.position = position;
        return instance;
    }

    private static GameObject SavePrefab(GameObject temporaryRoot, string path)
    {
        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(temporaryRoot, path);
        UnityEngine.Object.DestroyImmediate(temporaryRoot);
        return prefab;
    }

    private static Material CreateMaterial(string path, Color color)
    {
        Shader shader = Shader.Find("Universal Render Pipeline/Lit");
        if (shader == null) shader = Shader.Find("Standard");
        if (shader == null) shader = Shader.Find("Diffuse");

        Material material = new Material(shader);
        material.color = color;
        AssetDatabase.CreateAsset(material, path);
        return material;
    }

    private static void EnsureFolder(string parent, string child)
    {
        string path = parent + "/" + child;

        if (!AssetDatabase.IsValidFolder(path))
        {
            AssetDatabase.CreateFolder(parent, child);
        }
    }

    private static void CreateLegacyBootstrapperSuppressor()
    {
        Type legacyType = AppDomain.CurrentDomain
            .GetAssemblies()
            .SelectMany(assembly =>
            {
                try
                {
                    return assembly.GetTypes();
                }
                catch
                {
                    return Array.Empty<Type>();
                }
            })
            .FirstOrDefault(type =>
                type.Name == "MegaManBootstrapper" &&
                typeof(MonoBehaviour).IsAssignableFrom(type));

        if (legacyType == null)
        {
            return;
        }

        GameObject suppressor = new GameObject("LegacyBootstrapper_Suppressed");
        Component component = suppressor.AddComponent(legacyType);

        SerializedObject serialized = new SerializedObject(component);
        SerializedProperty buildDemoLevel = serialized.FindProperty("buildDemoLevel");

        if (buildDemoLevel != null)
        {
            buildDemoLevel.boolValue = false;
            serialized.ApplyModifiedPropertiesWithoutUndo();
        }
    }
}
