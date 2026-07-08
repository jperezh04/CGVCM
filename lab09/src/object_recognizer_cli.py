"""
Version por consola del detector de objetos.
Se deja incluida para que el proyecto tambien pueda ejecutarse sin interfaz web.

Ejemplos:
    python src/object_recognizer_cli.py --source image --input data/prueba.jpg --save
    python src/object_recognizer_cli.py --source webcam --camera 0 --save
"""

from __future__ import annotations

import argparse
import csv
from collections import Counter
from dataclasses import dataclass
from datetime import datetime
from pathlib import Path
from typing import Iterable, List, Tuple

import cv2
import numpy as np
from ultralytics import YOLO


@dataclass
class Detection:
    frame_id: int
    label: str
    confidence: float
    x1: int
    y1: int
    x2: int
    y2: int


def build_arg_parser() -> argparse.ArgumentParser:
    parser = argparse.ArgumentParser(description="Reconocimiento y clasificacion de objetos usando YOLOv8 y OpenCV.")
    parser.add_argument("--source", choices=["image", "video", "webcam"], required=True)
    parser.add_argument("--input", type=str, default=None)
    parser.add_argument("--model", type=str, default="yolov8n.pt")
    parser.add_argument("--conf", type=float, default=0.40)
    parser.add_argument("--camera", type=int, default=0)
    parser.add_argument("--save", action="store_true")
    parser.add_argument("--output-dir", type=str, default="runs")
    return parser


def ensure_dir(path: Path) -> None:
    path.mkdir(parents=True, exist_ok=True)


def draw_box(frame: np.ndarray, det: Detection) -> None:
    color = (0, 180, 0)
    text_color = (255, 255, 255)
    label = f"{det.label} {det.confidence:.2f}"
    cv2.rectangle(frame, (det.x1, det.y1), (det.x2, det.y2), color, 2)
    (tw, th), _ = cv2.getTextSize(label, cv2.FONT_HERSHEY_SIMPLEX, 0.6, 2)
    y_text = max(det.y1 - 10, th + 10)
    cv2.rectangle(frame, (det.x1, y_text - th - 6), (det.x1 + tw + 6, y_text + 4), color, -1)
    cv2.putText(frame, label, (det.x1 + 3, y_text), cv2.FONT_HERSHEY_SIMPLEX, 0.6, text_color, 2)


def draw_summary(frame: np.ndarray, counts: Counter) -> None:
    if not counts:
        summary = "Objetos detectados: 0"
    else:
        summary = " | ".join([f"{name}: {qty}" for name, qty in counts.most_common(5)])
    cv2.rectangle(frame, (10, 10), (min(10 + 14 * len(summary), frame.shape[1] - 10), 42), (30, 30, 30), -1)
    cv2.putText(frame, summary, (18, 34), cv2.FONT_HERSHEY_SIMPLEX, 0.6, (255, 255, 255), 2)


def run_inference_on_frame(model: YOLO, frame: np.ndarray, frame_id: int, conf_threshold: float) -> Tuple[np.ndarray, Counter, List[Detection]]:
    annotated = frame.copy()
    results = model.predict(source=frame, conf=conf_threshold, verbose=False)[0]
    detections: List[Detection] = []
    counts: Counter = Counter()

    if results.boxes is not None:
        for box in results.boxes:
            class_id = int(box.cls[0])
            confidence = float(box.conf[0])
            x1, y1, x2, y2 = [int(v) for v in box.xyxy[0].tolist()]
            label = model.names[class_id]
            det = Detection(frame_id, label, confidence, x1, y1, x2, y2)
            detections.append(det)
            counts[label] += 1
            draw_box(annotated, det)

    draw_summary(annotated, counts)
    return annotated, counts, detections


def write_csv_log(csv_path: Path, detections: Iterable[Detection]) -> None:
    write_header = not csv_path.exists()
    with csv_path.open("a", newline="", encoding="utf-8") as file:
        writer = csv.writer(file)
        if write_header:
            writer.writerow(["timestamp", "frame_id", "label", "confidence", "x1", "y1", "x2", "y2"])
        timestamp = datetime.now().isoformat(timespec="seconds")
        for det in detections:
            writer.writerow([timestamp, det.frame_id, det.label, f"{det.confidence:.4f}", det.x1, det.y1, det.x2, det.y2])


def process_image(model: YOLO, image_path: Path, output_dir: Path, conf: float, save: bool) -> None:
    if not image_path.exists():
        raise FileNotFoundError(f"No existe la imagen: {image_path}")
    frame = cv2.imread(str(image_path))
    if frame is None:
        raise ValueError(f"No se pudo leer la imagen: {image_path}")
    annotated, counts, detections = run_inference_on_frame(model, frame, 1, conf)
    print("Resumen de objetos detectados:", dict(counts))
    if save:
        ensure_dir(output_dir)
        output_path = output_dir / f"resultado_{image_path.stem}.jpg"
        log_path = output_dir / "detecciones.csv"
        cv2.imwrite(str(output_path), annotated)
        write_csv_log(log_path, detections)
        print(f"Imagen anotada guardada en: {output_path}")
        print(f"Log CSV guardado en: {log_path}")
    cv2.imshow("Reconocimiento de objetos - Imagen", annotated)
    cv2.waitKey(0)
    cv2.destroyAllWindows()


def process_video_source(model: YOLO, source: str | int, output_dir: Path, conf: float, save: bool, window_title: str) -> None:
    cap = cv2.VideoCapture(source)
    if not cap.isOpened():
        raise RuntimeError(f"No se pudo abrir la fuente de video/camara: {source}")
    ensure_dir(output_dir)
    captures_dir = output_dir / "capturas"
    ensure_dir(captures_dir)
    log_path = output_dir / "detecciones.csv"
    writer = None
    if save:
        width = int(cap.get(cv2.CAP_PROP_FRAME_WIDTH)) or 640
        height = int(cap.get(cv2.CAP_PROP_FRAME_HEIGHT)) or 480
        fps = cap.get(cv2.CAP_PROP_FPS)
        if fps is None or fps <= 0 or fps > 120:
            fps = 20.0
        video_output = output_dir / "video_resultado.mp4"
        writer = cv2.VideoWriter(str(video_output), cv2.VideoWriter_fourcc(*"mp4v"), fps, (width, height))
        print(f"Video anotado sera guardado en: {video_output}")

    frame_id = 0
    print("Presiona 'q' para salir o 's' para guardar una captura.")
    while True:
        ok, frame = cap.read()
        if not ok:
            break
        frame_id += 1
        annotated, counts, detections = run_inference_on_frame(model, frame, frame_id, conf)
        if save:
            write_csv_log(log_path, detections)
            if writer is not None:
                writer.write(annotated)
        cv2.imshow(window_title, annotated)
        key = cv2.waitKey(1) & 0xFF
        if key == ord("s"):
            capture_name = captures_dir / f"captura_frame_{frame_id}.jpg"
            cv2.imwrite(str(capture_name), annotated)
            print(f"Captura guardada: {capture_name}")
        elif key == ord("q"):
            break
    cap.release()
    if writer is not None:
        writer.release()
    cv2.destroyAllWindows()
    if save:
        print(f"Log CSV guardado en: {log_path}")


def main() -> None:
    args = build_arg_parser().parse_args()
    output_dir = Path(args.output_dir)
    print(f"Cargando modelo: {args.model}")
    model = YOLO(args.model)
    if args.source == "image":
        if not args.input:
            raise ValueError("Para --source image debes indicar --input con la ruta de la imagen.")
        process_image(model, Path(args.input), output_dir, args.conf, args.save)
    elif args.source == "video":
        if not args.input:
            raise ValueError("Para --source video debes indicar --input con la ruta del video.")
        process_video_source(model, args.input, output_dir, args.conf, args.save, "Reconocimiento de objetos - Video")
    elif args.source == "webcam":
        process_video_source(model, args.camera, output_dir, args.conf, args.save, "Reconocimiento de objetos - Webcam")


if __name__ == "__main__":
    main()
