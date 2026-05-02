\# CGVCM



Este repositorio contiene los laboratorios desarrollados para el curso, organizados por carpetas independientes.



Cada laboratorio cuenta con su propia carpeta, donde se encuentran los archivos principales del proyecto, el informe correspondiente y, cuando aplica, el ejecutable generado.



\## Estructura del repositorio



```text

CGVCM/

│

├── lab01/

│   ├── Assets/

│   ├── Packages/

│   ├── ProjectSettings/

│   ├── Informe/

│   └── Ejecutable/

│

├── lab02/

│   ├── Informe/

│   └── Ejecutable/

│

└── README.md

```



\## Informes y ejecutables



Para ver los informes y ejecutables de cada laboratorio, se debe ingresar a la carpeta correspondiente de cada laboratorio.



Por ejemplo:



```text

lab01/Informe/

lab01/Ejecutable/

```



Dentro de cada carpeta se encontrará el informe en formato PDF y el ejecutable del laboratorio, en caso haya sido generado.



\## Descargar solo el informe y el ejecutable



Si solo se desea extraer el informe y el ejecutable de un laboratorio específico, se puede usar el siguiente comando:



```bash

git clone --filter=blob:none --sparse https://github.com/jperezh04/CGVCM.git

cd CGVCM

git sparse-checkout set lab01/Informe lab01/Ejecutable

```



Para otro laboratorio, solo se cambia el nombre de la carpeta. Por ejemplo:



```bash

git sparse-checkout set lab02/Informe lab02/Ejecutable

```



\## Nota



Las carpetas generadas automáticamente por Unity, como `Library`, `Temp`, `Obj`, `Logs` y `UserSettings`, no forman parte del repositorio porque Unity puede volver a generarlas al abrir el proyecto.

