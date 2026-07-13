# Campaña completa y jefes

Este paquete genera cuatro stages introductorias y cinco stages principales.

## Arenas de jefe

`BossArena` controla el inicio del combate, las puertas, la cámara y la barra de
vida. Al derrotar al jefe se liberan las puertas y el jugador puede alcanzar el
`StageExit`.

## Modos de jefe

- Ground: persecución, saltos, cargas y disparos en abanico.
- Air: vuelo, persecución vertical, embestidas y disparos dirigidos.

## Flujo de campaña

`StageExit` conecta cada escena con la siguiente. El generador añade todas las
escenas a Build Settings para que `SceneManager.LoadScene` funcione sin
configuración manual.
