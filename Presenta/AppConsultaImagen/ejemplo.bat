@echo off

set "projectDir=%~1"
set "outDir=%~3"
set "environment=%2"
echo Configuration: %2

rem Combinar las rutas del directorio del proyecto y el directorio de salida
set "libDir=%projectDir%%outDir%lib"

rem Validar si el directorio "lib" existe
if not exist "%libDir%" (
    rem Crear el directorio "lib" si no existe
    mkdir "%libDir%"
    echo Directorio "lib" creado exitosamente.
) else (
    echo Directorio "lib" ya existe.
)
rem Validar si el directorio "lib\Infra" existe
if not exist "%libDir%\Infra" (
    rem Crear el directorio "lib\Infra" si no existe
    mkdir "%libDir%\Infra"
    echo Directorio "lib\Infra" creado exitosamente.
) else (
    echo Directorio "lib\Infra" ya existe.
)
rem Validar si el directorio "lib\Infra\gob.fnd.Infraestructura.Negocio" existe
if not exist "%libDir%\Infra\gob.fnd.Infraestructura.Negocio" (
    rem Crear el directorio "lib\Infra\gob.fnd.Infraestructura.Negocio" si no existe
    mkdir "%libDir%\Infra\gob.fnd.Infraestructura.Negocio"
    echo Directorio "lib\Infra\gob.fnd.Infraestructura.Negocio" creado exitosamente.
) else (
    echo Directorio "lib\Infra\gob.fnd.Infraestructura.Negocio" ya existe.
)
rem Validar si el directorio "lib\Infra\gob.fnd.Infraestructura.Negocio.CargaCsv" existe
if not exist "%libDir%\Infra\gob.fnd.Infraestructura.Negocio.CargaCsv" (
    rem Crear el directorio "lib\Infra\gob.fnd.Infraestructura.Negocio.CargaCsv" si no existe
    mkdir "%libDir%\Infra\gob.fnd.Infraestructura.Negocio.CargaCsv"
    echo Directorio "lib\Infra\gob.fnd.Infraestructura.Negocio.CargaCsv" creado exitosamente.
) else (
    echo Directorio "lib\Infra\gob.fnd.Infraestructura.Negocio.CargaCsv" ya existe.
)
rem Validar si el directorio "lib\Infra\gob.fnd.Infraestructura.Digitalizacion.Excel" existe
if not exist "%libDir%\Infra\gob.fnd.Infraestructura.Digitalizacion.Excel" (
    rem Crear el directorio "lib\Infra\gob.fnd.Infraestructura.Digitalizacion.Excel" si no existe
    mkdir "%libDir%\Infra\gob.fnd.Infraestructura.Digitalizacion.Excel"
    echo Directorio "lib\Infra\gob.fnd.Infraestructura.Digitalizacion.Excel" creado exitosamente.
) else (
    echo Directorio "lib\Infra\gob.fnd.Infraestructura.Digitalizacion.Excel" ya existe.
)
if not exist "%libDir%\Infra\gob.fnd.Infraestructura.Negocio.Procesa.Zip" (
    rem Crear el directorio "lib\Infra\gob.fnd.Infraestructura.Digitalizacion.Excel" si no existe
    mkdir "%libDir%\Infra\gob.fnd.Infraestructura.Negocio.Procesa.Zip"
    echo Directorio "lib\Infra\gob.fnd.Infraestructura.Negocio.Procesa.Zip" creado exitosamente.
) else (
    echo Directorio "lib\Infra\gob.fnd.Infraestructura.Negocio.Procesa.Zip" ya existe.
)


rem Obtener el valor de la variable de entorno "Configuration"
echo Este es el ambiente: %environment%
echo ============================================
rem Comprobar si estamos en un entorno de depuración o lanzamiento
if "%environment%"==""Debug"" (
    echo Estas en un entorno de depuracion.
xcopy "%cd%\..\..\Infra\gob.fnd.Infraestructura.Negocio\bin\Debug\net6.0\gob.fnd.Infraestructura.Negocio.dll" "%libDir%\Infra\gob.fnd.Infraestructura.Negocio\gob.fnd.Infraestructura.Negocio.dll" /d/y/-i

xcopy "%cd%\..\..\Infra\gob.fnd.Infraestructura.Negocio.CargaCsv\bin\Debug\net6.0\gob.fnd.Infraestructura.Negocio.CargaCsv.dll" "%libDir%\Infra\gob.fnd.Infraestructura.Negocio.CargaCsv\gob.fnd.Infraestructura.Negocio.CargaCsv.dll" /d/y/-i

xcopy "%cd%\..\..\Infra\gob.fnd.Infraestructura.Digitalizacion.Excel\bin\Debug\net6.0\gob.fnd.Infraestructura.Digitalizacion.Excel.dll" "%libDir%\Infra\gob.fnd.Infraestructura.Digitalizacion.Excel\gob.fnd.Infraestructura.Digitalizacion.Excel.dll" /d/y/-i

xcopy "%cd%\..\..\Infra\gob.fnd.Infraestructura.Digitalizacion.Excel\bin\Debug\net6.0\gob.fnd.ExcelHelper.dll" "%libDir%\Infra\gob.fnd.Infraestructura.Digitalizacion.Excel\gob.fnd.ExcelHelper.dll" /d/y/-i

xcopy "%cd%\..\..\Infra\gob.fnd.Infraestructura.Negocio.Procesa.Zip\bin\Debug\net6.0\gob.fnd.Infraestructura.Negocio.Procesa.Zip.dll" "%libDir%\Infra\gob.fnd.Infraestructura.Negocio.Procesa.Zip\gob.fnd.Infraestructura.Negocio.Procesa.Zip.dll" /d/y/-i


) else if "%environment%"==""Release"" (
    echo Estas en un entorno de lanzamiento.
xcopy "%cd%\..\..\Infra\gob.fnd.Infraestructura.Negocio\bin\Release\net6.0\gob.fnd.Infraestructura.Negocio.dll" "%libDir%\Infra\gob.fnd.Infraestructura.Negocio\gob.fnd.Infraestructura.Negocio.dll" /d/y/-i

xcopy "%cd%\..\..\Infra\gob.fnd.Infraestructura.Negocio.CargaCsv\bin\Release\net6.0\gob.fnd.Infraestructura.Negocio.CargaCsv.dll" "%libDir%\Infra\gob.fnd.Infraestructura.Negocio.CargaCsv\gob.fnd.Infraestructura.Negocio.CargaCsv.dll" /d/y/-i

xcopy "%cd%\..\..\Infra\gob.fnd.Infraestructura.Digitalizacion.Excel\bin\Release\net6.0\gob.fnd.Infraestructura.Digitalizacion.Excel.dll" "%libDir%\Infra\gob.fnd.Infraestructura.Digitalizacion.Excel\gob.fnd.Infraestructura.Digitalizacion.Excel.dll" /d/y/-i

xcopy "%cd%\..\..\Infra\gob.fnd.Infraestructura.Digitalizacion.Excel\bin\Release\net6.0\gob.fnd.ExcelHelper.dll" "%libDir%\Infra\gob.fnd.Infraestructura.Digitalizacion.Excel\gob.fnd.ExcelHelper.dll" /d/y/-i

xcopy "%cd%\..\..\Infra\gob.fnd.Infraestructura.Negocio.Procesa.Zip\bin\Release\net6.0\gob.fnd.Infraestructura.Negocio.Procesa.Zip.dll" "%libDir%\Infra\gob.fnd.Infraestructura.Negocio.Procesa.Zip\gob.fnd.Infraestructura.Negocio.Procesa.Zip.dll" /d/y/-i

) else (
    echo No se pudo determinar el entorno de compilacion. %environment%
)
echo ============================================

