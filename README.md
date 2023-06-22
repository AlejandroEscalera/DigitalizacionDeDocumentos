# Proyecto de digitalizacion de Expedientes

Este proyecto fue solicitado como un visor, que forma parte de los trabajos de entrega-recepción de la Financiera Nacional de Desarrollo Agropecuario, Rural, Forestal y Pesquero. 

Como parte del cumplimiento solicitado en el ACUERDO por el que se expide la Estrategia Digital Nacional 2021-2024, que en su objetivo específico número 3, de Promover la autonomía e independencia tecnológicas para establecer la rectoría del Estado en la definición de sus Tecnologías de la Información y Comunicación. Donde se busca.

a) Fomentar el desarrollo de sistemas de información gubernamentales propios y de acceso abierto que se compartan entre Instituciones.
b) Priorizar el uso de Software Libre y estándares abiertos.
c) Promover instrumentos de colaboración para compartir recursos e infraestructura tecnológica entre Instituciones.
d) Facilitar la reutilización del código de programación de las aplicaciones gubernamentales para su actualización, mejora o liberación entre las Instituciones.
e) Impulsar la migración hacia tecnologías basadas en Software Libre que otorguen mayor flexibilidad a la adecuación e implementación de los proyectos de TIC.
f) Alentar el intercambio de conocimientos técnicos entre Instituciones para fomentar la adopción de tecnologías basadas en Software Libre y estándares abiertos.
Así como 
g)  Promover la formación de nuevos expertos en TIC y atraer al mejor talento.

En concordancia a lo anterior, se pone a disposición de uso público el código correspondiente.

# Sobre el requerimiento.

El requerimiento en particular fue un sistema que se pudiera instalar de forma independiente en un equipo de cómputo, donde se puedan consultar todos los expedientes de la Financiera que corresponden al período posterior al 1 de junio del 2020 a la terminación de esta administración, donde no se requiera un motor de base de datos.

Inicialmente el alcance abarcaba los expedientes activos (Cartera Vigente, impagos y vencida) sin embargo, el alcance ahora abarca tratamientos, turnos de expedientes, créditos cancelados, créditos liquidados y bienes adjudicados.

En la pantalla se observará el flujo que normalmente siguen estos créditos hasta su liberación.

Adicionalmente se agregó búsquedas por número de crédito y por acreditado para mejorar la consulta de información en particular.

El sistema se puede modificar para que en el futuro se pueda conectar a motores de base de datos y con esto reutilizar como consulta de expedientes en la institución que reemplace a la financiera.

La información del proyecto no es parte del mismo, por contener información de la institución.

# Licenciamiento

El sistema fue desarrollado en .Net 6, el cual cuenta con una licencia abierta, para mas información consultar.
https://dotnet.microsoft.com/en-us/platform/open-source

# Paquetes utilizados públicos

[CefSharp.Common.NETCore], de The CefSharp Authors, licencia: (https://www.nuget.org/packages/CefSharp.WinForms.NETCore/111.2.70/license)
[CefSharp.WinForms.NETCore], de The CefSharp Authors, licencia: (https://www.nuget.org/packages/CefSharp.WinForms.NETCore/111.2.70/license)
[chromiumembeddedframework.runtime.win-arm64] de The Chromium Embedded Framework Authors, licencia: (https://www.nuget.org/packages/chromiumembeddedframework.runtime.win-arm64/111.2.7/license)
[chromiumembeddedframework.runtime.win-x64] de The Chromium Embedded Framework Authors, licencia: (https://www.nuget.org/packages/chromiumembeddedframework.runtime.win-x64/111.2.7/license)
[chromiumembeddedframework.runtime.win-x86] de The Chromium Embedded Framework Authors, licencia: (https://www.nuget.org/packages/chromiumembeddedframework.runtime.win-x86/111.2.7/license)
[CommunityToolkit.Mvvm] de Microsoft, licencia: (https://github.com/CommunityToolkit/dotnet/blob/main/License.md)



