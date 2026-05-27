LABORATORIO 6 - RAYCAST, RAYTRACING E ILUMINACIÓN
Proyecto base para Unity

QUÉ HACE
Este paquete crea una escena con 4 habitaciones conectadas. Cada habitación busca expresar una emoción diferente mediante iluminación, objetos, física e interacción:

1. Comodidad:
   - Luz cálida.
   - Sofá, mesa, lámpara.
   - Botón interactivo para prender/apagar lámpara.

2. Tristeza:
   - Luz azul tenue.
   - Cama, ventana y líneas de lluvia.
   - Botón para variar la intensidad/color de la luz.

3. Miedo:
   - Luz roja parpadeante.
   - Objetos oscuros y sombras fuertes.
   - Caja con Rigidbody que se empuja mediante interacción.
   - Raycast reflexivo visible con LineRenderer, simulando rebotes de rayo.

4. Calma:
   - Luz verde suave.
   - Cristal interactivo.
   - Botón para activar/desactivar la luz del cristal.

CÓMO USARLO
1. Crea un proyecto nuevo en Unity con plantilla 3D Core.
2. Copia la carpeta Assets de este ZIP dentro de tu proyecto.
3. Espera a que Unity compile.
4. En Unity, entra al menú:
   Tools > Laboratorio 6 > Generar habitaciones
5. Abre la escena:
   Assets/Lab6_RaycastIluminacion/Scenes/Lab6_Habitaciones_Iluminacion.unity

CONTROLES
- WASD: mover jugador.
- Mouse: mirar.
- Shift: correr.
- Espacio: saltar.
- E: interactuar con botones/objetos usando raycast.
- Escape: liberar cursor.
- Clic izquierdo: volver a capturar cursor.

QUÉ GRABAR PARA EL VIDEO
1. Recorrido por las 4 habitaciones.
2. Explicar qué emoción busca transmitir cada una.
3. Mostrar interacciones con E:
   - Lámpara de comodidad.
   - Luz azul de tristeza.
   - Caja física y raycast reflexivo en miedo.
   - Cristal de calma.
4. Explicar rendimiento:
   - Uso moderado de luces.
   - Luces puntuales/spot en vez de muchas luces globales.
   - Raycast solo para interacción cercana.
   - Objetos simples sin modelos pesados.

NOTA
No se usan assets externos para evitar problemas de licencias.
Puedes mejorar el escenario agregando modelos gratuitos propios, siempre que mantengas los scripts.
