# Documentacion Aqui se documenta para que sirve este proyecto

De entrada en este proyecto es solo un bootstrap, el cual manda llamar el programa principal desde
la librería gob.fnd.Infraestructura.Negocio.Procesa.

En ella, existen varias carpetas, una de ellas se llama Main, y la otra IOC.

   En IOC, mediane la clase OrquestaDI se registran los servicios que se van a utilizar, 
   y en Main se ejecuta el programa principal.

   En Main se mandan llamar los servicios que se registraron en IOC, y se ejecutan basados en banderas.

   El archivo de configuración se llama appsettings.json, en el cual se registran las librerías a cargar 
   de forma dinámica, basados en la ruta de instalación.

   En este mismo archivo se incluyen todas las variables que afectan la operación del programa.

Rutas relevantes

Bootstraper
C:\gob\ProyectoDigitalizacion\Presenta\GeneraInformacionDiagnostico\

Main
C:\gob\ProyectoDigitalizacion\Infra\gob.fnd.Infraestructura.Negocio.Procesa\gob.fnd.Infraestructura.Negocio.Procesa

IOC
C:\gob\ProyectoDigitalizacion\Infra\gob.fnd.Infraestructura.Negocio.Procesa\gob.fnd.Infraestructura.Negocio.Procesa.IOC

Cada librería de infraestructura que sea llamada por este programa tiene su propia carpeta IOC, por lo cual
cada librería se encarga de publicar sus propios servicios.

Ojo: en este proyecto no se incluye la información, ya que esta contiene información sensible, relacionado con créditos 
de la FND. Sin embargo, derivado que el código es open source, se dejan visibles el código y las estructuras para
posibles referencias de la Banca de Desarrollo.

El proyecto está amparado en que se hizo de manera individual, y las licencias utilizadas son open source, con excepción
de la librería IronOcr, queda pendiente de hacer las las llamadas directas a Tesseract sin pasar por la librería, pero
si alguien quiere hacer uso en producción del código como está, es libre de comprar esa licencia.

