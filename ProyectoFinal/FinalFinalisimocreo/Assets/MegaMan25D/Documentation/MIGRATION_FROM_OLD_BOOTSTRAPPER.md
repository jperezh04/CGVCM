# Migración desde MegaManBootstrapper.cs

El prototipo anterior construía todo durante `Play`, por lo que sus objetos no
existían en modo edición.

Este kit usa escenas y prefabs guardados. Recomendación:

1. Elimina `Assets/Scripts/MegaManBootstrapper.cs`.
2. Importa este kit.
3. Ejecuta el generador.
4. Trabaja únicamente con las escenas de `Generated/Scenes`.

El generador intenta detectar el componente antiguo mediante reflexión y añadirlo
a las escenas con `buildDemoLevel = false`, para evitar duplicados mientras haces
la migración. Eliminar el archivo antiguo sigue siendo la opción más limpia.
