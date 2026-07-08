# Laboratorio 10 - Deteccion y Clasificacion de Objetos con UI

Proyecto para el laboratorio de **Clasificacion y Reconocimiento**. La aplicacion permite subir imagenes en formato **JPG, JPEG, PNG o WEBP**, detectar objetos usando **YOLOv8**, mostrar resultados en una interfaz web bonita y exportar evidencias.

## 1. Que hace el programa

- Permite subir una imagen desde la interfaz.
- Convierte la imagen a formato RGB compatible.
- Ejecuta un modelo YOLOv8 para reconocer y clasificar objetos.
- Dibuja cajas delimitadoras sobre cada objeto detectado.
- Muestra clase, confianza, coordenadas y area de cada deteccion.
- Genera metricas: objetos detectados, clases distintas y confianza promedio.
- Permite descargar la imagen procesada y el CSV de detecciones.
- Permite guardar evidencias en la carpeta `runs/`.

## 2. Estructura del proyecto

```text
lab10_yolo_ui_reconocimiento/
├── app.py                         # UI principal con Streamlit
├── src/
│   └── object_recognizer_cli.py    # Version por consola opcional
├── data/                           # Imagenes de prueba
├── docs/
│   ├── Informe_Lab10_UI_Reconocimiento_Objetos.pdf
│   ├── informe_lab10_ui.md
│   └── GUIA_VIDEO_DEMO.md
├── models/                         # Aqui puedes colocar modelos propios .pt
├── runs/                           # Evidencias generadas
├── scripts/
│   ├── run_app_windows.bat
│   └── run_app_linux_mac.sh
├── requirements.txt
└── README.md
```

## 3. Instalacion en Windows

Abre PowerShell dentro de la carpeta del proyecto y ejecuta:

```powershell
python -m venv .venv
.venv\Scripts\activate
pip install -r requirements.txt
streamlit run app.py
```

Tambien puedes abrir directamente:

```powershell
scripts\run_app_windows.bat
```

## 4. Instalacion en Linux/Mac

```bash
python3 -m venv .venv
source .venv/bin/activate
pip install -r requirements.txt
streamlit run app.py
```

## 5. Uso de la UI

1. Ejecuta `streamlit run app.py`.
2. Se abrira una pagina local en el navegador.
3. En el panel izquierdo configura:
   - Modelo YOLO.
   - Confianza minima.
   - Umbral IoU/NMS.
   - Tamano de inferencia.
   - Clases en espanol.
4. Sube una imagen JPG, JPEG, PNG o WEBP.
5. Presiona **Detectar objetos**.
6. Revisa imagen original, imagen detectada, metricas, tabla y grafico.
7. Descarga la imagen procesada o el CSV.
8. Presiona **Guardar en runs/** para guardar evidencias.

## 6. Ejecucion por consola opcional

Imagen:

```bash
python src/object_recognizer_cli.py --source image --input data/prueba.jpg --conf 0.40 --save
```

Webcam:

```bash
python src/object_recognizer_cli.py --source webcam --camera 0 --conf 0.40 --save
```

Video:

```bash
python src/object_recognizer_cli.py --source video --input data/video.mp4 --conf 0.40 --save
```

## 7. Importante sobre el modelo

Por defecto se usa `yolov8n.pt`, que se descarga automaticamente la primera vez. Si no tienes internet, descarga previamente el archivo `.pt` y colocalo en la carpeta `models/`.
