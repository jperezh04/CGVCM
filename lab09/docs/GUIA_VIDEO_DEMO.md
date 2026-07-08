# Guia para grabar el video de demostracion

Duracion recomendada: 2 a 4 minutos.

## Guion sugerido

Hola, en este video voy a presentar mi aplicacion de clasificacion y reconocimiento de objetos. El programa fue desarrollado en Python usando YOLOv8, OpenCV y Streamlit. La idea principal es que el usuario pueda subir una imagen en formato JPG, PNG o WEBP, ejecutar la deteccion y visualizar los objetos reconocidos mediante cajas, etiquetas, confianza y una tabla de resultados.

## Pasos a mostrar

1. Mostrar la carpeta del proyecto en VSCode.
2. Mostrar los archivos principales: `app.py`, `requirements.txt`, `src/object_recognizer_cli.py` y `docs/`.
3. Abrir una terminal y ejecutar:

```bash
streamlit run app.py
```

4. Mostrar la interfaz web en el navegador.
5. Explicar rapidamente el panel izquierdo:
   - Modelo YOLO.
   - Confianza minima.
   - IoU/NMS.
   - Tamano de inferencia.
   - Etiquetas en espanol.
6. Subir una imagen JPG, PNG o WEBP.
7. Presionar **Detectar objetos**.
8. Mostrar la imagen original y la imagen detectada.
9. Mostrar metricas, tabla de detecciones y grafico de conteo.
10. Descargar la imagen o CSV.
11. Guardar evidencias en `runs/`.
12. Cerrar con una breve conclusion:

> En conclusion, la aplicacion permite aplicar vision computacional de forma interactiva, ya que reconoce, clasifica y localiza objetos dentro de una imagen, generando ademas evidencias utiles para validar el funcionamiento del sistema.

## Capturas recomendadas para el PDF

- UI inicial.
- Imagen subida.
- Resultado con cajas delimitadoras.
- Tabla de detecciones.
- CSV o carpeta `runs/`.
