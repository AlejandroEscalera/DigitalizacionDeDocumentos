using gob.fnd.Dominio.Digitalizacion.Entidades.BienesAdjudicados;
using gob.fnd.Dominio.Digitalizacion.Entidades.Cancelados;
using gob.fnd.Dominio.Digitalizacion.Entidades.Cancelados.RerporteFinal;
using gob.fnd.Dominio.Digitalizacion.Entidades.Consultas;
using gob.fnd.Dominio.Digitalizacion.Entidades.Consultas.ReporteFinal;
using gob.fnd.Dominio.Digitalizacion.Entidades.Consultas.ReporteLiquidados;
using gob.fnd.Dominio.Digitalizacion.Entidades.Consultas.Tratamientos;
using gob.fnd.Dominio.Digitalizacion.Entidades.GuardaValores;
using gob.fnd.Dominio.Digitalizacion.Entidades.Imagenes;
using gob.fnd.Dominio.Digitalizacion.Entidades.Juridico;
using gob.fnd.Dominio.Digitalizacion.Liquidaciones;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gob.fnd.Dominio.Digitalizacion.Negocio.Consultas
{
    public interface IConsultaServices
    {
        #region Carga de informacion inicial
        /// <summary>
        /// Se busca tener la unidad de imágenes válida con las imágenes
        /// </summary>
        /// <returns>Nombre del drive de la unidad</returns>
        string ObtieneUnidadImagenes();
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
        IEnumerable<ExpedienteDeConsultaGv> CargaInformacion();
      
        /// <summary>
        /// Carga la información del archivo de imágenes
        /// </summary>
        /// <returns>Listado con las imágenes y su ubicación</returns>
        IEnumerable<ArchivoImagenCorta> CargaInformacionImagenes();

        /// <summary>
        /// Carga la información del archivo de imágenes
        /// </summary>
        /// <returns>Listado con las imágenes y su ubicación</returns>

        Task<IEnumerable<ArchivoImagenCorta>> CargaInformacionImagenesAsync();

        /// <summary>
        /// Prepara y carga la información de consultas
        /// 
        /// Se carga internamente toda la información de Saldos con Castigo (ya procesado)
        /// Se carga internamente toda la información de Saldos Activos (ya procesado)
        /// Se carga internamente toda la información de la mesa
        /// 
        /// Se realiza un cruce de consultas entre los tres repositorios
        /// </summary>
        /// <param name="cruzaConImagenes">Cruza la información del expediente con la imagen, para luego volver a guardarse</param>
        /// <returns>Listado de expedientes a consultar</returns>
        Task<IEnumerable<ExpedienteDeConsultaGv>> CargaInformacionAsync(bool cruzaConImagenes = false);
        #endregion

        #region Informacion de Créditos Castigados
        /// <summary>
        /// Calcula la tabla para las regiones
        /// </summary>
        /// <param name="expedientes">Listado de expedientes</param>
        /// <returns>Detalle de regiones</returns>
        IEnumerable<ExpCasRegiones> CalculaRegiones(IEnumerable<ExpedienteDeConsultaGv> expedientes);
        /// <summary>
        /// Calcula la tabla para las Agencias
        /// </summary>
        /// <param name="expedientes">Listado de expedientes</param>
        /// <param name="region">Region del cual se listarán las agencias</param>
        /// <returns>Detalle de las agencias</returns>
        IEnumerable<ExpCasAgencias> CalculaAgencias(IEnumerable<ExpedienteDeConsultaGv> expedientes, int region);
        /// <summary>
        /// Calcula la tabla para los expedientes
        /// </summary>
        /// <param name="expedientes">Listado de expedientes</param>
        /// <param name="agencia">Agencia de la cual se listaran los expedientes</param>
        /// <returns>Detalle de expediente por agencia</returns>
        IEnumerable<ExpCasExpedientes> CalculaExpedientes(IEnumerable<ExpedienteDeConsultaGv> expedientes, int agencia);
        #endregion

        #region Vigente, Impago, Vencido
        /// <summary>
        /// Obtiene de los creditos vigentes el resumen de los totales (Se obtienen de los expedientes)
        /// </summary>
        /// <returns>Objeto que muestra los creditos totales</returns>
        CreditosTotales CalculaCreditosTotales();
        IEnumerable<ResumenDeCreditos> CalculaCreditosRegiones();

        IEnumerable<CreditosRegion> CalculaCreditosRegion(int region);

        IEnumerable<DetalleCreditos> CalculaCreditosAgencia(int agencia);

        IEnumerable<ArchivoImagenCorta> BuscaImagenes(string numeroDeCredito, bool directas = false);

        #endregion

        #region Informacion de Créditos Cancelados

        IEnumerable<CreditosCanceladosAplicacion> CargaCreditosCancelados();                
        int CalculaCreditosCancelados();
        IEnumerable<CreditosCanceladosResumen> CalculaCreditosCanceladosRegiones();
        IEnumerable<CreditosCanceladosRegion> CalculaCreditosCanceladosRegion(int region);
        IEnumerable<CreditosCanceladosDetalle> CalculaCreditosCanceladosAgencia(int agencia);
        Task<IEnumerable<CreditosCanceladosAplicacion>> CargaCreditosCanceladosAsync(bool cruzaConImagenes = false);
        #endregion

        #region Infomación de los Bienes Adjudicados
        int CalculaBienesAdjudicados();
        IEnumerable<ResumenBienesAdjudicados> CalculaResumenBienesAdjudicados();
        IEnumerable<EntidadesBienesAdjudicados> CalculaEntidadesBienesAdjudicados(int region);
        IEnumerable<DetalleBienesAdjudicados> CalculaDetalleBienesAdjudicados(int region, string? entidad);
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
        #endregion

        #region Información de los Créditos Liquidados
        IEnumerable<ColocacionConPagos> CargaColocacionConPagos();
        int CalculaCreditosLiquidados();
        IEnumerable<CreditosLiquidadosRegiones> CalculaCreditosLiquidadosRegiones();
        IEnumerable<CreditosLiquidadosAgencias> CalculaCreditosLiquidadosRegion(int region);
        IEnumerable<CreditosLiquidadosDetalle> CalculaCreditosLiquidadosAgencia(int agencia);
        Task<IEnumerable<ColocacionConPagos>> CargaColocacionConPagosAsync(bool cruzaConImagenes = false);
        #endregion
        void CreaTemporal();

        #region Tratamientos
        int CalculaTratamientos();
        IEnumerable<ResumenDeCreditos> CalculaCreditosRegionesTratamientos();

        IEnumerable<CreditosRegion> CalculaCreditosRegionTratamientos(int region);

        IEnumerable<DetalleCreditosTratamientos> CalculaCreditosAgenciaTratamientos(int agencia);

        IEnumerable<string> ObtieneCreditoOrigen(string numeroDeCreditoTratamiento);

        #endregion

        #region Expedientes Jurídicos
        IEnumerable<ExpedienteJuridico> CargaExpedientesJuridicos();
        IEnumerable<ArchivoImagenExpedientesCorta> CargaInformacionImagenesExpedientesJuridicos();
        int CalculaExpedientesJuridicos();
        IEnumerable<ExpedientesJuridicoRegion> CalculaExpedientesJuridicoRegiones();
        IEnumerable<ExpedientesJuridicoAgencia> CalculaExpedientesJuridicoAgencia(int region);
        IEnumerable<ExpedienteJuridicoDetalle> CalculaExpedienteJuridicoDetalle(int agencia);
        IEnumerable<ArchivoImagenExpedientesCorta> BuscaImagenesExpedientes(string contrato, string numCredito);
        ExpedienteJuridicoDetalle? BuscaExpedienteJuridico(string contrato, string numCredito);
        Task<IEnumerable<ExpedienteJuridico>> CargaExpedientesJuridicosAsync(bool cruzaConImagenes = false);
        Task<IEnumerable<ArchivoImagenExpedientesCorta>> CargaInformacionImagenesExpedientesJuridicosAsync();
        #endregion

        #region GuardaValores
        IEnumerable<GuardaValores> ObtieneGuardaValores(int Agencia);
        bool GuardaValores(IEnumerable<GuardaValores> valoresAGuardar, string nombreArchivoReporte);
        #endregion
    }
}
