"""
Laboratorio 8 - Procesamiento de imágenes
Autor: Jeremy Perez
Curso: Computación Gráfica, Visión Computacional y Multimedia

Incluye:
1. Apertura de tres imágenes.
2. Redimensionamiento al tamaño de la imagen más grande.
3. Combinación de canales: R de imagen 1, G de imagen 2 y B de imagen 3.
4. Conversión a negativo y escala de grises.
5. Visualizador interactivo de canales con botones.
6. Detección facial con Haar Cascade.
7. Aplicación de umbral binario.
8. Dibujo de círculo y texto.
9. Programa de dibujo interactivo con mouse y teclado.

Instalación:
    pip install opencv-python numpy pillow

Ejecución:
    python laboratorio8_interactivo.py
"""

import os
import tkinter as tk
from tkinter import filedialog, messagebox

import cv2
import numpy as np
from PIL import Image, ImageTk


# ==========================================================
# CONFIGURACIÓN DE CARPETAS
# ==========================================================

BASE_DIR = os.path.dirname(os.path.abspath(__file__))
IMG_DIR = os.path.join(BASE_DIR, "imagenes")
OUT_DIR = os.path.join(BASE_DIR, "outputs")

os.makedirs(IMG_DIR, exist_ok=True)
os.makedirs(OUT_DIR, exist_ok=True)


# ==========================================================
# FUNCIONES GENERALES DEL LABORATORIO
# ==========================================================

def crear_imagenes_demo():
    """Crea tres imágenes sencillas si todavía no existen."""
    rutas = [
        os.path.join(IMG_DIR, "persona.png"),
        os.path.join(IMG_DIR, "perro.png"),
        os.path.join(IMG_DIR, "gato.png"),
    ]

    if all(os.path.exists(ruta) for ruta in rutas):
        return rutas

    # Imagen 1: persona
    img1 = np.full((360, 480, 3), (225, 235, 245), dtype=np.uint8)
    cv2.circle(img1, (240, 115), 55, (70, 160, 230), -1)
    cv2.circle(img1, (220, 105), 6, (20, 20, 20), -1)
    cv2.circle(img1, (260, 105), 6, (20, 20, 20), -1)
    cv2.ellipse(img1, (240, 130), (25, 15), 0, 0, 180, (20, 20, 20), 2)
    cv2.rectangle(img1, (185, 180), (295, 310), (80, 80, 200), -1)
    cv2.line(img1, (185, 190), (135, 260), (80, 80, 200), 16)
    cv2.line(img1, (295, 190), (345, 260), (80, 80, 200), 16)
    cv2.putText(
        img1,
        "PERSONA",
        (155, 340),
        cv2.FONT_HERSHEY_SIMPLEX,
        1,
        (40, 40, 40),
        2,
    )
    cv2.imwrite(rutas[0], img1)

    # Imagen 2: perro
    img2 = np.full((400, 520, 3), (245, 235, 220), dtype=np.uint8)
    cv2.ellipse(img2, (260, 210), (130, 90), 0, 0, 360, (50, 140, 210), -1)
    cv2.circle(img2, (175, 145), 40, (40, 110, 190), -1)
    cv2.circle(img2, (345, 145), 40, (40, 110, 190), -1)
    cv2.circle(img2, (220, 185), 9, (30, 30, 30), -1)
    cv2.circle(img2, (300, 185), 9, (30, 30, 30), -1)
    cv2.ellipse(img2, (260, 230), (42, 28), 0, 0, 360, (30, 80, 150), -1)
    cv2.circle(img2, (260, 220), 10, (15, 15, 15), -1)
    cv2.line(img2, (260, 230), (250, 248), (15, 15, 15), 2)
    cv2.line(img2, (260, 230), (270, 248), (15, 15, 15), 2)
    cv2.putText(
        img2,
        "PERRO",
        (185, 350),
        cv2.FONT_HERSHEY_SIMPLEX,
        1.2,
        (45, 45, 45),
        2,
    )
    cv2.imwrite(rutas[1], img2)

    # Imagen 3: gato
    img3 = np.full((320, 430, 3), (230, 245, 230), dtype=np.uint8)
    oreja_1 = np.array([[145, 110], [180, 40], [210, 125]], np.int32)
    oreja_2 = np.array([[285, 110], [250, 40], [220, 125]], np.int32)
    cv2.fillPoly(img3, [oreja_1], (190, 120, 60))
    cv2.fillPoly(img3, [oreja_2], (190, 120, 60))
    cv2.circle(img3, (215, 160), 95, (205, 145, 80), -1)
    cv2.circle(img3, (180, 145), 10, (20, 20, 20), -1)
    cv2.circle(img3, (250, 145), 10, (20, 20, 20), -1)
    cv2.circle(img3, (215, 175), 8, (20, 20, 20), -1)
    cv2.line(img3, (215, 183), (200, 200), (20, 20, 20), 2)
    cv2.line(img3, (215, 183), (230, 200), (20, 20, 20), 2)

    for y in [165, 185]:
        cv2.line(img3, (120, y), (180, 175), (20, 20, 20), 2)
        cv2.line(img3, (250, 175), (310, y), (20, 20, 20), 2)

    cv2.putText(
        img3,
        "GATO",
        (145, 300),
        cv2.FONT_HERSHEY_SIMPLEX,
        1.1,
        (40, 40, 40),
        2,
    )
    cv2.imwrite(rutas[2], img3)

    return rutas


def abrir_imagenes(rutas):
    """Lee una lista de imágenes desde el disco."""
    imagenes = []

    for ruta in rutas:
        imagen = cv2.imread(ruta)

        if imagen is None:
            raise FileNotFoundError(f"No se pudo abrir la imagen: {ruta}")

        imagenes.append(imagen)

    return imagenes


def redimensionar_a_mayor(imagenes):
    """Redimensiona todas las imágenes al tamaño de la imagen con mayor área."""
    imagen_mayor = max(
        imagenes,
        key=lambda imagen: imagen.shape[0] * imagen.shape[1],
    )

    alto, ancho = imagen_mayor.shape[:2]

    redimensionadas = []

    for imagen in imagenes:
        alto_actual, ancho_actual = imagen.shape[:2]

        if ancho_actual > ancho or alto_actual > alto:
            interpolacion = cv2.INTER_AREA
        else:
            interpolacion = cv2.INTER_LINEAR

        nueva = cv2.resize(
            imagen,
            (ancho, alto),
            interpolation=interpolacion,
        )
        redimensionadas.append(nueva)

    return redimensionadas, (ancho, alto)


def combinar_canales(imagenes):
    """Combina R de la imagen 1, G de la imagen 2 y B de la imagen 3."""
    imagen_1, imagen_2, imagen_3 = imagenes

    # OpenCV usa el orden BGR.
    canal_rojo = imagen_1[:, :, 2]
    canal_verde = imagen_2[:, :, 1]
    canal_azul = imagen_3[:, :, 0]

    return cv2.merge([canal_azul, canal_verde, canal_rojo])


def convertir_negativo(imagen):
    """Invierte los valores de todos los píxeles."""
    return 255 - imagen


def convertir_grises(imagen):
    """Convierte una imagen BGR a escala de grises."""
    return cv2.cvtColor(imagen, cv2.COLOR_BGR2GRAY)


def aplicar_umbral_binario(imagen, valor_umbral=127):
    """Convierte una imagen en blanco y negro usando un umbral."""
    gris = convertir_grises(imagen)
    _, binaria = cv2.threshold(
        gris,
        valor_umbral,
        255,
        cv2.THRESH_BINARY,
    )
    return binaria


def dibujar_circulo_y_texto(imagen, texto="Figura: Persona"):
    """Dibuja un círculo aproximado sobre la zona central superior."""
    resultado = imagen.copy()
    alto, ancho = resultado.shape[:2]

    centro = (ancho // 2, alto // 3)
    radio = max(20, min(ancho, alto) // 5)

    cv2.circle(resultado, centro, radio, (0, 0, 255), 4)
    cv2.putText(
        resultado,
        texto,
        (30, alto - 35),
        cv2.FONT_HERSHEY_SIMPLEX,
        1,
        (0, 0, 255),
        3,
    )

    return resultado


def cargar_detector_facial():
    """Carga el clasificador Haar incluido con OpenCV."""
    ruta = os.path.join(
        cv2.data.haarcascades,
        "haarcascade_frontalface_default.xml",
    )

    detector = cv2.CascadeClassifier(ruta)

    if detector.empty():
        raise RuntimeError("No se pudo cargar el detector facial de OpenCV.")

    return detector


def detectar_rostros(imagen, detector):
    """Devuelve una copia de la imagen con los rostros marcados."""
    resultado = imagen.copy()
    gris = cv2.cvtColor(imagen, cv2.COLOR_BGR2GRAY)
    gris = cv2.equalizeHist(gris)

    rostros = detector.detectMultiScale(
        gris,
        scaleFactor=1.1,
        minNeighbors=5,
        minSize=(40, 40),
    )

    for x, y, ancho, alto in rostros:
        cv2.rectangle(
            resultado,
            (x, y),
            (x + ancho, y + alto),
            (0, 255, 0),
            3,
        )
        cv2.putText(
            resultado,
            "Rostro",
            (x, max(25, y - 10)),
            cv2.FONT_HERSHEY_SIMPLEX,
            0.7,
            (0, 255, 0),
            2,
        )

    return resultado, len(rostros)


def generar_resultados(rutas=None):
    """Genera y guarda todos los resultados principales del laboratorio."""
    if rutas is None:
        rutas = crear_imagenes_demo()

    imagenes = abrir_imagenes(rutas)
    redimensionadas, tamano = redimensionar_a_mayor(imagenes)

    for indice, imagen in enumerate(redimensionadas, start=1):
        cv2.imwrite(
            os.path.join(
                OUT_DIR,
                f"imagen_{indice}_redimensionada.png",
            ),
            imagen,
        )

    combinada = combinar_canales(redimensionadas)
    cv2.imwrite(
        os.path.join(OUT_DIR, "imagen_combinada_rgb.png"),
        combinada,
    )

    negativa = convertir_negativo(combinada)
    cv2.imwrite(
        os.path.join(OUT_DIR, "imagen_negativa.png"),
        negativa,
    )

    grises = convertir_grises(negativa)
    cv2.imwrite(
        os.path.join(OUT_DIR, "imagen_grises.png"),
        grises,
    )

    anotada = dibujar_circulo_y_texto(redimensionadas[0])
    cv2.imwrite(
        os.path.join(OUT_DIR, "imagen_circulo_texto.png"),
        anotada,
    )

    binaria = aplicar_umbral_binario(combinada, 127)
    cv2.imwrite(
        os.path.join(OUT_DIR, "imagen_umbral_binario.png"),
        binaria,
    )

    print("Resultados generados en la carpeta outputs.")
    print(f"Tamaño final: {tamano[0]} x {tamano[1]} píxeles")

    return combinada


# ==========================================================
# PROGRAMA DE DIBUJO INTERACTIVO CON OPENCV
# ==========================================================

class DibujadorInteractivo:
    """Permite dibujar líneas, rectángulos y círculos con el mouse."""

    def __init__(self, ancho=900, alto=600):
        self.lienzo = np.full((alto, ancho, 3), 255, dtype=np.uint8)
        self.temporal = self.lienzo.copy()
        self.historial = []
        self.modo = "linea"
        self.dibujando = False
        self.inicio = None

    def mouse_callback(self, event, x, y, flags, param):
        if event == cv2.EVENT_LBUTTONDOWN:
            self.historial.append(self.lienzo.copy())
            self.dibujando = True
            self.inicio = (x, y)

        elif event == cv2.EVENT_MOUSEMOVE and self.dibujando:
            self.temporal = self.lienzo.copy()
            self._dibujar_figura(
                self.temporal,
                self.inicio,
                (x, y),
            )

        elif event == cv2.EVENT_LBUTTONUP and self.dibujando:
            self.dibujando = False
            self._dibujar_figura(
                self.lienzo,
                self.inicio,
                (x, y),
            )
            self.temporal = self.lienzo.copy()

    def _dibujar_figura(self, imagen, punto_1, punto_2):
        if punto_1 is None:
            return

        color = (0, 0, 0)
        grosor = 3

        if self.modo == "linea":
            cv2.line(imagen, punto_1, punto_2, color, grosor)

        elif self.modo == "rectangulo":
            cv2.rectangle(imagen, punto_1, punto_2, color, grosor)

        elif self.modo == "circulo":
            delta_x = punto_2[0] - punto_1[0]
            delta_y = punto_2[1] - punto_1[1]
            radio = int(np.sqrt(delta_x**2 + delta_y**2))
            cv2.circle(imagen, punto_1, radio, color, grosor)

    def ejecutar(self):
        ventana = "Dibujo interactivo"
        cv2.namedWindow(ventana)
        cv2.setMouseCallback(ventana, self.mouse_callback)

        while True:
            vista = self.temporal.copy()

            cv2.rectangle(vista, (0, 0), (vista.shape[1], 85), (255, 255, 255), -1)

            cv2.putText(
                vista,
                f"Modo actual: {self.modo}",
                (15, 30),
                cv2.FONT_HERSHEY_SIMPLEX,
                0.8,
                (0, 0, 255),
                2,
            )

            cv2.putText(
                vista,
                "1 Linea | 2 Rectangulo | 3 Circulo | Z Deshacer | S Guardar | C Limpiar | ESC Salir",
                (15, 65),
                cv2.FONT_HERSHEY_SIMPLEX,
                0.55,
                (60, 60, 60),
                2,
            )

            cv2.imshow(ventana, vista)
            tecla = cv2.waitKey(20) & 0xFF

            if tecla == 27:
                break
            if tecla == ord("1"):
                self.modo = "linea"
            elif tecla == ord("2"):
                self.modo = "rectangulo"
            elif tecla == ord("3"):
                self.modo = "circulo"
            elif tecla in (ord("z"), ord("Z")):
                if self.historial:
                    self.lienzo = self.historial.pop()
                    self.temporal = self.lienzo.copy()
            elif tecla in (ord("s"), ord("S")):
                ruta = os.path.join(OUT_DIR, "dibujo_final.png")
                cv2.imwrite(ruta, self.lienzo)
                print(f"Dibujo guardado en: {ruta}")
            elif tecla in (ord("c"), ord("C")):
                self.historial.append(self.lienzo.copy())
                self.lienzo[:] = 255
                self.temporal = self.lienzo.copy()

        cv2.destroyWindow(ventana)


# ==========================================================
# INTERFAZ GRÁFICA PRINCIPAL
# ==========================================================

class AplicacionProcesamiento:
    """Interfaz con botones para canales, filtros y detección facial."""

    def __init__(self, ventana):
        self.ventana = ventana
        self.ventana.title("Laboratorio 8 - Procesamiento de imágenes")
        self.ventana.geometry("1180x820")
        self.ventana.minsize(980, 700)

        self.detector_facial = cargar_detector_facial()

        self.imagen_original = None
        self.imagen_resultado = None
        self.imagen_tk = None

        self.mostrar_rojo = True
        self.mostrar_verde = True
        self.mostrar_azul = True
        self.usar_deteccion = False
        self.usar_negativo = False
        self.usar_grises = False
        self.usar_binario = False

        self.crear_interfaz()
        self.configurar_atajos()
        self.mostrar_lienzo_inicial()

    def crear_interfaz(self):
        contenedor = tk.Frame(self.ventana)
        contenedor.pack(fill="both", expand=True, padx=10, pady=10)

        panel_archivo = tk.LabelFrame(
            contenedor,
            text="Archivos y laboratorio",
            padx=8,
            pady=8,
        )
        panel_archivo.pack(fill="x", pady=(0, 8))

        tk.Button(
            panel_archivo,
            text="Abrir una imagen",
            width=18,
            command=self.abrir_imagen,
        ).grid(row=0, column=0, padx=4, pady=4)

        tk.Button(
            panel_archivo,
            text="Seleccionar 3 imágenes",
            width=20,
            command=self.seleccionar_tres_imagenes,
        ).grid(row=0, column=1, padx=4, pady=4)

        tk.Button(
            panel_archivo,
            text="Usar imágenes demo",
            width=18,
            command=self.usar_imagenes_demo,
        ).grid(row=0, column=2, padx=4, pady=4)

        tk.Button(
            panel_archivo,
            text="Guardar resultado",
            width=18,
            command=self.guardar_resultado,
        ).grid(row=0, column=3, padx=4, pady=4)

        tk.Button(
            panel_archivo,
            text="Abrir dibujo interactivo",
            width=22,
            command=self.abrir_dibujador,
        ).grid(row=0, column=4, padx=4, pady=4)

        panel_canales = tk.LabelFrame(
            contenedor,
            text="Canales de color",
            padx=8,
            pady=8,
        )
        panel_canales.pack(fill="x", pady=(0, 8))

        self.boton_rojo = tk.Button(
            panel_canales,
            width=18,
            command=lambda: self.alternar_canal("rojo"),
        )
        self.boton_rojo.grid(row=0, column=0, padx=4, pady=4)

        self.boton_verde = tk.Button(
            panel_canales,
            width=18,
            command=lambda: self.alternar_canal("verde"),
        )
        self.boton_verde.grid(row=0, column=1, padx=4, pady=4)

        self.boton_azul = tk.Button(
            panel_canales,
            width=18,
            command=lambda: self.alternar_canal("azul"),
        )
        self.boton_azul.grid(row=0, column=2, padx=4, pady=4)

        panel_efectos = tk.LabelFrame(
            contenedor,
            text="Filtros y detección",
            padx=8,
            pady=8,
        )
        panel_efectos.pack(fill="x", pady=(0, 8))

        self.boton_rostros = tk.Button(
            panel_efectos,
            width=20,
            command=self.alternar_deteccion,
        )
        self.boton_rostros.grid(row=0, column=0, padx=4, pady=4)

        self.boton_negativo = tk.Button(
            panel_efectos,
            width=18,
            command=self.alternar_negativo,
        )
        self.boton_negativo.grid(row=0, column=1, padx=4, pady=4)

        self.boton_grises = tk.Button(
            panel_efectos,
            width=18,
            command=self.alternar_grises,
        )
        self.boton_grises.grid(row=0, column=2, padx=4, pady=4)

        self.boton_binario = tk.Button(
            panel_efectos,
            width=18,
            command=self.alternar_binario,
        )
        self.boton_binario.grid(row=0, column=3, padx=4, pady=4)

        tk.Button(
            panel_efectos,
            text="Restablecer",
            width=16,
            command=self.restablecer,
        ).grid(row=0, column=4, padx=4, pady=4)

        self.etiqueta_estado = tk.Label(
            contenedor,
            text="Abra una imagen o seleccione tres imágenes.",
            anchor="w",
            font=("Arial", 10),
        )
        self.etiqueta_estado.pack(fill="x", pady=(0, 6))

        marco_imagen = tk.Frame(
            contenedor,
            bd=2,
            relief="sunken",
            bg="#202020",
        )
        marco_imagen.pack(fill="both", expand=True)

        self.etiqueta_imagen = tk.Label(
            marco_imagen,
            bg="#202020",
        )
        self.etiqueta_imagen.pack(fill="both", expand=True, padx=8, pady=8)

        self.actualizar_botones()

    def configurar_atajos(self):
        self.ventana.bind("<r>", lambda event: self.alternar_canal("rojo"))
        self.ventana.bind("<R>", lambda event: self.alternar_canal("rojo"))
        self.ventana.bind("<g>", lambda event: self.alternar_canal("verde"))
        self.ventana.bind("<G>", lambda event: self.alternar_canal("verde"))
        self.ventana.bind("<b>", lambda event: self.alternar_canal("azul"))
        self.ventana.bind("<B>", lambda event: self.alternar_canal("azul"))
        self.ventana.bind("<Escape>", lambda event: self.ventana.destroy())

    def mostrar_lienzo_inicial(self):
        lienzo = np.full((520, 900, 3), 235, dtype=np.uint8)
        cv2.putText(
            lienzo,
            "Abra una imagen para comenzar",
            (165, 255),
            cv2.FONT_HERSHEY_SIMPLEX,
            1.2,
            (40, 40, 40),
            2,
        )
        cv2.putText(
            lienzo,
            "Tambien puede seleccionar tres imagenes para generar el laboratorio",
            (95, 310),
            cv2.FONT_HERSHEY_SIMPLEX,
            0.72,
            (70, 70, 70),
            2,
        )
        self.mostrar_en_interfaz(lienzo)

    def abrir_imagen(self):
        ruta = filedialog.askopenfilename(
            title="Seleccionar imagen",
            filetypes=[
                ("Imágenes", "*.png *.jpg *.jpeg *.bmp *.webp"),
                ("Todos los archivos", "*.*"),
            ],
        )

        if not ruta:
            return

        imagen = cv2.imread(ruta)

        if imagen is None:
            messagebox.showerror(
                "Error",
                "No se pudo abrir la imagen seleccionada.",
            )
            return

        self.imagen_original = imagen
        self.restablecer(actualizar=False)
        self.actualizar_imagen()

    def seleccionar_tres_imagenes(self):
        rutas = filedialog.askopenfilenames(
            title="Seleccione exactamente tres imágenes",
            filetypes=[
                ("Imágenes", "*.png *.jpg *.jpeg *.bmp *.webp"),
                ("Todos los archivos", "*.*"),
            ],
        )

        if not rutas:
            return

        if len(rutas) != 3:
            messagebox.showwarning(
                "Cantidad incorrecta",
                "Debe seleccionar exactamente tres imágenes.",
            )
            return

        try:
            combinada = generar_resultados(list(rutas))
        except (FileNotFoundError, ValueError, RuntimeError) as error:
            messagebox.showerror("Error", str(error))
            return

        self.imagen_original = combinada
        self.restablecer(actualizar=False)
        self.actualizar_imagen()

        messagebox.showinfo(
            "Proceso completado",
            "Las imágenes fueron procesadas. Los resultados están en la carpeta outputs.",
        )

    def usar_imagenes_demo(self):
        combinada = generar_resultados(crear_imagenes_demo())
        self.imagen_original = combinada
        self.restablecer(actualizar=False)
        self.actualizar_imagen()

        messagebox.showinfo(
            "Imágenes demo",
            "Se generaron los resultados usando las imágenes de demostración.",
        )

    def alternar_canal(self, canal):
        if not self.validar_imagen():
            return

        if canal == "rojo":
            self.mostrar_rojo = not self.mostrar_rojo
        elif canal == "verde":
            self.mostrar_verde = not self.mostrar_verde
        elif canal == "azul":
            self.mostrar_azul = not self.mostrar_azul

        self.actualizar_botones()
        self.actualizar_imagen()

    def alternar_deteccion(self):
        if not self.validar_imagen():
            return

        self.usar_deteccion = not self.usar_deteccion
        self.actualizar_botones()
        self.actualizar_imagen()

    def alternar_negativo(self):
        if not self.validar_imagen():
            return

        self.usar_negativo = not self.usar_negativo
        self.actualizar_botones()
        self.actualizar_imagen()

    def alternar_grises(self):
        if not self.validar_imagen():
            return

        self.usar_grises = not self.usar_grises

        if self.usar_grises:
            self.usar_binario = False

        self.actualizar_botones()
        self.actualizar_imagen()

    def alternar_binario(self):
        if not self.validar_imagen():
            return

        self.usar_binario = not self.usar_binario

        if self.usar_binario:
            self.usar_grises = False

        self.actualizar_botones()
        self.actualizar_imagen()

    def restablecer(self, actualizar=True):
        self.mostrar_rojo = True
        self.mostrar_verde = True
        self.mostrar_azul = True
        self.usar_deteccion = False
        self.usar_negativo = False
        self.usar_grises = False
        self.usar_binario = False

        self.actualizar_botones()

        if actualizar and self.imagen_original is not None:
            self.actualizar_imagen()

    def validar_imagen(self):
        if self.imagen_original is None:
            messagebox.showwarning(
                "Advertencia",
                "Primero debe abrir una imagen.",
            )
            return False

        return True

    def procesar_imagen(self):
        if self.imagen_original is None:
            return None, 0

        azul, verde, rojo = cv2.split(self.imagen_original)
        canal_vacio = np.zeros_like(azul)

        resultado = cv2.merge(
            [
                azul if self.mostrar_azul else canal_vacio,
                verde if self.mostrar_verde else canal_vacio,
                rojo if self.mostrar_rojo else canal_vacio,
            ]
        )

        if self.usar_negativo:
            resultado = convertir_negativo(resultado)

        if self.usar_grises:
            gris = convertir_grises(resultado)
            resultado = cv2.cvtColor(gris, cv2.COLOR_GRAY2BGR)

        if self.usar_binario:
            binaria = aplicar_umbral_binario(resultado, 127)
            resultado = cv2.cvtColor(binaria, cv2.COLOR_GRAY2BGR)

        cantidad_rostros = 0

        if self.usar_deteccion:
            gris_original = cv2.cvtColor(
                self.imagen_original,
                cv2.COLOR_BGR2GRAY,
            )
            gris_original = cv2.equalizeHist(gris_original)

            rostros = self.detector_facial.detectMultiScale(
                gris_original,
                scaleFactor=1.1,
                minNeighbors=5,
                minSize=(40, 40),
            )

            cantidad_rostros = len(rostros)

            for x, y, ancho, alto in rostros:
                cv2.rectangle(
                    resultado,
                    (x, y),
                    (x + ancho, y + alto),
                    (0, 255, 0),
                    3,
                )
                cv2.putText(
                    resultado,
                    "Rostro",
                    (x, max(25, y - 10)),
                    cv2.FONT_HERSHEY_SIMPLEX,
                    0.7,
                    (0, 255, 0),
                    2,
                )

        return resultado, cantidad_rostros

    def actualizar_imagen(self):
        resultado, cantidad_rostros = self.procesar_imagen()

        if resultado is None:
            return

        self.imagen_resultado = resultado.copy()
        self.mostrar_en_interfaz(resultado)

        alto, ancho = self.imagen_original.shape[:2]
        self.etiqueta_estado.config(
            text=(
                f"Imagen: {ancho} x {alto} | "
                f"R: {'Sí' if self.mostrar_rojo else 'No'} | "
                f"G: {'Sí' if self.mostrar_verde else 'No'} | "
                f"B: {'Sí' if self.mostrar_azul else 'No'} | "
                f"Rostros detectados: {cantidad_rostros}"
            )
        )

    def mostrar_en_interfaz(self, imagen_bgr):
        alto, ancho = imagen_bgr.shape[:2]

        max_ancho = 1080
        max_alto = 560
        escala = min(max_ancho / ancho, max_alto / alto, 1.0)

        nuevo_ancho = max(1, int(ancho * escala))
        nuevo_alto = max(1, int(alto * escala))

        redimensionada = cv2.resize(
            imagen_bgr,
            (nuevo_ancho, nuevo_alto),
            interpolation=cv2.INTER_AREA,
        )

        rgb = cv2.cvtColor(redimensionada, cv2.COLOR_BGR2RGB)
        imagen_pil = Image.fromarray(rgb)
        self.imagen_tk = ImageTk.PhotoImage(imagen_pil)

        self.etiqueta_imagen.config(image=self.imagen_tk)

    def guardar_resultado(self):
        if self.imagen_resultado is None:
            messagebox.showwarning(
                "Advertencia",
                "No existe una imagen procesada para guardar.",
            )
            return

        ruta = filedialog.asksaveasfilename(
            title="Guardar resultado",
            initialdir=OUT_DIR,
            initialfile="resultado_interactivo.png",
            defaultextension=".png",
            filetypes=[
                ("PNG", "*.png"),
                ("JPG", "*.jpg"),
            ],
        )

        if not ruta:
            return

        if cv2.imwrite(ruta, self.imagen_resultado):
            messagebox.showinfo(
                "Guardado",
                f"La imagen se guardó correctamente en:\n{ruta}",
            )
        else:
            messagebox.showerror(
                "Error",
                "No se pudo guardar la imagen.",
            )

    def abrir_dibujador(self):
        self.etiqueta_estado.config(
            text="Programa de dibujo abierto. Cierre la ventana de OpenCV para volver."
        )
        dibujador = DibujadorInteractivo()
        dibujador.ejecutar()
        self.etiqueta_estado.config(
            text="Programa de dibujo cerrado."
        )

    def actualizar_botones(self):
        self.boton_rojo.config(
            text=f"Rojo: {'ACTIVO' if self.mostrar_rojo else 'INACTIVO'}"
        )
        self.boton_verde.config(
            text=f"Verde: {'ACTIVO' if self.mostrar_verde else 'INACTIVO'}"
        )
        self.boton_azul.config(
            text=f"Azul: {'ACTIVO' if self.mostrar_azul else 'INACTIVO'}"
        )
        self.boton_rostros.config(
            text=f"Detección facial: {'SÍ' if self.usar_deteccion else 'NO'}"
        )
        self.boton_negativo.config(
            text=f"Negativo: {'SÍ' if self.usar_negativo else 'NO'}"
        )
        self.boton_grises.config(
            text=f"Grises: {'SÍ' if self.usar_grises else 'NO'}"
        )
        self.boton_binario.config(
            text=f"Umbral: {'SÍ' if self.usar_binario else 'NO'}"
        )


# ==========================================================
# EJECUCIÓN
# ==========================================================

def main():
    ventana = tk.Tk()
    AplicacionProcesamiento(ventana)
    ventana.mainloop()


if __name__ == "__main__":
    main()
