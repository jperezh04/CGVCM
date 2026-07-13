# MegaMan 2.5D — Complete Package

Paquete consolidado para un proyecto nuevo de Unity.

## Generar todo

Después de importar `Assets/MegaMan25D`, usa:

`Tools > MegaMan 2.5D > Generate Complete Package`

El generador crea prefabs, materiales y nueve escenas reales de Unity dentro de:

`Assets/MegaMan25D/Generated_CompletePackage`

También coloca las nueve escenas en **Build Settings**, en orden, para que las
salidas de nivel funcionen.

## Orden de campaña

1. Training Stage
2. Intro Platformer
3. Intro Ride Chaser
4. Intro Air Mission
5. Highway Assault
6. Factory Core
7. Ride Chaser Canyon
8. Sky Fortress
9. Boss Rush Laboratory

## Controles

- Movimiento: A/D o flechas izquierda/derecha.
- Salto: Espacio, W o flecha arriba.
- Nave vertical: W/S o flechas arriba/abajo.
- Disparo: J, X, Ctrl izquierdo o clic izquierdo.

## Reemplazar placeholders

Abre cualquier prefab generado y usa su componente `Asset Visual Slot`.
Asigna tu prefab visual y pulsa `Apply / Refresh Asset In Scene`.

La lógica, física, colliders y armas permanecen en el objeto raíz.
