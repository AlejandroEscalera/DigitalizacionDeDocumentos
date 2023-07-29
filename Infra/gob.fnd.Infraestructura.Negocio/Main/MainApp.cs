using Microsoft.Extensions.Logging;
using Jaec.Helper.IoC;
using gob.fnd.Dominio.Digitalizacion.Excel.Config;
using gob.fnd.Dominio.Digitalizacion.Excel.ABSaldosC;
using gob.fnd.Dominio.Digitalizacion.Entidades.Config;
using gob.fnd.Dominio.Digitalizacion.Negocio.Directorios;
using gob.fnd.Infraestructura.Negocio.Directorios;
using gob.fnd.Dominio.Digitalizacion.Excel.Arqueos;
using Microsoft.Extensions.Configuration;
using gob.fnd.Dominio.Digitalizacion.Excel.GuardaValores;
using gob.fnd.Dominio.Digitalizacion.Excel.Imagenes;
using gob.fnd.Dominio.Digitalizacion.Entidades.Arqueo;
using System.Collections.Generic;
using gob.fnd.Dominio.Digitalizacion.Negocio.CruceInformacion;
using gob.fnd.Infraestructura.Negocio.CruceInformacion;
using gob.fnd.Dominio.Digitalizacion.Excel;
using gob.fnd.Dominio.Digitalizacion.Excel.Cancelados;
using gob.fnd.Dominio.Digitalizacion.Excel.Ministraciones;
using gob.fnd.Dominio.Digitalizacion.Entidades.Imagenes;
using gob.fnd.Dominio.Digitalizacion.Entidades.ABSaldos;

namespace gob.fnd.Infraestructura.Negocio.Main
{
    public class MainApp : IMainControlApp
    {
        private readonly ILogger<MainApp> _logger;
        private readonly IConfiguration _configuration;
        private readonly IObtieneCatalogoProductos _obtieneCatalogoProductos;
        private readonly IServicioABSaldosConCastigo _servicioABSaldosConCastigo;
        private readonly IObtieneCorreosAgentes _obtieneCorreosAgentes;
        private readonly IExploraCarpetas _exploraCarpetas;
        private readonly IAnalisisArqueos _analisisArqueos;
        private readonly IAnalisisCamposGuardaValores _analisisCamposGuardaValores;
        private readonly IAdministraGuardaValores _administraGuardaValores;
        private readonly IServicioImagenes _servicioImagenes;
        private readonly IServicioABSaldosActivos _servicioABSaldosActivos;
        private readonly ICruceInformacion _cruceInformacion;
        private readonly IServicioABSaldosRegionales _servicioABSaldosRegionales;
        private readonly IServicioCancelados _servicioCancelados;
        private readonly IServicioMinistracionesMesa _servicioMinistracionesMesa;
        private readonly bool _seRealizaAnalisisArqueo;
        private readonly bool _registraTodasLasImagenes;
        private readonly bool _soloCargaImagenesProcesadas;
        private readonly bool _seRealizaArqueo;
        private readonly string _archivoSaldosConCastigoProcesados;

        public MainApp(ILogger<MainApp> logger,
            IConfiguration configuration,
            IObtieneCatalogoProductos obtieneCatalogoProductos,
            IServicioABSaldosConCastigo servicioABSaldosConCastigo,
            IObtieneCorreosAgentes obtieneCorreosAgentes,
            IExploraCarpetas exploraCarpetas,
            IAnalisisArqueos analisisArqueos,
            IAnalisisCamposGuardaValores analisisCamposGuardaValores,
            IAdministraGuardaValores administraGuardaValores,
            IServicioImagenes servicioImagenes,
            IServicioABSaldosActivos servicioABSaldosActivos,
            ICruceInformacion cruceInformacion,
            IServicioABSaldosRegionales servicioABSaldosRegionales,
            IServicioCancelados servicioCancelados,
            IServicioMinistracionesMesa servicioMinistracionesMesa

            // IConfiguration config, 
            // IObtieneParametrosCarta parametros
            )
        {
            _logger = logger;
            _configuration = configuration;
            _obtieneCatalogoProductos = obtieneCatalogoProductos;
            _servicioABSaldosConCastigo = servicioABSaldosConCastigo;
            _obtieneCorreosAgentes = obtieneCorreosAgentes;
            _exploraCarpetas = exploraCarpetas;
            _analisisArqueos = analisisArqueos;
            _analisisCamposGuardaValores = analisisCamposGuardaValores;
            _administraGuardaValores = administraGuardaValores;
            _servicioImagenes = servicioImagenes;
            _servicioABSaldosActivos = servicioABSaldosActivos;
            _cruceInformacion = cruceInformacion;
            _servicioABSaldosRegionales = servicioABSaldosRegionales;
            _servicioCancelados = servicioCancelados;
            _servicioMinistracionesMesa = servicioMinistracionesMesa;
            string valorDeSeRealizaAnalisisArqueo = _configuration.GetValue<string>("seRealizaAnalisisArqueo") ?? "0";
            _seRealizaAnalisisArqueo = valorDeSeRealizaAnalisisArqueo == "1";
            _registraTodasLasImagenes = (_configuration.GetValue<string>("registraTodasLasImagenes") ?? "").Equals("Si");

            _soloCargaImagenesProcesadas = (_configuration.GetValue<string>("soloCargaImagenesProcesadas") ?? "").Equals("Si");
            _seRealizaArqueo = (_configuration.GetValue<string>("seRealizaArqueo") ?? "No") == "Si";
            _archivoSaldosConCastigoProcesados = _configuration.GetValue<string>("archivoSaldosConCastigoProcesados") ?? "";
            //             _parametros = parametros;
        }

        private async Task MetodoNormal()
        {
            // Aqui comienza la aplicación
            _logger.LogWarning("Inicio el proceso: {fecha} {hora}", DateTime.Now.ToLongDateString(), DateTime.Now.ToLongTimeString());


            #region Obtengo el Catalogo de productos
            var productos = _obtieneCatalogoProductos.ObtieneElCatalogoDelProducto();
            _logger.LogInformation("Se obtivieron {numProductos} productos de crédito", productos.Count());
            #endregion

            #region Obtengo los ABSaldos de las regionales
            IEnumerable<ABSaldosRegionales> saldoRegionales = await _servicioABSaldosRegionales.CargaProcesaYLimpia();

            _logger.LogInformation("La cantidad de archivos de saldos regionales son {cantidadArchivosRegionales}", saldoRegionales.Count());
            #endregion

            #region Obtengo los Creditos Cancelados
            var creditosCancelados = _servicioCancelados.GetCreditosCancelados();
            _logger.LogInformation("La cantidad de creditos cancelados son {cantidadCreditosCancelados}", creditosCancelados.Count());
            #endregion

            #region Obtengo las Ministraciones de la Mesa
            var ministracionesMesa = _servicioMinistracionesMesa.GetMinistraciones();
            _logger.LogInformation("La cantidad de creditos ministrados por la mesa son {cantidadMinistracionesMesa}", ministracionesMesa.Count());
            #endregion

            #region Obtengo la lista de archivos de los arqueos
            var archivos = _exploraCarpetas.ObtieneListaArchivosDeArqueos();
            _logger.LogInformation("La cantidad de archivos de excel son {cantidadArchivos}", archivos.Count());
            #endregion

            #region Obtengo los parámetros de trabajo para filtrar AB Saldos
            var filtro = _obtieneCatalogoProductos.ObtieneListaAFiltrar();
            var fechaFiltro = _obtieneCatalogoProductos.ObtieneFechaFiltro();
            var correos = _obtieneCorreosAgentes.ObtieneTodosLosCorreosYAgentes();
            _logger.LogInformation("La fecha a filtrar es {fechaFiltro:dd/MM/yyyy}", fechaFiltro);
            _logger.LogInformation("La cantidad de tipos de producto a filtrar son {cantidadFiltro}", filtro.Count());
            #endregion

            #region Obtengo la información de AB Saldos Corporativo
            var saldosConCastigo = _servicioABSaldosConCastigo.CargaProcesaYLimpia("D:\\202302 Digitalizacion\\1. AB Saldos\\Con castigos\\Saldos_Diarios con Castigos al 28Feb2023.xlsx");
            _logger.LogInformation("La cantidad de tipos de producto a filtrar son {cantidadFiltro}", saldosConCastigo.Count());
            _servicioABSaldosConCastigo.AgregaAgentesYGuardaValores(saldosConCastigo, correos);
            #endregion

            #region Obtengo la información de AB Saldos Natural -- Me la paso Carlos
            var saldosActivos = _servicioABSaldosActivos.GetABSaldosActivos();
            _logger.LogInformation("!!!!! Leyo la información de AB Saldos Natural");
            _servicioABSaldosActivos.AgregaAgentesYGuardaValores(saldosActivos, correos);
            #endregion

            #region Analiza cada archivo de arqueo
            if (_seRealizaAnalisisArqueo)
                _analisisArqueos.AnalisaArchivosArqueos(archivos);
            #endregion

            #region Guardo la informacion del analisis de los arqueos
            if (_seRealizaAnalisisArqueo)
            {
                string archivoAnalisisArqueo = _configuration.GetValue<string>("archivoAnalisisArqueo") ?? "";
                _analisisArqueos.GuardaInformacionArqueos(archivos, archivoAnalisisArqueo);
            }
            #endregion

            #region Se carga la informacion de los campos para la integración del arqueo
            var informacionArchivosYCampos = _analisisCamposGuardaValores.CargaInformacion();
            _logger.LogInformation("La cantidad de archivos por cargar son {cantidadArchivosMasCampos}", informacionArchivosYCampos.Count());
            #endregion

            #region Obtengo la información de todos los guarda valores para concentrarlo en un solo archivo            
            IEnumerable<InformacionGuardaValor>? detalleGuardaValores = null;
            if (_seRealizaArqueo)
            {
                detalleGuardaValores = _administraGuardaValores.CargaInformacionGuardaValores(informacionArchivosYCampos);
                _logger.LogInformation("La cantidad de registros del guarda valores es {cantidadRegistrosGuardaValores}", detalleGuardaValores.Count());
            }
            #endregion

            #region Obtiene Imagenes
            // IEnumerable<ArchivosImagenes> imagenesExceles;
            IEnumerable<ArchivosImagenes> imagenesExps;
            if (!_soloCargaImagenesProcesadas)
            {
                var imagenesExceles = _servicioImagenes.ObtieneInformacionDeTodosLosArchivosDeImagen(null, false);
                _logger.LogInformation("Se encontraron las siguientes imagenes de exceles {imagenesExceles}", imagenesExceles.Count());
                imagenesExps = _servicioImagenes.ObtieneInformacionDeTodosLosArchivosDeImagen(imagenesExceles, true);
                _logger.LogInformation("Se encontraron las siguientes imagenes de exceles {imagenesExpedientes}", (imagenesExps.Count() - imagenesExceles.Count()) + 1);
            }
            else
            {
                imagenesExps = _servicioImagenes.CargaImagenesTratadas();
                _logger.LogInformation("Se cargaron {totalImagenes} imagenes", imagenesExps.Count());
            }
            #endregion

            /* AQUI SE CRUZA LA INFORMACION ANTES DE GUARDAR */
            #region Cruce de informacion
            _cruceInformacion.CruzaInformacionSaldosRegionalesSaldosNaturales(ref saldoRegionales, ref saldosActivos);
            _logger.LogInformation("Termino de cruzar Saldos Regionales vs Saldos Activos!");
            _cruceInformacion.CruzaInformacionSaldosRegionalesSaldosConCastigo(ref saldoRegionales, ref saldosConCastigo);
            _logger.LogInformation("Termino de cruzar Saldos Regionales vs Saldos con Castigo (Contables)!");




            _cruceInformacion.CruzaInformacionSaldosActivosCreditosCancelados(ref saldosActivos, ref creditosCancelados);
            _logger.LogInformation("Termino de cruzar Saldos Activos y Créditos Cancelados!");
            _cruceInformacion.CruzaInformacionSaldosConCastigoCreditosCancelados(ref saldosConCastigo, ref creditosCancelados);
            _logger.LogInformation("Termino de cruzar Saldos con Castigo (Contables) y Créditos Cancelados!");
            _cruceInformacion.CruzaInformacionSaldosConCastigoConPagados(ref saldosConCastigo, ref ministracionesMesa);
            _logger.LogInformation("Termino de cruzar Saldos con Castigo (Contables) y Mesa!");
            _cruceInformacion.CruzaInformacionSaldosActivosConPagados(ref saldosActivos, ref ministracionesMesa);
            _logger.LogInformation("Termino de cruzar Saldos Activos y Mesa!");
            if (!_registraTodasLasImagenes || _soloCargaImagenesProcesadas)
                _cruceInformacion.CruzaInformacionMesaConImagenes(ref ministracionesMesa, ref imagenesExps);
            _logger.LogInformation("Termino de cruzar Mesa con las Imágenes!");

            // Se agrega el tipo de producto si no se tiene
            _cruceInformacion.CruzaInformacionAbSaldos(ref saldosConCastigo, ref saldosActivos);

            // Se le agrega la descripción del producto desde los catálogos
            _cruceInformacion.CruzaInformacionTipoProductoSaldosActivos(ref saldosActivos, ref productos);
            _cruceInformacion.CruzaInformacionTipoProductoSaldosConCastigo(ref saldosConCastigo, ref productos);

            if (detalleGuardaValores is not null)
            {
                _cruceInformacion.CruzaInformacionAbSaldosCorporativoGV(ref saldosConCastigo, ref detalleGuardaValores);
                _cruceInformacion.CruzaInformacionAbSaldosNaturalGV(ref saldosActivos, ref detalleGuardaValores);
                _cruceInformacion.IncluyeRegionYAgenciaGV(ref detalleGuardaValores);
            }
            if (!_registraTodasLasImagenes || _soloCargaImagenesProcesadas)
                _cruceInformacion.CruzaInformacionAbSaldosImagenes(ref saldosActivos, ref imagenesExps);
            if (!_registraTodasLasImagenes || _soloCargaImagenesProcesadas)
                _cruceInformacion.CruzaInformacionAbSaldosCorporativoImagenes(ref saldosConCastigo, ref imagenesExps);
            if (detalleGuardaValores is not null)
            {
                _cruceInformacion.CruzaInformacionMinistracionesGuardaValores(ref ministracionesMesa, ref detalleGuardaValores);
                if (!_registraTodasLasImagenes || _soloCargaImagenesProcesadas)
                    _cruceInformacion.CruzaInformacionGvImagenes(ref detalleGuardaValores, ref imagenesExps);
            }
            #endregion

            #region Guardoo la informacion de todos los guarda valores en un solo archivo
            if (_seRealizaArqueo)
            {
                if (detalleGuardaValores is not null)
                {
                    _administraGuardaValores.GuardaInformacionGuardaValores(detalleGuardaValores);
                    string archivoConcentradoGV = _configuration.GetValue<string>("archivoConcentradoGV") ?? "";
                    _logger.LogInformation("Se guardo el archivo de guarda valores {archivoConcentradoGuardaValores}", archivoConcentradoGV);
                }
            }
            #endregion
            #region Guardo la información de AB Saldos
            _servicioABSaldosConCastigo.GuardaInformacionABSaldos(_archivoSaldosConCastigoProcesados, saldosConCastigo);
            #endregion

            #region Guardo la informacion de Imagenes
            if (!_soloCargaImagenesProcesadas)
            {
                imagenesExps = _servicioImagenes.RenumeraImagenes(imagenesExps);
            }
            _servicioImagenes.GuardarImagenes(imagenesExps);
            _logger.LogInformation("Se guardaron {totalImagenes} imagenes", imagenesExps.Count());
            #endregion

            #region Guardo la información de ABSaldos Natural
            _ = _servicioABSaldosActivos.GuardaABSaldosActivos(saldosActivos);
            _logger.LogInformation("Se escribió la información de los saldos de créditos activos!!!");
            #endregion

            #region Guardo la información de Cancelaciones
            _ = _servicioCancelados.GuardaCreditosCancelados(creditosCancelados);
            _logger.LogInformation("Se escribió la información de los créditos cancelados!!!");
            #endregion 

            #region Guardo la información de Mesa
            _ = _servicioMinistracionesMesa.GuardaMinistraciones(ministracionesMesa);
            _logger.LogInformation("Se escribió la información de las ministraciones de la mesa!!!");
            #endregion 

            #region Se obtienen los parámetros a manejar
            // ParametrosCarta datosParaLaCarta = _parametros.ObtieneLosParametrosDeLaCarta();
            // IObtieneCorreosAgentes obtieneCorreosAgentes = _obtieneCorreosAgentes;
            // var correos = obtieneCorreosAgentes.ObtieneTodosLosCorreosYAgentes();
            // IDocumentoWord word = _plantillaDocumentoWord;
            #endregion

            _logger.LogWarning("Finalizó correctamente la generación de informacion: {dia} {hora}", DateTime.Now.ToLongDateString(), DateTime.Now.ToLongTimeString());
            return;

        }

        public void Run()
        {
            MetodoNormal().Wait();
        }

    }
}
