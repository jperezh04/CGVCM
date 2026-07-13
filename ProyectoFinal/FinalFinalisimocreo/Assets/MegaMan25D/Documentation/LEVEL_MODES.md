# Modos de nivel

## Platformer

`PlayerMotor` controla movimiento, salto y orientación. `WeaponController` instancia
proyectiles. La cámara sigue al jugador con suavizado.

## Ride Chaser

`RideChaserController` ofrece:

- Avance automático.
- Frenado con izquierda/A.
- Boost con derecha/D.
- Salto.
- Disparo mediante `WeaponController`.

Puedes construir túneles, rampas, obstáculos, secciones de velocidad y jefes.

## Air Mission

`AirVehicleController` ofrece:

- Avance horizontal automático configurable.
- Movimiento vertical libre.
- Boost horizontal.
- Límites verticales.
- Disparo.

Sirve para niveles de nave similares a las secciones aéreas de juegos de acción
2.5D. Puedes desactivar `Auto Forward` para control horizontal completo.

## Crear más niveles

Duplica una escena generada, cambia el componente `Level Definition` y reutiliza
los prefabs. No necesitas volver a ejecutar el generador para cada nivel.
