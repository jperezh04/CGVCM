"""
UI web para el Laboratorio de Clasificacion y Reconocimiento de Objetos.

Ejecutar:
    streamlit run app.py

Formatos soportados en la UI:
    JPG, JPEG, PNG, WEBP
"""

from __future__ import annotations

import io
from collections import Counter
from datetime import datetime
from pathlib import Path
from typing import Dict, List, Tuple

import cv2
import numpy as np
import pandas as pd
import streamlit as st
from PIL import Image
from ultralytics import YOLO


APP_TITLE = "Detector de Objetos con YOLOv8"
RUNS_DIR = Path("runs")

# Traducciones de clases COCO mas comunes. Si una clase no aparece aqui,
# se muestra el nombre original del modelo.
COCO_ES: Dict[str, str] = {
    "person": "persona",
    "bicycle": "bicicleta",
    "car": "auto",
    "motorcycle": "motocicleta",
    "airplane": "avion",
    "bus": "bus",
    "train": "tren",
    "truck": "camion",
    "boat": "bote",
    "traffic light": "semaforo",
    "fire hydrant": "hidrante",
    "stop sign": "senal de pare",
    "parking meter": "parquimetro",
    "bench": "banca",
    "bird": "ave",
    "cat": "gato",
    "dog": "perro",
    "horse": "caballo",
    "sheep": "oveja",
    "cow": "vaca",
    "elephant": "elefante",
    "bear": "oso",
    "zebra": "cebra",
    "giraffe": "jirafa",
    "backpack": "mochila",
    "umbrella": "paraguas",
    "handbag": "bolso",
    "tie": "corbata",
    "suitcase": "maleta",
    "frisbee": "frisbee",
    "skis": "esquis",
    "snowboard": "snowboard",
    "sports ball": "pelota",
    "kite": "cometa",
    "baseball bat": "bate",
    "baseball glove": "guante",
    "skateboard": "skateboard",
    "surfboard": "tabla de surf",
    "tennis racket": "raqueta",
    "bottle": "botella",
    "wine glass": "copa",
    "cup": "taza",
    "fork": "tenedor",
    "knife": "cuchillo",
    "spoon": "cuchara",
    "bowl": "tazon",
    "banana": "platano",
    "apple": "manzana",
    "sandwich": "sandwich",
    "orange": "naranja",
    "broccoli": "brocoli",
    "carrot": "zanahoria",
    "hot dog": "hot dog",
    "pizza": "pizza",
    "donut": "dona",
    "cake": "pastel",
    "chair": "silla",
    "couch": "sofa",
    "potted plant": "maceta",
    "bed": "cama",
    "dining table": "mesa",
    "toilet": "inodoro",
    "tv": "televisor",
    "laptop": "laptop",
    "mouse": "mouse",
    "remote": "control remoto",
    "keyboard": "teclado",
    "cell phone": "celular",
    "microwave": "microondas",
    "oven": "horno",
    "toaster": "tostadora",
    "sink": "lavadero",
    "refrigerator": "refrigeradora",
    "book": "libro",
    "clock": "reloj",
    "vase": "florero",
    "scissors": "tijeras",
    "teddy bear": "peluche",
    "hair drier": "secadora",
    "toothbrush": "cepillo de dientes",
}


st.set_page_config(
    page_title=APP_TITLE,
    page_icon="🔎",
    layout="wide",
    initial_sidebar_state="expanded",
)


CUSTOM_CSS = """
<style>
    .stApp {
        background: linear-gradient(135deg, #eef2ff 0%, #f8fafc 45%, #ecfeff 100%);
    }
    .main .block-container {
        padding-top: 1.4rem;
        padding-bottom: 2rem;
    }
    .hero {
        padding: 1.4rem 1.6rem;
        border-radius: 24px;
        background: linear-gradient(135deg, #0f172a 0%, #1e3a8a 52%, #0891b2 100%);
        color: white;
        box-shadow: 0 18px 45px rgba(15, 23, 42, 0.18);
        margin-bottom: 1.1rem;
    }
    .hero h1 {
        margin: 0;
        font-size: 2.2rem;
        font-weight: 800;
    }
    .hero p {
        margin: 0.45rem 0 0 0;
        font-size: 1rem;
        opacity: 0.92;
    }
    .pill {
        display: inline-block;
        padding: 0.28rem 0.7rem;
        border-radius: 999px;
        background: rgba(255,255,255,0.16);
        border: 1px solid rgba(255,255,255,0.24);
        margin-right: 0.45rem;
        font-size: 0.84rem;
    }
    .card {
        padding: 1rem 1.1rem;
        border-radius: 18px;
        background: rgba(255,255,255,0.82);
        border: 1px solid rgba(148, 163, 184, 0.28);
        box-shadow: 0 12px 30px rgba(15, 23, 42, 0.08);
        margin-bottom: 1rem;
    }
    .card h3 {
        margin-top: 0;
        color: #0f172a;
    }
    .mini-title {
        font-size: 0.82rem;
        letter-spacing: 0.06em;
        text-transform: uppercase;
        color: #2563eb;
        font-weight: 800;
        margin-bottom: 0.25rem;
    }
    .good {
        color: #166534;
        font-weight: 700;
    }
    .warn {
        color: #92400e;
        font-weight: 700;
    }
    div[data-testid="stMetricValue"] {
        font-size: 1.7rem;
        font-weight: 800;
    }
</style>
"""

st.markdown(CUSTOM_CSS, unsafe_allow_html=True)


@st.cache_resource(show_spinner=False)
def load_yolo_model(model_path: str) -> YOLO:
    """Carga el modelo una sola vez para no reiniciarlo en cada interaccion."""
    return YOLO(model_path)


def pil_to_rgb_array(image: Image.Image) -> np.ndarray:
    """Convierte cualquier imagen subida a RGB, incluyendo PNG/WEBP con transparencia."""
    if image.mode not in ("RGB", "RGBA"):
        image = image.convert("RGB")
    if image.mode == "RGBA":
        background = Image.new("RGB", image.size, (255, 255, 255))
        background.paste(image, mask=image.split()[-1])
        image = background
    return np.array(image.convert("RGB"))


def readable_label(label: str, spanish: bool) -> str:
    return COCO_ES.get(label, label) if spanish else label


def color_for_class(class_id: int) -> Tuple[int, int, int]:
    """Genera un color estable en RGB para cada clase detectada."""
    palette = [
        (37, 99, 235),
        (5, 150, 105),
        (234, 88, 12),
        (147, 51, 234),
        (220, 38, 38),
        (8, 145, 178),
        (202, 138, 4),
        (79, 70, 229),
    ]
    return palette[class_id % len(palette)]


def draw_detection(
    image_rgb: np.ndarray,
    x1: int,
    y1: int,
    x2: int,
    y2: int,
    label: str,
    confidence: float,
    color: Tuple[int, int, int],
) -> None:
    """Dibuja una caja delimitadora y una etiqueta legible sobre la imagen."""
    cv2.rectangle(image_rgb, (x1, y1), (x2, y2), color, 3)
    text = f"{label} {confidence:.2f}"
    font = cv2.FONT_HERSHEY_SIMPLEX
    font_scale = 0.68
    thickness = 2
    (tw, th), baseline = cv2.getTextSize(text, font, font_scale, thickness)
    y_text = y1 - 10 if y1 - 10 > th else y1 + th + 12
    box_y1 = max(0, y_text - th - baseline - 8)
    box_y2 = min(image_rgb.shape[0], y_text + baseline + 6)
    box_x2 = min(image_rgb.shape[1], x1 + tw + 12)
    cv2.rectangle(image_rgb, (x1, box_y1), (box_x2, box_y2), color, -1)
    cv2.putText(image_rgb, text, (x1 + 6, y_text), font, font_scale, (255, 255, 255), thickness)


def run_detection(
    image_rgb: np.ndarray,
    model: YOLO,
    conf_threshold: float,
    iou_threshold: float,
    image_size: int,
    use_spanish_labels: bool,
) -> Tuple[np.ndarray, pd.DataFrame, Counter]:
    """
    Ejecuta el flujo principal de reconocimiento:
    adquisicion -> normalizacion -> inferencia -> postprocesamiento -> visualizacion.
    """
    results = model.predict(
        source=image_rgb,
        conf=conf_threshold,
        iou=iou_threshold,
        imgsz=image_size,
        verbose=False,
    )[0]

    annotated = image_rgb.copy()
    rows: List[Dict[str, object]] = []
    counts: Counter = Counter()

    if results.boxes is not None and len(results.boxes) > 0:
        for index, box in enumerate(results.boxes, start=1):
            class_id = int(box.cls[0])
            confidence = float(box.conf[0])
            x1, y1, x2, y2 = [int(v) for v in box.xyxy[0].tolist()]
            original_label = str(model.names[class_id])
            label = readable_label(original_label, use_spanish_labels)
            area = max(0, x2 - x1) * max(0, y2 - y1)

            rows.append(
                {
                    "Nro": index,
                    "Clase": label,
                    "Clase original": original_label,
                    "Confianza": round(confidence, 4),
                    "x1": x1,
                    "y1": y1,
                    "x2": x2,
                    "y2": y2,
                    "Area bbox": area,
                }
            )
            counts[label] += 1
            draw_detection(annotated, x1, y1, x2, y2, label, confidence, color_for_class(class_id))

    df = pd.DataFrame(rows)
    return annotated, df, counts


def image_to_bytes(image_rgb: np.ndarray, file_format: str = "PNG") -> bytes:
    """Convierte una imagen RGB a bytes para descarga."""
    buffer = io.BytesIO()
    Image.fromarray(image_rgb).save(buffer, format=file_format)
    return buffer.getvalue()


def dataframe_to_csv_bytes(df: pd.DataFrame) -> bytes:
    return df.to_csv(index=False).encode("utf-8-sig")


def save_evidence(image_rgb: np.ndarray, df: pd.DataFrame) -> Path:
    """Guarda la evidencia del resultado en runs/ con fecha y hora."""
    RUNS_DIR.mkdir(parents=True, exist_ok=True)
    timestamp = datetime.now().strftime("%Y%m%d_%H%M%S")
    session_dir = RUNS_DIR / f"deteccion_{timestamp}"
    session_dir.mkdir(parents=True, exist_ok=True)
    Image.fromarray(image_rgb).save(session_dir / "imagen_detectada.png")
    df.to_csv(session_dir / "detecciones.csv", index=False, encoding="utf-8-sig")
    return session_dir


def render_header() -> None:
    st.markdown(
        """
        <div class="hero">
            <span class="pill">YOLOv8</span>
            <span class="pill">OpenCV</span>
            <span class="pill">Streamlit UI</span>
            <span class="pill">JPG · PNG · WEBP</span>
            <h1>Detector y Clasificador de Objetos</h1>
            <p>Aplicacion multimedia para cargar una imagen, detectar objetos, clasificarlos, contar resultados y exportar evidencias del laboratorio.</p>
        </div>
        """,
        unsafe_allow_html=True,
    )


def render_sidebar() -> Tuple[str, float, float, int, bool]:
    st.sidebar.title("Configuracion")
    model_choice = st.sidebar.selectbox(
        "Modelo YOLO",
        ["yolov8n.pt", "yolov8s.pt", "models/modelo_entrenado.pt"],
        index=0,
        help="yolov8n es mas ligero. yolov8s puede ser mas preciso, pero consume mas recursos.",
    )
    conf = st.sidebar.slider(
        "Confianza minima",
        min_value=0.10,
        max_value=0.95,
        value=0.40,
        step=0.05,
        help="Si subes este valor, se aceptan menos detecciones pero con mayor seguridad.",
    )
    iou = st.sidebar.slider(
        "Umbral IoU / NMS",
        min_value=0.10,
        max_value=0.90,
        value=0.45,
        step=0.05,
        help="Controla la eliminacion de cajas repetidas sobre el mismo objeto.",
    )
    imgsz = st.sidebar.select_slider(
        "Tamano de inferencia",
        options=[320, 416, 512, 640, 768, 960],
        value=640,
        help="Mayor tamano puede mejorar deteccion de objetos pequenos, pero sera mas lento.",
    )
    spanish_labels = st.sidebar.toggle("Mostrar clases en espanol", value=True)

    st.sidebar.markdown("---")
    st.sidebar.caption("Flujo: imagen subida -> RGB -> YOLO -> filtrado -> cajas -> tabla -> exportacion.")
    return model_choice, conf, iou, imgsz, spanish_labels


def render_detector_tab(model_path: str, conf: float, iou: float, imgsz: int, spanish_labels: bool) -> None:
    left, right = st.columns([0.38, 0.62], gap="large")

    with left:
        st.markdown('<div class="card">', unsafe_allow_html=True)
        st.subheader("1. Subir imagen")
        uploaded = st.file_uploader(
            "Selecciona una imagen JPG, PNG o WEBP",
            type=["jpg", "jpeg", "png", "webp"],
            accept_multiple_files=False,
        )
        analyze_button = st.button("Detectar objetos", type="primary", use_container_width=True)
        st.markdown(
            """
            <div class="mini-title">Formatos aceptados</div>
            JPG/JPEG para fotografias, PNG para imagenes con mejor calidad y WEBP para imagenes comprimidas modernas.
            """,
            unsafe_allow_html=True,
        )
        st.markdown("</div>", unsafe_allow_html=True)

        st.markdown('<div class="card">', unsafe_allow_html=True)
        st.subheader("2. Evidencia esperada")
        st.write(
            "La aplicacion genera imagen anotada, tabla de objetos, conteo por clase, CSV descargable y carpeta `runs/` para sustentar el funcionamiento."
        )
        st.markdown("</div>", unsafe_allow_html=True)

    with right:
        if uploaded is None:
            st.info("Sube una imagen para iniciar la deteccion. Puedes usar una foto con personas, laptops, celulares, botellas, autos u otros objetos comunes.")
            return

        image = Image.open(uploaded)
        image_rgb = pil_to_rgb_array(image)

        if analyze_button:
            try:
                with st.spinner("Cargando modelo y procesando imagen..."):
                    model = load_yolo_model(model_path)
                    annotated, df, counts = run_detection(image_rgb, model, conf, iou, imgsz, spanish_labels)

                st.session_state["last_result"] = {
                    "original": image_rgb,
                    "annotated": annotated,
                    "df": df,
                    "counts": counts,
                    "filename": uploaded.name,
                }
            except Exception as exc:
                st.error("No se pudo ejecutar la deteccion.")
                st.exception(exc)
                return

        if "last_result" not in st.session_state:
            st.image(image_rgb, caption="Vista previa de la imagen cargada", use_container_width=True)
            st.warning("Presiona 'Detectar objetos' para procesar la imagen.")
            return

        result = st.session_state["last_result"]
        annotated = result["annotated"]
        df = result["df"]
        counts = result["counts"]

        metric_a, metric_b, metric_c = st.columns(3)
        metric_a.metric("Objetos detectados", int(len(df)))
        metric_b.metric("Clases distintas", int(len(counts)))
        avg_conf = 0 if df.empty else round(float(df["Confianza"].mean()), 3)
        metric_c.metric("Confianza promedio", avg_conf)

        img_col_1, img_col_2 = st.columns(2)
        with img_col_1:
            st.image(result["original"], caption="Imagen original", use_container_width=True)
        with img_col_2:
            st.image(annotated, caption="Imagen con detecciones", use_container_width=True)

        st.subheader("Resultados de clasificacion")
        if df.empty:
            st.warning("No se detectaron objetos con la confianza configurada. Prueba bajando el umbral de confianza o usando otra imagen.")
        else:
            st.dataframe(df, use_container_width=True, hide_index=True)
            st.bar_chart(pd.DataFrame.from_dict(counts, orient="index", columns=["Cantidad"]))

        dl_col_1, dl_col_2, dl_col_3 = st.columns(3)
        with dl_col_1:
            st.download_button(
                "Descargar imagen detectada",
                data=image_to_bytes(annotated, "PNG"),
                file_name="resultado_deteccion.png",
                mime="image/png",
                use_container_width=True,
            )
        with dl_col_2:
            st.download_button(
                "Descargar CSV",
                data=dataframe_to_csv_bytes(df),
                file_name="detecciones.csv",
                mime="text/csv",
                use_container_width=True,
                disabled=df.empty,
            )
        with dl_col_3:
            if st.button("Guardar en runs/", use_container_width=True, disabled=df.empty):
                folder = save_evidence(annotated, df)
                st.success(f"Evidencia guardada en: {folder}")


def render_processing_tab() -> None:
    st.markdown('<div class="card">', unsafe_allow_html=True)
    st.header("Funcionamiento del programa")
    st.write(
        "El sistema implementa una cadena de procesamiento de imagenes orientada al reconocimiento y clasificacion de objetos. "
        "La entrada es una imagen subida por el usuario y la salida es una imagen anotada con cajas delimitadoras, etiquetas, confianza y una tabla de resultados."
    )
    st.markdown("</div>", unsafe_allow_html=True)

    steps = [
        ("1. Adquisicion", "La imagen se obtiene desde la interfaz mediante `st.file_uploader`, aceptando archivos JPG, JPEG, PNG y WEBP."),
        ("2. Normalizacion", "La imagen se convierte a RGB. Si tiene transparencia, como algunos PNG o WEBP, se coloca sobre fondo blanco para evitar errores."),
        ("3. Inferencia", "YOLOv8 procesa la imagen completa y predice cajas delimitadoras, clase del objeto y confianza."),
        ("4. Postprocesamiento", "Se filtran detecciones usando la confianza minima y el umbral IoU/NMS para reducir cajas repetidas."),
        ("5. Visualizacion", "OpenCV dibuja rectangulos, etiquetas y valores de confianza sobre la imagen."),
        ("6. Evidencia", "Streamlit muestra metricas, tabla de detecciones, grafico de conteo, imagen descargable y CSV."),
    ]

    for title, description in steps:
        st.markdown(
            f"""
            <div class="card">
                <div class="mini-title">{title}</div>
                <p>{description}</p>
            </div>
            """,
            unsafe_allow_html=True,
        )

    st.subheader("Funciones principales implementadas")
    st.code(
        """
pil_to_rgb_array()       # Convierte JPG/PNG/WEBP a matriz RGB compatible.
load_yolo_model()        # Carga el modelo YOLO con cache de Streamlit.
run_detection()          # Ejecuta inferencia y devuelve imagen anotada + tabla.
draw_detection()         # Dibuja caja, etiqueta y confianza sobre la imagen.
save_evidence()          # Guarda imagen procesada y CSV dentro de runs/.
        """.strip(),
        language="python",
    )


def render_lab_tab() -> None:
    st.header("Contenido requerido por el laboratorio")

    col1, col2 = st.columns(2)
    with col1:
        st.markdown(
            """
            <div class="card">
                <h3>Descripcion general</h3>
                <p>La aplicacion permite reconocer y clasificar objetos presentes en una imagen. El usuario carga una imagen, configura parametros del modelo, ejecuta la deteccion y obtiene resultados visuales y tabulares.</p>
            </div>
            """,
            unsafe_allow_html=True,
        )
        st.markdown(
            """
            <div class="card">
                <h3>Tecnologias utilizadas</h3>
                <ul>
                    <li><b>Python:</b> lenguaje principal.</li>
                    <li><b>Streamlit:</b> interfaz web interactiva.</li>
                    <li><b>YOLOv8 / Ultralytics:</b> reconocimiento y clasificacion.</li>
                    <li><b>OpenCV:</b> dibujo de cajas y procesamiento visual.</li>
                    <li><b>Pillow:</b> lectura de JPG, PNG y WEBP.</li>
                    <li><b>Pandas:</b> tabla y CSV de evidencias.</li>
                </ul>
            </div>
            """,
            unsafe_allow_html=True,
        )
    with col2:
        st.markdown(
            """
            <div class="card">
                <h3>Modificaciones realizadas</h3>
                <p>Se adapto el reconocimiento de objetos a una UI web, se agrego carga de imagenes, soporte para PNG/WEBP, panel de configuracion, metricas, tabla de resultados, descargas y almacenamiento de evidencias.</p>
            </div>
            """,
            unsafe_allow_html=True,
        )
        st.markdown(
            """
            <div class="card">
                <h3>Ventajas y desventajas</h3>
                <p><b>Ventajas:</b> facil de usar, no requiere escribir comandos para probar imagenes, entrega evidencias y permite variar parametros.</p>
                <p><b>Desventajas:</b> depende del modelo usado, puede fallar con objetos pequenos, borrosos, tapados o clases no incluidas en COCO.</p>
            </div>
            """,
            unsafe_allow_html=True,
        )

    st.subheader("Cuestionario")
    with st.expander("1. Describan un algoritmo utilizado para reconocimiento y clasificacion de objetos", expanded=True):
        st.write(
            "YOLO es un algoritmo de deteccion de objetos de una sola etapa. En lugar de analizar la imagen por partes con varios clasificadores, "
            "procesa la imagen completa en una sola pasada y predice, al mismo tiempo, las cajas delimitadoras, la clase probable y la confianza. "
            "Por eso es adecuado para aplicaciones interactivas, ya que ofrece una buena relacion entre velocidad y precision."
        )
    with st.expander("2. Problemas que pueden ocurrir en reconocimiento y clasificacion"):
        st.write(
            "Pueden aparecer falsos positivos, falsos negativos, baja precision con poca luz, objetos ocluidos, imagenes borrosas, objetos muy pequenos, "
            "fondos confusos, clases no entrenadas, dependencia del dataset y consumo de recursos si se usa un modelo grande."
        )

    st.subheader("Video sugerido para la entrega")
    st.info(
        "Grabar: 1) abrir proyecto en VSCode, 2) ejecutar streamlit run app.py, 3) subir JPG/PNG/WEBP, "
        "4) cambiar confianza, 5) mostrar deteccion, tabla, grafico y descargas, 6) mostrar carpeta runs/."
    )


def render_about_tab() -> None:
    st.header("Acerca del proyecto")
    st.markdown(
        """
        <div class="card">
            <h3>Objetivo</h3>
            <p>Demostrar el uso de tecnicas de vision computacional para reconocer, localizar y clasificar objetos dentro de imagenes digitales, mediante una interfaz visual entendible para el usuario.</p>
        </div>
        """,
        unsafe_allow_html=True,
    )
    st.markdown(
        """
        <div class="card">
            <h3>Limitaciones</h3>
            <p>El modelo base fue entrenado con clases generales del dataset COCO. Por ello, reconoce objetos comunes, pero no necesariamente objetos especificos del laboratorio o de un negocio. Para una tematica especializada, se deberia entrenar un modelo propio.</p>
        </div>
        """,
        unsafe_allow_html=True,
    )
    st.markdown(
        """
        <div class="card">
            <h3>Mejoras futuras</h3>
            <ul>
                <li>Agregar deteccion en video y camara web desde la misma UI.</li>
                <li>Entrenar un modelo con clases personalizadas.</li>
                <li>Guardar historico de pruebas en una base de datos.</li>
                <li>Exportar reporte PDF automatico con imagen y resultados.</li>
            </ul>
        </div>
        """,
        unsafe_allow_html=True,
    )


def main() -> None:
    render_header()
    model_path, conf, iou, imgsz, spanish_labels = render_sidebar()

    tabs = st.tabs(["Detector", "Procesamiento", "Informe del lab", "Acerca de"])
    with tabs[0]:
        render_detector_tab(model_path, conf, iou, imgsz, spanish_labels)
    with tabs[1]:
        render_processing_tab()
    with tabs[2]:
        render_lab_tab()
    with tabs[3]:
        render_about_tab()


if __name__ == "__main__":
    main()
