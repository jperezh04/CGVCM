# Pipeline para colocar tus assets

## Sustituir el jugador, enemigo, moto o nave

1. Ejecuta el generador del kit.
2. Ve a `Assets/MegaMan25D/Generated/Prefabs`.
3. Abre el prefab deseado con doble clic.
4. Selecciona el objeto raíz.
5. En `Asset Visual Slot`, asigna tu prefab al campo `Asset Prefab`.
6. Pulsa **Apply / Refresh Asset In Scene**.
7. Ajusta posición, rotación y escala desde el mismo componente.
8. Guarda el prefab.

El objeto personalizado se crea bajo `VisualAnchor`. La geometría placeholder se
desactiva, pero la física y los scripts permanecen en la raíz.

## Sprites 2D

Puedes usar un prefab que contenga `SpriteRenderer` o un rig 2D. Para un juego 2.5D:

- Mantén el movimiento en el plano X/Y.
- Coloca el sprite mirando hacia la cámara.
- Conserva los colliders y Rigidbody en el objeto raíz generado.
- No pongas Rigidbody adicional dentro del asset visual.

## Modelos 3D

- Orientación recomendada: frente hacia +X.
- El cañón dispara desde el objeto `Muzzle`.
- Ajusta `Visual Rotation Offset` si el modelo fue creado mirando a +Z.
- Usa el Animator dentro de tu prefab visual.

## Animaciones

El controlador no depende de nombres de estados. Puedes añadir un script adaptador
en el prefab visual y leer:

- Velocidad del `Rigidbody`.
- Estado de suelo del controlador.
- Dirección horizontal.
- Eventos de daño de `Damageable`.

## Colisiones

Los colliders principales están en la raíz. Los meshes y sprites visuales no
necesitan colliders. Evita añadir colliders adicionales al asset visual salvo que
sean triggers deliberados.
