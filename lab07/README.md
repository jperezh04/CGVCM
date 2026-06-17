# Laboratorio 7 - Procesamiento de Imagenes

Proyecto resuelto con Python + OpenCV.

## Requisitos
```bash
pip install opencv-python numpy
```

## Ejecutar
```bash
python main.py
```

El programa genera imagenes de ejemplo en `imagenes/` si no existen y guarda los resultados en `outputs/`.

## Partes interactivas
En `main.py`, al final del archivo, puedes descomentar:

```python
visualizador_canales(combinada)
DibujadorInteractivo().ejecutar()
```

## Controles
Visualizador de canales:
- R: activar/desactivar canal rojo
- G: activar/desactivar canal verde
- B: activar/desactivar canal azul
- ESC: salir

Dibujo interactivo:
- 1: linea
- 2: rectangulo
- 3: circulo
- Z: deshacer
- S: guardar
- C: limpiar
- ESC: salir
