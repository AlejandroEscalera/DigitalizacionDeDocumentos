using gob.fnd.Dominio.Digitalizacion.Entidades.ABSaldos;
using gob.fnd.Dominio.Digitalizacion.Entidades.ABSaldosC;
using gob.fnd.Dominio.Digitalizacion.Entidades.Arqueo;
using gob.fnd.Dominio.Digitalizacion.Entidades.Cancelados;
using gob.fnd.Dominio.Digitalizacion.Entidades.Config;
using gob.fnd.Dominio.Digitalizacion.Entidades.Imagenes;
using gob.fnd.Dominio.Digitalizacion.Entidades.Ministraciones;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gob.fnd.Dominio.Digitalizacion.Negocio.CruceInformacion
{
    public interface ICruceInformacion
    {
        /// <summary>
        /// Cruza la información de los créditos aprovados en la mesa vs los archivos de imágenes
        /// </summary>
        /// <param name="pagados">Información de la Mesa</param>
        /// <param name="archivosImagenes">Las imagenes encontradas</param>
        void CruzaInformacionMesaConImagenes(ref IEnumerable<MinistracionesMesa> pagados, ref IEnumerable<ArchivosImagenes> archivosImagenes);
        /// <summary>
        /// Cruza la información de los créditos autorizados por la mesa contra los saldos con castigo (contables)
        /// </summary>
        /// <param name="saldosConCastigo">Saldos Activos</param>
        /// <param name="pagados">Ministraciones pagadas</param>
        void CruzaInformacionSaldosConCastigoConPagados(ref IEnumerable<ABSaldosConCastigo> saldosConCastigo, ref IEnumerable<MinistracionesMesa> pagados);
        /// <summary>
        /// Cruza la información de los créditos autorizados por la mesa contra los saldos activos
        /// </summary>
        /// <param name="saldosActivos">Saldos Activos</param>
        /// <param name="pagados">Ministraciones pagadas</param>
        void CruzaInformacionSaldosActivosConPagados(ref IEnumerable<ABSaldosActivos> saldosActivos, ref IEnumerable<MinistracionesMesa> pagados);
        /// <summary>
        /// Se agrega la información de los saldos cancelados a los saldos con castigo, 
        /// porque cambia el número de credito y las imagenes tendrán que ver con las 
        /// actas de las sesiones de crédito
        /// </summary>
        /// <param name="saldosConCastigo">Saldos con Castigo</param>
        /// <param name="creditosCancelados">Creditos Cancelados</param>
        void CruzaInformacionSaldosConCastigoCreditosCancelados(ref IEnumerable<ABSaldosConCastigo> saldosConCastigo, ref IEnumerable<CreditosCancelados> creditosCancelados);
        /// <summary>
        /// Se agrega la información de los saldos cancelados a los saldos activos, 
        /// porque cambia el número de credito y las imagenes tendrán que ver con las 
        /// actas de las sesiones de crédito
        /// </summary>
        /// <param name="saldosActivos">Saldos Activos</param>
        /// <param name="creditosCancelados">Saldos Cancelados</param>
        void CruzaInformacionSaldosActivosCreditosCancelados(ref IEnumerable<ABSaldosActivos> saldosActivos, ref IEnumerable<CreditosCancelados> creditosCancelados);
        /// <summary>
        /// Complementa la información para los saldos con castigo proveniente de saldos regionales
        /// </summary>
        /// <param name="saldosRegionales">AB Saldos que maneja la región para sus controles</param>
        /// <param name="saldosConCastigo">AB Saldos clasificado para la contabilidad</param>
        void CruzaInformacionSaldosRegionalesSaldosConCastigo(ref IEnumerable<ABSaldosRegionales> saldosRegionales, ref IEnumerable<ABSaldosConCastigo> saldosConCastigo);
        /// <summary>
        /// Complementa la información de los saldos naturales proveniente de saldos regionales
        /// </summary>
        /// <param name="saldosRegionales">El resultado de la carga de todos los salods regionales</param>
        /// <param name="saldosActivos">Información de los saldos activos</param>
        void CruzaInformacionSaldosRegionalesSaldosNaturales(ref IEnumerable<ABSaldosRegionales> saldosRegionales, ref IEnumerable<ABSaldosActivos> saldosActivos);
        /// <summary>
        /// Se cruza la informacion de los saldos naturales contra las imagenes para complementar amba informacion
        /// </summary>
        /// <param name="saldosNaturales">Lista de créditos del ABSaldos</param>
        /// <param name="imagenes">Lista de las imagenes encontradas</param>
        void CruzaInformacionAbSaldosImagenes(ref IEnumerable<ABSaldosActivos> saldosNaturales, ref IEnumerable<ArchivosImagenes> imagenes);
        /// <summary>
        /// Se cruza la información de los saldos corporativos vs saldos naturales, para agregar la columna de castigo
        /// </summary>
        /// <param name="saldosCorporativo">Archivo de saldos con castigo</param>
        /// <param name="saldosNaturales">Archivo que sale del terfin cada semana</param>
        void CruzaInformacionAbSaldos(ref IEnumerable<ABSaldosConCastigo> saldosCorporativo, ref IEnumerable<ABSaldosActivos> saldosNaturales);
        /// <summary>
        /// Se cruza la informacion de los saldos corporativo contra las imagenes para complementar amba informacion
        /// </summary>
        /// <param name="saldosCorporativo">Lista de créditos del ABSaldos con castigo</param>
        /// <param name="imagenes">Lista de las imagenes encontradas</param>
        void CruzaInformacionAbSaldosCorporativoImagenes(ref IEnumerable<ABSaldosConCastigo> saldosCorporativo, ref IEnumerable<ArchivosImagenes> imagenes);
        /// <summary>
        /// Comparo el ABSaldos Corporativo vs Guarda valores
        /// </summary>
        /// <param name="saldosCorporativo"></param>
        /// <param name="detalleGuardaValores"></param>
        void CruzaInformacionAbSaldosCorporativoGV(ref IEnumerable<ABSaldosConCastigo> saldosCorporativo, ref IEnumerable<InformacionGuardaValor> detalleGuardaValores);
        /// <summary>
        /// Comparo el ABSaldos Natural vs Guarda valores
        /// </summary>
        /// <param name="saldosNaturales"></param>
        /// <param name="detalleGuardaValores"></param>
        void CruzaInformacionAbSaldosNaturalGV(ref IEnumerable<ABSaldosActivos> saldosNaturales, ref IEnumerable<InformacionGuardaValor> detalleGuardaValores);
        /// <summary>
        /// Incluyo region y agencia de GV
        /// </summary>
        /// <param name="detalleGuardaValores"></param>
        void IncluyeRegionYAgenciaGV(ref IEnumerable<InformacionGuardaValor> detalleGuardaValores);
        /// <summary>
        /// Hago el cruce entre el detalle de las imagenes y los GV, para corregir si tiene expediente digital
        /// </summary>
        /// <param name="detalleGuardaValores"></param>
        /// <param name="imagenes"></param>
        void CruzaInformacionGvImagenes(ref IEnumerable<InformacionGuardaValor> detalleGuardaValores, ref IEnumerable<ArchivosImagenes> imagenes);
        /// <summary>
        /// Cruza la información del tipo de producto desde un catálogo
        /// </summary>
        /// <param name="saldosActivos">Lista de créditos activos</param>
        /// <param name="productos">lista de productos</param>
        void CruzaInformacionTipoProductoSaldosActivos(ref IEnumerable<ABSaldosActivos> saldosActivos, ref IEnumerable<CatalogoProductos> productos);
        /// <summary>
        /// Cruza la información del tipo de producto desde un catálogo
        /// </summary>
        /// <param name="saldosConCastigo">Lista de créditos clasificados con castigo</param>
        /// <param name="productos">lista de productos</param>
        void CruzaInformacionTipoProductoSaldosConCastigo(ref IEnumerable<ABSaldosConCastigo> saldosConCastigo, ref IEnumerable<CatalogoProductos> productos);
        /// <summary>
        /// Se cruza con la información de los Guarda Valores para sacar si son del período del Dr.
        /// </summary>
        /// <param name="pagosMesa">Los pagos realizados en la mesa</param>
        /// <param name="guardaValores">Los guarda valores</param>
        void CruzaInformacionMinistracionesGuardaValores(ref IEnumerable<MinistracionesMesa> pagosMesa, ref IEnumerable<InformacionGuardaValor> guardaValores);
    }
}
