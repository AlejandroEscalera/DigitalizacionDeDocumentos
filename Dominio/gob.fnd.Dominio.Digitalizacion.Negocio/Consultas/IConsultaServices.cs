using gob.fnd.Dominio.Digitalizacion.Entidades.BienesAdjudicados;
using gob.fnd.Dominio.Digitalizacion.Entidades.Cancelados;
using gob.fnd.Dominio.Digitalizacion.Entidades.Cancelados.RerporteFinal;
using gob.fnd.Dominio.Digitalizacion.Entidades.Consultas;
using gob.fnd.Dominio.Digitalizacion.Entidades.Consultas.ReporteFinal;
using gob.fnd.Dominio.Digitalizacion.Entidades.Imagenes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gob.fnd.Dominio.Digitalizacion.Negocio.Consultas
{
    public interface IConsultaServices
    {
        /// <summary>
        /// Prepara y carga la información de consultas
        /// 
        /// Se carga internamente toda la información de Saldos con Castigo (ya procesado)
        /// Se carga internamente toda la información de Saldos Activos (ya procesado)
        /// Se carga internamente toda la información de la mesa
        /// 
        /// Se realiza un cruce de consultas entre los tres repositorios
        /// </summary>
        /// <returns>Listado de expedientes a consultar</returns>
        IEnumerable<ExpedienteDeConsulta> CargaInformacion();
        /// <summary>
        /// Carga la información del archivo de imágenes
        /// </summary>
        /// <returns>Listado con las imágenes y su ubicación</returns>
        IEnumerable<ArchivoImagenCorta> CargaInformacionImagenes();

        /// <summary>
        /// Calcula la tabla para las regiones
        /// </summary>
        /// <param name="expedientes">Listado de expedientes</param>
        /// <returns>Detalle de regiones</returns>
        IEnumerable<ExpCasRegiones> CalculaRegiones(IEnumerable<ExpedienteDeConsulta> expedientes);
        /// <summary>
        /// Calcula la tabla para las Agencias
        /// </summary>
        /// <param name="expedientes">Listado de expedientes</param>
        /// <param name="region">Region del cual se listarán las agencias</param>
        /// <returns>Detalle de las agencias</returns>
        IEnumerable<ExpCasAgencias> CalculaAgencias(IEnumerable<ExpedienteDeConsulta> expedientes, int region);
        /// <summary>
        /// Calcula la tabla para los expedientes
        /// </summary>
        /// <param name="expedientes">Listado de expedientes</param>
        /// <param name="agencia">Agencia de la cual se listaran los expedientes</param>
        /// <returns>Detalle de expediente por agencia</returns>
        IEnumerable<ExpCasExpedientes> CalculaExpedientes(IEnumerable<ExpedienteDeConsulta> expedientes, int agencia);
        /// <summary>
        /// Obtiene de los creditos vigentes el resumen de los totales (Se obtienen de los expedientes)
        /// </summary>
        /// <returns>Objeto que muestra los creditos totales</returns>
        CreditosTotales CalculaCreditosTotales();
        IEnumerable<ResumenDeCreditos> CalculaCreditosRegiones();

        IEnumerable<CreditosRegion> CalculaCreditosRegion(int region);

        IEnumerable<DetalleCreditos> CalculaCreditosAgencia(int agencia);

        IEnumerable<ArchivoImagenCorta> BuscaImagenes(string numeroDeCredito, bool directas = false);

        IEnumerable<CreditosCanceladosAplicacion> CargaCreditosCancelados();
        int CalculaCreditosCancelados();
        IEnumerable<CreditosCanceladosResumen> CalculaCreditosCanceladosRegiones();
        IEnumerable<CreditosCanceladosRegion> CalculaCreditosCanceladosRegion(int region);
        IEnumerable<CreditosCanceladosDetalle> CalculaCreditosCanceladosAgencia(int agencia);

        int CalculaBienesAdjudicados();
        IEnumerable<ResumenBienesAdjudicados> CalculaResumenBienesAdjudicados();
        IEnumerable<EntidadesBienesAdjudicados> CalculaEntidadesBienesAdjudicados(int region);
        IEnumerable<DetalleBienesAdjudicados> CalculaDetalleBienesAdjudicados(int region, string? entidad);

        string ObtieneUnidadImagenes();

        /// <summary>
        /// Carga la información del archivo de imágenes
        /// </summary>
        /// <returns>Listado con las imágenes y su ubicación</returns>
        IEnumerable<ArchivoImagenBienesAdjudicadosCorta> CargaInformacionImagenesBienesAdjudicados();
        /// <summary>
        /// Carga la información del archivo de imágenes
        /// </summary>
        /// <returns>Listado con las imágenes y su ubicación</returns>
        IEnumerable<ArchivoImagenBienesAdjudicadosCorta> BuscaImagenesBienesAdjudicados(string numeroDeExpediente, string acreditados, bool directas = false);

    }
}
