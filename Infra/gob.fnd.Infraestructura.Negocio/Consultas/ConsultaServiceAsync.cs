using gob.fnd.Dominio.Digitalizacion.Entidades.ABSaldos;
using gob.fnd.Dominio.Digitalizacion.Entidades.ABSaldosC;
using gob.fnd.Dominio.Digitalizacion.Entidades.Cancelados;
using gob.fnd.Dominio.Digitalizacion.Entidades.Config;
using gob.fnd.Dominio.Digitalizacion.Entidades.Consultas;
using gob.fnd.Dominio.Digitalizacion.Entidades.CorteDiario.ABSaldos;
using gob.fnd.Dominio.Digitalizacion.Entidades.GuardaValores;
using gob.fnd.Dominio.Digitalizacion.Entidades.Imagenes;
using gob.fnd.Dominio.Digitalizacion.Entidades.Juridico;
using gob.fnd.Dominio.Digitalizacion.Entidades.Ministraciones;
using gob.fnd.Dominio.Digitalizacion.Entidades.ReportesAvance;
using gob.fnd.Dominio.Digitalizacion.Liquidaciones;
using gob.fnd.Dominio.Digitalizacion.Negocio.Consultas;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace gob.fnd.Infraestructura.Negocio.Consultas;

partial class ConsultaServices : IConsultaServices
{
    public async Task<IEnumerable<ArchivoImagenCorta>> CargaInformacionImagenesAsync()
    {
        #region En caso de existir, se carga el archivo de imagenes cortas
        FileInfo fi = new(_archivoImagenCorta);
        if (fi.Exists)
        {
            _imagenesCortas = await Task.Run(() => { return _administraCargaConsultaService.CargaArchivoImagenCorta(_archivoImagenCorta); });
            _logger.LogInformation("Cargo la información de los expedientes previamente procesada");

            #region Se retira, para optimizar una carga en paralelo
            ///  La idea es cargar primero las imagenes, luego los expedientes en paralelo, y en cada expediente hacer la cruza de información con las imagenes
            /*
            if (_saldosConBandera is not null && (!_saldosConBandera.Where(x => x.StatusImpago == true || x.StatusCarteraVencida == true || x.StatusCarteraVigente == true).Any(y => y.TieneImagenDirecta == true)))
            {
                CruzaInformacionImagenes(_imagenesCortas);
            }
            */
            #endregion

            await Task.Run(() =>
            {
                CambiaUnidad();
            });

            return _imagenesCortas;
        }
        #endregion

        IEnumerable<ArchivosImagenes> resultado = await _servicioImagenes.CargaImagenesTratadasAsync(_archivoSoloPDFProcesado); 
        var resultadoCorto = await Task.Run(() =>
        {
            return resultado.Select(x => new ArchivoImagenCorta()
            {
                CarpetaDestino = x.CarpetaDestino,
                Hash = x.Hash,
                Id = x.Id,
                NombreArchivo = x.NombreArchivo,
                NumPaginas = x.NumPaginas,
                NumCredito = x.NumCredito
            }).ToList();
        });

        #region Se retira, para optimizar una carga en paralelo
        ///  La idea es cargar primero las imagenes, luego los expedientes en paralelo, y en cada expediente hacer la cruza de información con las imagenes
        /*
        if (_saldosConBandera is not null)
        {
            CruzaInformacionImagenes(resultadoCorto);
        }
        */
        #endregion

        await Task.Run(() =>
        {
            _ = _creaArchivoCsvService.CreaArchivoCsv(_archivoImagenCorta, resultadoCorto);
        });
        _imagenesCortas = resultadoCorto;

        await Task.Run(() =>
        {
            CambiaUnidad();
        });
        return _imagenesCortas;
    }

    #region Carga Informacion de Expedientes y sus helpers
    public async Task<IEnumerable<ExpedienteDeConsultaGv>> CargaInformacionAsync(bool cruzaConImagenes = false)
    {
        _expedienteDeConsulta = null; // new List<ExpedienteDeConsulta>();
        bool ultimosSaldos = false;
        if (await _administraABSaldosDiarioService.CopiaABSaldosDiarioAync())
        {
            _nombreArchivoReporte = _archivoABSaldosDiarioDestino;
            _fechaReporte = String.Format("{0:dd}/{0:MM}/{0:yyyy}", ObtieneFechaDeUnArchivo(_nombreArchivoReporte));
            var abSaldosCompleta = await Task.Run(() => { return _administraCargaCorteDiarioService.CargaABSaldosCompleta(_archivoABSaldosDiarioDestino); });
            await LlenaInformacionABSaldosFiltroCreditoAync(abSaldosCompleta);
            ultimosSaldos = true;
        }
        else
        {
            if (!File.Exists(_archivoABSaldosDiarioDestino))
            {
                if (await _administraABSaldosDiarioService.DescargaABSaldosDiarioAync())
                {
                    string lastFileName = ObtieneUltimoABSaldos();
                    _nombreArchivoReporte = lastFileName;
                    _fechaReporte = String.Format("{0:dd}/{0:MM}/{0:yyyy}", ObtieneFechaDeUnArchivo(_nombreArchivoReporte));
                    var abSaldosCompleta = await Task.Run(() => { return _administraCargaCorteDiarioService.CargaABSaldosCompleta(lastFileName); });
                    await LlenaInformacionABSaldosFiltroCreditoAync(abSaldosCompleta);
                    ultimosSaldos = true;
                }
            }
            else
            {
                string lastFileName = await ObtieneUltimoABSaldosAsync();
                if (string.IsNullOrEmpty(lastFileName))
                {
                    lastFileName = _archivoSaldosDiarioProcesados;
                }

                if (File.Exists(lastFileName))
                {
                    _nombreArchivoReporte = lastFileName;
                    _fechaReporte = String.Format("{0:dd}/{0:MM}/{0:yyyy}", ObtieneFechaDeUnArchivo(_nombreArchivoReporte));
                    var abSaldosCompleta = await Task.Run(() => { return _administraCargaCorteDiarioService.CargaABSaldosCompleta(lastFileName); });

                    await LlenaInformacionABSaldosFiltroCreditoAync(abSaldosCompleta);
                    ultimosSaldos = true;
                    // ultimosSaldos = true;
                }
                else
                {
                    if (File.Exists(_archivoABSaldosDiarioDestino))
                    {
                        _nombreArchivoReporte = _archivoABSaldosDiarioDestino;
                        _fechaReporte = String.Format("{0:dd}/{0:MM}/{0:yyyy}", ObtieneFechaDeUnArchivo(_nombreArchivoReporte));
                        var abSaldosCompleta = await Task.Run(() => { return _administraCargaCorteDiarioService.CargaABSaldosCompleta(_archivoABSaldosDiarioDestino); });
                        await LlenaInformacionABSaldosFiltroCreditoAync(abSaldosCompleta);
                        ultimosSaldos = true;
                    }
                }
            }
        }

        FileInfo fi = new(_archivoExpedientesConsultaGvCsv);
        FileInfo fi2 = new(_archivoExpedientesConsultaCsv);
        if (fi.Exists || fi2.Exists)
        {
            IEnumerable<ExpedienteDeConsultaGv> resultado;
            if (!fi.Exists && fi2.Exists)
            {
                IEnumerable<ExpedienteDeConsulta> compatibilidad = (IEnumerable<ExpedienteDeConsultaGv>)await Task.Run(() => { return _administraCargaConsultaService.CargaExpedienteDeConsulta(_archivoExpedientesConsultaCsv); });
                resultado = compatibilidad.Select(x => new ExpedienteDeConsultaGv(x));
            }
            else
            {
                resultado = await Task.Run(() =>{ return _administraCargaConsultaService.CargaExpedienteDeConsultaGv(_archivoExpedientesConsultaGvCsv); });
            }

            _logger.LogInformation("Cargo la información de los expedientes previamente procesada");
            if (ultimosSaldos)
            {
                await Task.Run(() =>
                {
                    Parallel.ForEach(resultado, (expediente) =>
                    {
                        expediente.EsCreditoAReportar = false;
                        expediente.StatusCarteraVencida = false;
                        expediente.StatusCarteraVencida = false;
                        expediente.StatusImpago = false;
                        // return expediente;
                    });
                });

                resultado = await CruceConSaldosCuandoHayCambiosAsync(resultado);
            }
            _expedienteDeConsulta = resultado.ToList();
            if (cruzaConImagenes)
            {
                if (_imagenesCortas is not null)
                {
                    await CruzaInformacionImagenesAsync(_imagenesCortas);
                }
            }
            return _expedienteDeConsulta;
        }

        _logger.LogInformation("Carga la información para las consultas de imágenes (Información de Expedientes)");
        var ministracionesMesa = await _ministracionesMesaService.GetMinistracionesAsync();
        _logger.LogInformation("Se cargaron {numMinistraciones} ministraciones de la mesa", ministracionesMesa.Count());

        _logger.LogInformation("Se carga la información de los saldos activos");
        var saldosActivos = await _abSaldosActivosService.GetABSaldosActivosAsync();
        _logger.LogInformation("Se cargaron {numSaldosActivos} expedientes activos", saldosActivos.Count());

        _logger.LogInformation("Se carga la información de los saldos con Castigo");
        var saldosConCastigo = await _abSaldosConCastigoService.ObtieneABSaldosConCastigoProcesadosAsync();
        _logger.LogInformation("Se cargaron {numSaldosConCastigo} saldos clasificados con Castigos", saldosConCastigo.Count());

        IEnumerable<ExpedienteDeConsultaGv> expedientesCreditosActivos = await CruzaInformacionMesaAsync(ministracionesMesa, saldosActivos, saldosConCastigo);
        expedientesCreditosActivos = CruceConSaldosCuandoHayCambios(expedientesCreditosActivos);
        _expedienteDeConsulta = expedientesCreditosActivos.ToList();

        if (cruzaConImagenes)
        {
            if (_imagenesCortas is not null)
            {
                await CruzaInformacionImagenesAsync(_imagenesCortas);
            }
        }

        return _expedienteDeConsulta;
    }

    #region Helpers asyncronos de Carga Información
    private async Task<bool> CruzaInformacionImagenesAsync(IEnumerable<ArchivoImagenCorta> imagenes)
    {
        if (_saldosConBandera is not null)
        {
            var cruzaImagenes = await Task.Run(()=> { return (from scb in _saldosConBandera
                        join img in imagenes on QuitaCastigo(scb.NumCredito) equals QuitaCastigo(img.NumCredito)
                        select new
                        {
                            Saldos = scb,
                            Imagenes = img
                        }).ToList();});
            await Task.Run(() => {
                Parallel.ForEach(cruzaImagenes, (cruce) =>
                {
                    cruce.Saldos.TieneImagenDirecta = true;
                    cruce.Saldos.TieneImagenIndirecta = false;
                });
            });

            var cruzaImagenesSinImagenDirecta = await Task.Run(() => { return (from scb in _saldosConBandera.Where(x => !x.TieneImagenDirecta)
                        join img in imagenes on QuitaCastigo(scb.NumCredito)[..14] equals QuitaCastigo(img.NumCredito)[..14]
                        select new
                        {
                            Saldos = scb,
                            Imagenes = img
                        }).ToList(); });
            await Task.Run(() => {
                Parallel.ForEach(cruzaImagenesSinImagenDirecta, (cruce) =>
                {
                    if (!cruce.Saldos.TieneImagenDirecta)
                    {
                        // cruce.Saldos.TieneImagenDirecta = true;
                        cruce.Saldos.TieneImagenIndirecta = true;
                    }
                });
            });

            IEnumerable<ABSaldosFiltroCreditoDiario> saldosFiltroCreditosDiario = _saldosConBandera;
            if (_archivoABSaldosFiltroCreditoDiario is not null)
            {
                await Task.Run(() => { _ = _creaArchivoCsvService.CreaArchivoCsv(_archivoABSaldosFiltroCreditoDiario, saldosFiltroCreditosDiario); });
            }
        }

        if (_expedienteDeConsulta is not null)
        {
            var cruzaImagenes = await Task.Run(() => { return (from scb in _expedienteDeConsulta
                 join img in imagenes on QuitaCastigo(scb.NumCredito) equals QuitaCastigo(img.NumCredito)
                 select new
                 {
                     Saldos = scb,
                     Imagenes = img
                 }).ToList(); });

            await Task.Run(() =>
            {
                Parallel.ForEach(cruzaImagenes, (cruce) => {
                    cruce.Saldos.TieneImagenDirecta = true;
                    cruce.Saldos.TieneImagenIndirecta = false;
                    cruce.Saldos.TieneGuardaValor = cruce.Saldos.TieneGuardaValor || (cruce.Imagenes.CarpetaDestino ?? "").Contains(@"\11\GV\", StringComparison.InvariantCultureIgnoreCase);
                });
            });

            var cruzaImagenesSinImagenDirecta = await Task.Run(() => { return (from scb in _expedienteDeConsulta.Where(x => !x.TieneImagenDirecta)
                        join img in imagenes on QuitaCastigo(scb.NumCredito)[..14] equals QuitaCastigo(img.NumCredito)[..14]
                        select new
                        {
                            Saldos = scb,
                            Imagenes = img
                        }).ToList(); });

            await Task.Run(() => {
                Parallel.ForEach(cruzaImagenesSinImagenDirecta, (cruce) =>
                {
                    if (!cruce.Saldos.TieneImagenDirecta)
                    {
                        // cruce.Saldos.TieneImagenDirecta = true;
                        cruce.Saldos.TieneImagenIndirecta = true;
                    }
                    cruce.Saldos.TieneGuardaValor = cruce.Saldos.TieneGuardaValor || (cruce.Imagenes.CarpetaDestino ?? "").Contains(@"\11\GV\", StringComparison.InvariantCultureIgnoreCase);
                });
            });

            if (File.Exists(_archivoExpedientesConsultaGvCsv))
                File.Delete(_archivoExpedientesConsultaGvCsv);
            await Task.Run(() =>
            {
                _ = _creaArchivoCsvService.CreaArchivoCsv(_archivoExpedientesConsultaGvCsv, _expedienteDeConsulta);
            });
            _logger.LogInformation("Se guardo el archivo {archivoExpedienteConsulta} para una carga rápida en el futuro", _archivoExpedientesConsultaGvCsv);

        }
        return true;
    }

    private async Task LlenaInformacionABSaldosFiltroCreditoAync(IEnumerable<ABSaldosCompleta> abSaldosCompleta)
    {
        string[] arrayProductosFiltra = "K|L|M|N|P".Split('|');
        var filtaABSaldosCompletaPorProducto = await Task.Run(() =>
        {
            return abSaldosCompleta.Where(x => !arrayProductosFiltra.Any(y => y.Equals((x.NumProducto ?? " ")[..1], StringComparison.InvariantCultureIgnoreCase))).ToList();
        });
        filtaABSaldosCompletaPorProducto = await Task.Run(() => { return filtaABSaldosCompletaPorProducto.Where(x => !(x.NumProducto ?? "").Equals("D402", StringComparison.InvariantCultureIgnoreCase)).ToList(); });

        _saldosConBandera = await Task.Run(() =>
        {
            return filtaABSaldosCompletaPorProducto.Select(x =>
                    new ABSaldosFiltroCreditoDiario()
                    {
                        Regional = x.Regional,
                        NumCte = x.NumCte,
                        Sucursal = x.Sucursal,
                        ApellPaterno = x.ApellPaterno,
                        ApellMaterno = x.ApellMaterno,
                        Nombre1 = x.Nombre1,
                        Nombre2 = x.Nombre2,
                        RazonSocial = x.RazonSocial,
                        EstadoInegi = x.EstadoInegi,
                        NumCredito = x.NumCredito,
                        NumProducto = x.NumProducto,
                        TipoCartera = x.TipoCartera,
                        FechaApertura = x.FechaApertura,
                        FechaVencim = x.FechaVencim,
                        StatusContable = x.StatusContable,
                        SldoTotContval = x.SldoTotContval,
                        NumContrato = x.NumContrato,
                        FechaSuscripcion = x.FechaSuscripcion,
                        FechaVencCont = x.FechaVencCont,
                        TipoCreditoCont = x.TipoCreditoCont,
                        StatusImpago = (x.StatusContable ?? "").Equals("A", StringComparison.InvariantCultureIgnoreCase) && ((x.TipoCartera ?? "")[..1].Equals("B", StringComparison.InvariantCultureIgnoreCase)),
                        StatusCarteraVencida = (x.StatusContable ?? "").Equals("B", StringComparison.InvariantCultureIgnoreCase),
                        StatusCarteraVigente = (x.StatusContable ?? "").Equals("A", StringComparison.InvariantCultureIgnoreCase) && (!(x.TipoCartera ?? "")[..1].Equals("B", StringComparison.InvariantCultureIgnoreCase)),
                        TieneImagenDirecta = false,
                        TieneImagenIndirecta = false
                    }
                    ).ToList();
        });
    }
    private async Task<string> ObtieneUltimoABSaldosAsync()
    {
        if (!Directory.Exists(_carpetaUltimoABSaldos))
            return string.Empty;
        var csvFiles = await Task.Run(() => { return Directory.GetFiles(_carpetaUltimoABSaldos, "*.csv"); });            

        if (csvFiles.Length == 0)
        {
            return string.Empty;
        }

        var latestCsvFile = await Task.Run(() =>
        {
            return csvFiles.OrderByDescending(file => File.GetLastWriteTime(file)).First();
        });

        return latestCsvFile;
    }

    private async Task<IEnumerable<ExpedienteDeConsultaGv>> CruceConSaldosCuandoHayCambiosAsync(IEnumerable<ExpedienteDeConsultaGv> expedientesCreditosActivos)
    {
        expedientesCreditosActivos = await CruzaInformacionMesaAsync(expedientesCreditosActivos);

        _agencias = await _obtieneCorreosAgentes.ObtieneTodosLosCorreosYAgentesAsync();
        await CruzaInformacionAgenciasAsync(_agencias, expedientesCreditosActivos);

        await Task.Run(() => { _ = _creaArchivoCsvService.CreaArchivoCsv(_archivoExpedientesConsultaGvCsv, expedientesCreditosActivos); } );
        if (File.Exists(_archivoExpedientesConsultaCsv))
        { 
            File.Delete(_archivoExpedientesConsultaCsv);
        }
        _logger.LogInformation("Se guardo el archivo {archivoExpedienteConsulta} para una carga rápida en el futuro", _archivoExpedientesConsultaCsv);

        return expedientesCreditosActivos;
    }

    private static async Task<bool> CruzaInformacionAgenciasAsync(IEnumerable<CorreosAgencia> correos, IEnumerable<ExpedienteDeConsultaGv> expedientes)
    {
        var cruzaInformacion = await Task.Run(() => { return (from exp in expedientes
                                                              join correo in correos on exp.Agencia equals correo.NoAgencia
                                                              select new { Expedientes = exp, Agencias = correo }).ToList();
        });

        await Task.Run(() => {
            Parallel.ForEach(cruzaInformacion, (info) =>
            {
                info.Expedientes.CatAgencia = info.Agencias.Agencia;
                info.Expedientes.CatRegion = info.Agencias.Region;
            });
        });
        return true;
    }

    private async Task<IEnumerable<ExpedienteDeConsultaGv>> CruzaInformacionMesaAsync(IEnumerable<ExpedienteDeConsultaGv> origen)
    {
        if (_saldosConBandera is null)
        {
            return origen;
        }
        var leftJoinResult = await Task.Run(() => { 
            return (from scb in _saldosConBandera
                    join sa in origen on QuitaCastigo(scb.NumCredito) equals QuitaCastigo(sa.NumCredito) into sa2Group
                    from sa2Result in sa2Group.DefaultIfEmpty()
                    select new
                    {
                        SalBan = scb,
                        ExpCons = sa2Result
                    }
                              ).ToList();
        });

        IList<ExpedienteDeConsultaGv> lista = new List<ExpedienteDeConsultaGv>();
        var resultados = await Task.Run(() =>
        {
            return (from a in leftJoinResult
                    where a.ExpCons is not null
                    select new ExpedienteDeConsultaGv
                    {
                        NumCredito = QuitaCastigo(a.SalBan.NumCredito),
                        NumCreditoCancelado = (a.ExpCons is not null) ? a.ExpCons.NumCreditoCancelado : String.Empty,
                        EsSaldosActivo = (a.ExpCons is not null) && a.ExpCons.EsSaldosActivo,
                        EsCancelado = (a.ExpCons is not null) && a.ExpCons.EsCancelado,
                        EsOrigenDelDr = a.SalBan.FechaApertura >= _periodoDelDoctor,
                        EsCanceladoDelDr = (a.ExpCons is not null) && a.ExpCons.EsCanceladoDelDr,
                        EsCastigado = (a.ExpCons is not null) && a.ExpCons.EsCastigado,
                        TieneArqueo = (a.ExpCons is not null) && a.ExpCons.TieneArqueo,
                        Acreditado = ObtieneAcreditado(a.SalBan.ApellPaterno, a.SalBan.ApellMaterno, a.SalBan.Nombre1, a.SalBan.Nombre2, a.SalBan.RazonSocial),
                        FechaApertura = a.SalBan.FechaApertura,
                        FechaCancelacion = a.ExpCons?.FechaCancelacion,
                        Castigo = (a.ExpCons is not null) ? a.ExpCons.Castigo : string.Empty,
                        NumProducto = a.SalBan.NumProducto,
                        CatProducto = (a.ExpCons is not null) ? a.ExpCons.CatProducto : string.Empty,
                        TipoDeCredito = (a.ExpCons is not null) ? a.ExpCons.TipoDeCredito : string.Empty,
                        Ejecutivo = (a.ExpCons is not null) ? a.ExpCons.Ejecutivo : string.Empty,
                        Analista = (a.ExpCons is not null) ? a.ExpCons.Analista : string.Empty,
                        FechaInicioMinistracion = a.ExpCons?.FechaInicioMinistracion,
                        FechaSolicitud = a.ExpCons?.FechaSolicitud,
                        MontoCredito = (a.ExpCons is not null) ? a.ExpCons.MontoCredito : 0M,
                        InterCont = (a.ExpCons is not null) ? a.ExpCons.InterCont : 0M,
                        Region = a.SalBan.Regional,
                        Agencia = a.SalBan.Sucursal,
                        CatRegion = (a.ExpCons is not null) ? a.ExpCons.CatRegion : string.Empty,
                        CatAgencia = (a.ExpCons is not null) ? a.ExpCons.CatAgencia : string.Empty,
                        EsCreditoAReportar = true,
                        StatusImpago = a.SalBan.StatusImpago,
                        StatusCarteraVencida = a.SalBan.StatusCarteraVencida,
                        StatusCarteraVigente = a.SalBan.StatusCarteraVigente,
                        TieneImagenDirecta = a.SalBan.TieneImagenDirecta,
                        TieneImagenIndirecta = a.SalBan.TieneImagenIndirecta,
                        SldoTotContval = a.SalBan.SldoTotContval,
                        NumCliente = ObtieneNumCliente(a.SalBan, a.ExpCons)
                    }).ToList();
        });

        ((List<ExpedienteDeConsultaGv>)lista).AddRange(resultados);

        var leftJoinResultExpConsulta = await Task.Run(() =>
        {
            return (from sa in origen
                    join scb in lista on QuitaCastigo(sa.NumCredito) equals QuitaCastigo(scb.NumCredito) into sa2Group
                    from sa2Result in sa2Group.DefaultIfEmpty()
                    select new
                    {
                        ExpCons = sa,
                        SalBan = sa2Result
                    }
                      ).ToList();
        });
        var soloOrigen = await Task.Run(() =>
        {
            return leftJoinResultExpConsulta.Where(x => x.SalBan is null).Select(x => x.ExpCons).ToList();
        });
        ((List<ExpedienteDeConsultaGv>)lista).AddRange(soloOrigen);

        return lista.ToList();
    }


    private static async Task<IEnumerable<ExpedienteDeConsultaGv>> CruzaInformacionMesaAsync(IEnumerable<MinistracionesMesa> ministracionesMesa, IEnumerable<ABSaldosActivos> saldosActivos, IEnumerable<ABSaldosConCastigo> saldosConCastigo)
    {

        #region las ministraciones unicas
        var ministracionesUnicas = await Task.Run(() =>
        {
            return (from min in ministracionesMesa
                    group min by min.NumCredito into grupo
                    let laMayorMinistracion = grupo.OrderByDescending(x => x.NumMinistracion).FirstOrDefault()
                    select laMayorMinistracion).ToList();
        });
            
            
        var leftJoinResult = await Task.Run(() =>
        {
            return (from min in ministracionesUnicas
                    join sa in saldosActivos on min.NumCredito equals QuitaCastigo(sa.NumCredito) into sa2Group
                    from sa2Result in sa2Group.DefaultIfEmpty()
                    join t3 in saldosConCastigo on min.NumCredito equals QuitaCastigo(t3.NumCredito) into t3Group
                    from t3Result in t3Group.DefaultIfEmpty()
                    select new
                    {
                        Ministracion = min,
                        SaldosActivos = sa2Result,
                        SaldosConCastigo = t3Result
                    }
                              ).ToList();
        });
        #endregion

        #region Cruzo la información de los Saldos Activos vs la Mesa
        IList<ExpedienteDeConsultaGv> lista = new List<ExpedienteDeConsultaGv>();
        var resultados = await Task.Run(() =>
        {
            return (from a in leftJoinResult
                    select new ExpedienteDeConsultaGv
                    {
                        NumCredito = a.Ministracion.NumCredito,
                        NumCreditoCancelado = (a.SaldosConCastigo is not null) ? a.SaldosConCastigo.NumCredito : (a.SaldosActivos is not null) ? a.SaldosActivos.NumCreditoCancelado : string.Empty,
                        EsSaldosActivo = a.SaldosActivos is not null,
                        EsCancelado = a.SaldosConCastigo is not null,
                        EsOrigenDelDr = a.Ministracion.EsOrigenDelDoctor,
                        EsCanceladoDelDr = a.SaldosConCastigo is not null && a.SaldosConCastigo.EsCancelacionDelDoctor,
                        EsCastigado = (a.SaldosConCastigo is not null) && (a.SaldosConCastigo.Castigo ?? "").Equals("Castigado", StringComparison.OrdinalIgnoreCase),
                        TieneArqueo = (a.SaldosActivos is not null) ? a.SaldosActivos.CuentaConGuardaValores : (a.SaldosConCastigo is not null) && a.SaldosConCastigo.CuentaConGuardaValores,
                        Acreditado = a.Ministracion.Acreditado,
                        FechaApertura = (a.SaldosActivos is not null) ? a.SaldosActivos.FechaApertura : a.SaldosConCastigo?.FechaApertura,
                        FechaCancelacion = (a.SaldosConCastigo is not null) ? a.SaldosConCastigo.FechaCancelacion : a.SaldosActivos?.FechaCancelacion,
                        Castigo = (a.SaldosConCastigo is not null) ? a.SaldosConCastigo.Castigo : a.SaldosActivos?.Castigo,
                        NumProducto = (a.SaldosActivos is not null) ? a.SaldosActivos.NumProducto : (a.SaldosConCastigo is not null) ? a.SaldosConCastigo.NumProducto : "",
                        CatProducto = (a.SaldosConCastigo is not null) ? a.SaldosConCastigo.CatProducto : (a.SaldosActivos is not null) ? a.SaldosActivos.CatProducto : "",
                        TipoDeCredito = a.Ministracion.Descripcion,
                        Ejecutivo = (a.SaldosActivos is not null) ? a.SaldosActivos.NombreEjecutivo : (a.SaldosConCastigo is not null) ? a.SaldosConCastigo.NombreEjecutivo : "",
                        Analista = a.Ministracion.Analista,
                        FechaInicioMinistracion = a.Ministracion.FechaAsignacion,
                        FechaSolicitud = a.Ministracion.FechaAsignacion,
                        MontoCredito = (a.SaldosConCastigo is not null) ? a.SaldosConCastigo.MontoCredito ?? 0 :
                         (a.SaldosActivos is not null) ? a.SaldosActivos.MontoMinistraCap : 0,
                        InterCont = (a.SaldosConCastigo is not null) ? a.SaldosConCastigo.InteresCont ?? 0 :
                         (a.SaldosActivos is not null) ? a.SaldosActivos.SldoIntCont : 0,
                        Region = a.Ministracion.Regional,
                        Agencia = a.Ministracion.Sucursal,
                        CatRegion = (a.SaldosConCastigo is not null && !string.IsNullOrEmpty(a.SaldosConCastigo.CatRegion)) ? a.SaldosConCastigo.CatRegion ?? string.Empty :
                         (a.SaldosActivos is not null && !string.IsNullOrEmpty(a.SaldosActivos.CatRegion)) ? a.SaldosActivos.CatRegion : string.Empty,
                        CatAgencia = a.Ministracion.CatAgencia,
                        NumCliente = ObtieneNumCliente(a.SaldosConCastigo, a.SaldosActivos)

                    }).ToList();
        });            

        ((List<ExpedienteDeConsultaGv>)lista).AddRange(resultados);
        #endregion

        #region Con castigo sin ministracion
        var leftJoinResult2 = await Task.Run(() => { 
            return (from sc in saldosConCastigo
                    join min in ministracionesUnicas on QuitaCastigo(sc.NumCredito) equals min.NumCredito into t2Group
                    from t2Result in t2Group.DefaultIfEmpty()
                    join sa in saldosActivos on QuitaCastigo(sc.NumCredito) equals QuitaCastigo(sa.NumCredito) into t3Group
                    from t3Result in t3Group.DefaultIfEmpty()
                    select new
                    {
                        SaldosConCastigo = sc,
                        Ministracion = t2Result,
                        SaldosActivos = t3Result
                    }
                              ).ToList();
        });

        // Elimino las que tienen ministración, esas ya las atendí
        leftJoinResult2 = await Task.Run(() => { return leftJoinResult2.Where(x => x.Ministracion is null).ToList(); });

        var resultadosNuevos = await Task.Run(() =>
        {
            return (from a in leftJoinResult2
                    select new ExpedienteDeConsultaGv
                    {
                        NumCredito = QuitaCastigo(a.SaldosConCastigo.NumCredito),
                        NumCreditoCancelado = (a.SaldosConCastigo is not null) ? a.SaldosConCastigo.NumCredito : (a.SaldosActivos is not null) ? a.SaldosActivos.NumCreditoCancelado : String.Empty,
                        EsSaldosActivo = a.SaldosActivos is not null,
                        EsCancelado = a.SaldosConCastigo is not null,
                        EsOrigenDelDr = (a.SaldosConCastigo is not null) ? a.SaldosConCastigo.EsOrigenDelDoctor : (a.SaldosActivos is not null) && a.SaldosActivos.EsOrigenDelDoctor,
                        EsCanceladoDelDr = (a.SaldosConCastigo is not null) ? a.SaldosConCastigo.EsCancelacionDelDoctor : (a.SaldosActivos is not null) && a.SaldosActivos.EsCancelacionDelDoctor,
                        EsCastigado = (a.SaldosConCastigo is not null) && (a.SaldosConCastigo.Castigo ?? "").Equals("Castigado", StringComparison.OrdinalIgnoreCase),
                        TieneArqueo = (a.SaldosActivos is not null) ? a.SaldosActivos.CuentaConGuardaValores : (a.SaldosConCastigo is not null) && a.SaldosConCastigo.CuentaConGuardaValores,
                        Acreditado = (a.SaldosConCastigo is not null) ? a.SaldosConCastigo.Acreditado : (a.SaldosActivos is not null) ? a.SaldosActivos.Acreditado : String.Empty,
                        FechaApertura = (a.SaldosActivos is not null) ? a.SaldosActivos.FechaApertura : a.SaldosConCastigo?.FechaApertura,
                        FechaCancelacion = (a.SaldosConCastigo is not null) ? a.SaldosConCastigo.FechaCancelacion : a.SaldosActivos?.FechaCancelacion,
                        Castigo = (a.SaldosConCastigo is not null) ? a.SaldosConCastigo.Castigo : a.SaldosActivos?.Castigo,
                        NumProducto = (a.SaldosActivos is not null) ? a.SaldosActivos.NumProducto : (a.SaldosConCastigo is not null) ? a.SaldosConCastigo.NumProducto : String.Empty,
                        CatProducto = (a.SaldosConCastigo is not null) ? a.SaldosConCastigo.CatProducto : (a.SaldosActivos is not null) ? a.SaldosActivos.CatProducto : "",
                        TipoDeCredito = (a.SaldosConCastigo is not null) ? a.SaldosConCastigo.CatProducto : (a.SaldosActivos is not null) ? a.SaldosActivos.CatProducto : String.Empty,
                        Ejecutivo = (a.SaldosActivos is not null) ? a.SaldosActivos.NombreEjecutivo : (a.SaldosConCastigo is not null) ? a.SaldosConCastigo.NombreEjecutivo : String.Empty,
                        Analista = String.Empty,
                        FechaInicioMinistracion = a.SaldosActivos?.FechaInicioMinistracion,
                        FechaSolicitud = null,
                        MontoCredito = (a.SaldosConCastigo is not null) ? a.SaldosConCastigo.MontoCredito ?? 0 :
                           (a.SaldosActivos is not null) ? a.SaldosActivos.MontoMinistraCap : 0,
                        InterCont = (a.SaldosConCastigo is not null) ? a.SaldosConCastigo.InteresCont ?? 0 :
                           (a.SaldosActivos is not null) ? a.SaldosActivos.SldoIntCont : 0,
                        Region = (a.SaldosActivos is not null) ? a.SaldosActivos.Regional : 0,
                        Agencia = (a.SaldosConCastigo is not null) ? Convert.ToInt32(a.SaldosConCastigo.Sucursal) : (a.SaldosActivos is not null) ? a.SaldosActivos.Regional : 0,
                        CatRegion = (a.SaldosConCastigo is not null && !string.IsNullOrEmpty(a.SaldosConCastigo.CatRegion)) ? a.SaldosConCastigo.CatRegion ?? string.Empty :
                           (a.SaldosActivos is not null && !string.IsNullOrEmpty(a.SaldosActivos.CatRegion)) ? a.SaldosActivos.CatRegion : string.Empty,
                        CatAgencia = (a.SaldosConCastigo is not null) ? a.SaldosConCastigo.CatSucursal : (a.SaldosActivos is not null) ? a.SaldosActivos.CatSucursal : (a.Ministracion is not null) ? a.Ministracion.CatAgencia : string.Empty,
                        NumCliente = ObtieneNumCliente(a.SaldosConCastigo, a.SaldosActivos)
                    }).ToList();
        });
        ((List<ExpedienteDeConsultaGv>)lista).AddRange(resultadosNuevos);
        #endregion

        #region Agrego regiones y Agencias
        await Task.Run(() =>
        {
            Parallel.ForEach(lista, (item) => {
                if (item.Region == 0)
                    item.Region = Convert.ToInt32((item.NumCredito ?? "0")[..1]) * 100;
                if (item.Agencia == 0)
                    item.Agencia = Convert.ToInt32((item.NumCredito ?? "000")[..3]);
            });
        });
        #endregion
        return lista.OrderBy(x => x.NumCredito).ToList();
    }
    #endregion
    #endregion

    #region Carga los Creditos Cancelados y sus helpers
    public async Task<IEnumerable<CreditosCanceladosAplicacion>> CargaCreditosCanceladosAsync(bool cruzaConImagenes = false)
    {
        IEnumerable<CreditosCanceladosAplicacion> resultado;
        #region Cargo los créditos cancelados desde CSV
        // TODO: Eliminar el CSV para que se pueda cruzar con las imagenes o poner la carga de imagenes despues de la carga de los creditos cancelados
        FileInfo fi = new(_archivoDeExpedientesCanceladosCsv);
        if (fi.Exists)
        {
            resultado = await Task.Run(() => { return _administraCreditosCanceladosService.CargaExpedienteCancelados(_archivoDeExpedientesCanceladosCsv); });
            _creditosCanceladosAplicacion = resultado;
            if (cruzaConImagenes)
            {
                await CruzaConImagenesExpedientesCancelados(resultado);
            }
            return _creditosCanceladosAplicacion;
        }
        #endregion

        var creditosCancelados = await _creditosCanceladosService.GetCreditosCanceladosAsync(_cargaCreditosCancelados);
        resultado = await Task.Run(() =>
        {
            return creditosCancelados.Where(x => x.PrimeraDispersion >= new DateTime(2020, 07, 1))
            .Select(x => new CreditosCanceladosAplicacion()
            {
                NumCreditoCancelado = x.NumCreditoActual,
                NumCreditoOrigen = x.NumCreditoOrigen,
                NumRegion = Convert.ToInt32((x.NumCreditoOrigen ?? "0")[..1]) * 100,
                CatRegion = x.CoordinacionRegional,
                Agencia = Convert.ToInt32((x.NumCreditoOrigen ?? "000")[..3]),
                CatAgencia = x.Agencia,
                NumCliente = x.NumCliente,
                Acreditado = x.Acreditado,
                TipoPersona = x.TipoPersona,
                AnioOriginacion = x.AnioOrignacion,
                PisoCredito = x.Piso,
                Portafolio = x.Portafolio,
                Concepto = x.Concepto,
                FechaCancelacion = x.FechaCancelacion,
                FechaPrimeraDispersion = x.PrimeraDispersion,
                TieneImagenDirecta = false,
                TieneImagenIndirecta = false
            }).ToList();

        });

        _creditosCanceladosAplicacion = resultado;
        // Aqui se cruza con imágenes
        await CruzaConImagenesExpedientesCancelados(resultado);
        return resultado;
    }

    private async Task CruzaConImagenesExpedientesCancelados(IEnumerable<CreditosCanceladosAplicacion> resultado)
    {
        if (_imagenesCortas is not null)
        {
            await CruzaInformacionImagenesConCreditosCanceladosAsync(_imagenesCortas);
        }

        await Task.Run(() => { _ = _creaArchivoCsvService.CreaArchivoCsv(_archivoDeExpedientesCanceladosCsv, resultado); });
    }
    #region Helpers de creditos cancelados
    private async Task<bool> CruzaInformacionImagenesConCreditosCanceladosAsync(IEnumerable<ArchivoImagenCorta> imagenes)
    {
        if (_creditosCanceladosAplicacion is not null)
        {
            var cruzaImagenes = await Task.Run(() => { return (from scb in _creditosCanceladosAplicacion
                                                               join img in imagenes on QuitaCastigo(scb.NumCreditoCancelado) equals QuitaCastigo(img.NumCredito)
                                                               select new
                                                               {
                                                                   Saldos = scb,
                                                                   Imagenes = img
                                                               }).ToList();
            });

            await Task.Run(() => {
                Parallel.ForEach(cruzaImagenes,(cruce) => 
                {
                    cruce.Saldos.TieneImagenDirecta = true;
                    cruce.Saldos.TieneImagenIndirecta = false;
                });
            });

            var cruzaImagenesSinImagenDirecta = await Task.Run(() =>
            {
                return (from scb in _creditosCanceladosAplicacion.Where(x => !x.TieneImagenDirecta)
                        join img in imagenes on QuitaCastigo(scb.NumCreditoCancelado)[..14] equals QuitaCastigo(img.NumCredito)[..14]
                        select new
                        {
                            Saldos = scb,
                            Imagenes = img
                        }).ToList();
            });

            await Task.Run(() =>
            {
                Parallel.ForEach(cruzaImagenesSinImagenDirecta, (cruce) => {
                    if (!cruce.Saldos.TieneImagenDirecta)
                    {
                        // cruce.Saldos.TieneImagenDirecta = true;
                        cruce.Saldos.TieneImagenIndirecta = true;
                    }
                });
            });
        }
        return true;
    }
    #endregion
    #endregion

    #region Carga la Colocación con Pagos (Liquidados) y sus helpers
    public async Task<IEnumerable<ColocacionConPagos>> CargaColocacionConPagosAsync(bool cruzaConImagenes = false)
    {
        IEnumerable<ColocacionConPagos> resultado;
        FileInfo fi = new(_archivoColocacionConPagosCsv);
        if (fi.Exists)
        {
            resultado = await Task.Run(() => { return _administraCargaLiquidaciones.CargaLiquidacionesCompleta(_archivoColocacionConPagosCsv); });
            _creditosLiquidados = resultado;
            if (cruzaConImagenes)
            {
                await CruzaImagenesColocacionConPagosAsync(resultado);
            }
            return _creditosLiquidados;
        }

        var creditosLiquidados = await _liquidacionesService.GetColocacionConPagosAsync(_archivoColocacionConPagos);
        resultado = creditosLiquidados.ToList();
        _creditosLiquidados = resultado;
        // Aqui se cruza con imágenes
        await CruzaImagenesColocacionConPagosAsync(resultado);
        return resultado;
    }

    private async Task CruzaImagenesColocacionConPagosAsync(IEnumerable<ColocacionConPagos> resultado)
    {
        if (_imagenesCortas is not null)
        {
            await CruzaInformacionImagenesConCreditosLiquidadosAsync(_imagenesCortas);
        }

        await Task.Run(() =>
        {
            _ = _creaArchivoCsvService.CreaArchivoCsv(_archivoColocacionConPagosCsv, resultado);
        });
    }

    private async Task<bool> CruzaInformacionImagenesConCreditosLiquidadosAsync(IEnumerable<ArchivoImagenCorta> imagenesCortas)
    {
        if (_creditosLiquidados is not null)
        {
            var cruzaImagenes = await Task.Run(() => { 
                return (from scb in _creditosLiquidados
                        join img in imagenesCortas on QuitaCastigo(scb.NumCredito) equals QuitaCastigo(img.NumCredito)
                        select new
                        {
                            Saldos = scb,
                            Imagenes = img
                        }).ToList();
            });

            await Task.Run(() =>
            {
                Parallel.ForEach(cruzaImagenes, (cruce) =>
                {
                    cruce.Saldos.TieneImagenDirecta = true;
                    cruce.Saldos.TieneImagenIndirecta = false;
                });
            }
            );

            var cruzaImagenesSinImagenDirecta = await Task.Run(() =>
            {
                return (from scb in _creditosLiquidados.Where(x => !x.TieneImagenDirecta)
                        join img in imagenesCortas on QuitaCastigo(scb.NumCredito)[..14] equals QuitaCastigo(img.NumCredito)[..14]
                        select new
                        {
                            Saldos = scb,
                            Imagenes = img
                        }).ToList();
            });

            await Task.Run(() =>
            {
                Parallel.ForEach(cruzaImagenesSinImagenDirecta, (cruce) =>
                {
                    if (!cruce.Saldos.TieneImagenDirecta)
                    {
                        // cruce.Saldos.TieneImagenDirecta = true;
                        cruce.Saldos.TieneImagenIndirecta = true;
                    }
                });
            });

        }
        return true;
    }
    #endregion

    #region Carga los expedientes jurídicos y sus helpers
    public async Task<IEnumerable<ExpedienteJuridico>> CargaExpedientesJuridicosAsync(bool cruzaConImagenes = false)

    {
        IEnumerable<ExpedienteJuridico> resultado;
        FileInfo fi = new(_archivoExpedienteJuridicoCsv);
        if (fi.Exists)
        {
            resultado = await Task.Run(() => { return _administraExpedientesJuridicosService.CargaExpedientesJuridicos(_archivoExpedienteJuridicoCsv); });
            _expedientesJuridicos = resultado;
            if (cruzaConImagenes)
            {
                await CruzaConImagenesExpedientesJuridicos(resultado);
            }

            await Task.Run(() =>
            {
                Parallel.ForEach(resultado, (elemento) =>
                {
                    elemento.NumCreditos = (elemento.NumCreditos ?? "").Replace("¬", "\n");
                    elemento.JuzgadoUbicacion = (elemento.JuzgadoUbicacion ?? "").Replace("¬", "\n");
                    elemento.Observaciones = (elemento.Observaciones ?? "").Replace("¬", "\n");
                    elemento.Descripcion = (elemento.Descripcion ?? "").Replace("¬", "\n");
                    elemento.DescripcionActuacion = (elemento.DescripcionActuacion ?? "").Replace("¬", "\n");
                });
            });
            return _expedientesJuridicos;
        }

        var expedientesJuridicos = await _juridicoService.GetExpedientesJuridicosAsync(_archivoExpedienteJuridico);
        resultado = await Task.Run(() =>
        {
            return expedientesJuridicos.Select(x => new ExpedienteJuridico()
            {
                Region = ObtieneRegionJuridico(x.CatRegion ?? ""),
                CatRegion = (x.CatRegion ?? "").Replace("\n", "").Replace("|", ""),
                Agencia = ObtieneAgenciaJuridico(ObtieneRegionJuridico(x.CatRegion ?? ""), x.Estado ?? ""),
                CatAgencia = (x.Estado ?? "").Replace("\n", "").Replace("|", ""),
                Estado = (x.CatAgencia ?? "").Replace("\n", "").Replace("|", ""),
                NombreDemandado = (x.NombreDemandado ?? "").Replace("\n", "").Replace("|", ""),
                TipoCredito = (x.TipoCredito ?? "").Replace("\n", "").Replace("|", ""),
                NumContrato = (x.NumContrato ?? "").Replace("\n", "").Replace("|", ""),
                NumCte = (x.NumCte ?? "").Replace("\n", "").Replace("|", ""),
                NumCredito = (x.NumCredito ?? "").Replace("\n", "").Replace("|", ""),
                NumCreditos = (x.NumCreditos ?? "").Trim().Replace("\n", "¬").Replace(";", "¬").Replace(" ", "¬").Replace("|", "").Replace("¬¬", "¬").Replace("¬¬", "¬"),
                FechaTranspasoJuridico = x.FechaTranspasoJuridico,
                FechaTranspasoExterno = x.FechaTranspasoExterno,
                FechaDemanda = x.FechaDemanda,
                Juicio = (x.Juicio ?? "").Replace("\n", "").Replace("|", ""),
                CapitalDemandado = x.CapitalDemandado,
                Dlls = x.Dlls,
                JuzgadoUbicacion = (x.JuzgadoUbicacion ?? "").Replace("\n", "¬").Replace("|", ""),
                Expediente = (x.Expediente ?? "").Replace("\n", "").Replace("|", ""),
                ClaveProcesal = (x.ClaveProcesal ?? "").Replace("\n", "").Replace("|", ""),
                EtapaProcesal = (x.EtapaProcesal ?? "").Replace("\n", "").Replace("|", ""),
                Rppc = (x.Rppc ?? "").Replace("\n", "").Replace("|", ""),
                Ran = (x.Ran ?? "").Replace("\n", "").Replace("|", ""),
                RedCredAgricola = (x.RedCredAgricola ?? "").Replace("\n", "").Replace("|", ""),
                FechaRegistro = x.FechaRegistro,
                Tipo = (x.Tipo ?? "").Replace("\n", "").Replace("|", ""),
                TipoGarantias = (x.TipoGarantias ?? "").Replace("\n", "").Replace("|", ""),
                Descripcion = (x.Descripcion ?? "").Replace("\n", "¬").Replace("|", ""),
                ValorRegistro = x.ValorRegistro,
                FechaAvaluo = x.FechaAvaluo,
                GradoPrelacion = (x.GradoPrelacion ?? "").Replace("\n", "").Replace("|", ""),
                CAval = (x.CAval ?? "").Replace("\n", "").Replace("|", ""),
                SAval = (x.SAval ?? "").Replace("\n", "").Replace("|", ""),
                Inscripcion = (x.Inscripcion ?? "").Replace("\n", "").Replace("|", ""),
                Clave = (x.Clave ?? "").Replace("\n", "").Replace("|", ""),
                NumFolio = (x.NumFolio ?? "").Replace("\n", "").Replace("|", ""),
                AbogadoResponsable = (x.AbogadoResponsable ?? "").Replace("\n", "").Replace("|", ""),
                Observaciones = (x.Observaciones ?? "").Replace("\n", "¬").Replace("|", ""),
                FechaUltimaActuacion = x.FechaUltimaActuacion,
                DescripcionActuacion = (x.DescripcionActuacion ?? "").Replace("\n", "¬").Replace("|", ""),
                Piso = (x.Piso ?? "").Replace("\n", "").Replace("|", ""),
                Fonaga = (x.Fonaga ?? "").Replace("\n", "").Replace("|", ""),
                PequenioProductor = (x.PequenioProductor ?? "").Replace("\n", "").Replace("|", ""),
                FondoMutual = (x.FondoMutual ?? "").Replace("\n", "").Replace("|", ""),
                ExpectativasRecuperacion = (x.ExpectativasRecuperacion ?? "").Replace("\n", "").Replace("|", ""),
                TieneImagenDirecta = x.TieneImagenDirecta,
                TieneImagenIndirecta = x.TieneImagenIndirecta,
                TieneImagenExpediente = x.TieneImagenExpediente
            }).ToList();
        });

        _expedientesJuridicos = resultado;
        await CruzaConImagenesExpedientesJuridicos(resultado);

        await Task.Run(() =>
        Parallel.ForEach(resultado, (elemento) =>
        {
            elemento.NumCreditos = (elemento.NumCreditos ?? "").Replace("¬", "\n");
            elemento.JuzgadoUbicacion = (elemento.JuzgadoUbicacion ?? "").Replace("¬", "\n");
            elemento.Observaciones = (elemento.Observaciones ?? "").Replace("¬", "\n");
            elemento.Descripcion = (elemento.Descripcion ?? "").Replace("¬", "\n");
            elemento.DescripcionActuacion = (elemento.DescripcionActuacion ?? "").Replace("¬", "\n");

        }));
        return resultado;
    }

    private async Task CruzaConImagenesExpedientesJuridicos(IEnumerable<ExpedienteJuridico> resultado)
    {
        #region Aqui se cruza con imágenes
        if (_imagenesCortasExpedientesJuridico is not null)
        {
            await CruzaInformacionImagenesExpedientesJuridicosAsync();  // _imagenesCortasExpedientesJuridico
        }
        if (_imagenesCortas is not null)
        {
            await CruzaInformacionImagenesConExpedientesJuridicosExpedientesAsync(_imagenesCortas);
        }
        #endregion
        await Task.Run(() =>
        Parallel.ForEach(resultado, (elemento) =>
        {
            elemento.NumCreditos = (elemento.NumCreditos ?? "").Replace("\n", "¬");
            elemento.JuzgadoUbicacion = (elemento.JuzgadoUbicacion ?? "").Replace("\n", "¬");
            elemento.Observaciones = (elemento.Observaciones ?? "").Replace("\n", "¬");
            elemento.Descripcion = (elemento.Descripcion ?? "").Replace("\n", "¬");
            elemento.DescripcionActuacion = (elemento.DescripcionActuacion ?? "").Replace("\n", "¬");
        }));

        await Task.Run(() => _creaArchivoCsvService.CreaArchivoCsv(_archivoExpedienteJuridicoCsv, resultado));
    }

    public async Task<IEnumerable<ArchivoImagenExpedientesCorta>> CargaInformacionImagenesExpedientesJuridicosAsync()
    {
        FileInfo fi = new(_archivosImagenesExpedientesJuridicoCorta);
        if (fi.Exists)
        {
            _imagenesCortasExpedientesJuridico = await Task.Run(() => { return _administraCargaConsultaService.CargaArchivoImagenExpedientesCortas(_archivosImagenesExpedientesJuridicoCorta); });
                
            _logger.LogInformation("Cargo la información de los expedientes previamente procesada");

            CambiaUnidadExpedientesJuridico();

            return _imagenesCortasExpedientesJuridico;
        }
        // IList<ArchivoImagenCorta> lista = new List<ArchivoImagenCorta>();
        IEnumerable<ArchivosImagenes> resultado = await _servicioImagenes.CargaImagenesTratadasAsync(_archivosImagenesExpedientesJuridico);
        _imagenesCortasExpedientesJuridico = await GuardaImagenesCortasJuridico(resultado);
        return _imagenesCortasExpedientesJuridico;
    }

    public async Task<IEnumerable<ArchivoImagenExpedientesCorta>> GuardaImagenesCortasJuridico(IEnumerable<ArchivosImagenes> imagenes)
    {
        imagenes = imagenes.Where(x => (x.Extension ?? "").Equals(".pdf", comparisonType: StringComparison.InvariantCultureIgnoreCase));
        var resultadoCorto = await Task.Run(() =>
        {
            return imagenes.Select(x => new ArchivoImagenExpedientesCorta()
            {
                Id = x.Id,
                NumContrato = x.NumContrato,
                NumCredito = x.NumCredito,
                NombreArchivo = x.NombreArchivo,
                CarpetaDestino = x.CarpetaDestino,
                NumPaginas = x.NumPaginas,
                Hash = x.Hash,
                EsTurno = (x.UsuarioDeModificacion ?? "").Equals("Si", StringComparison.InvariantCultureIgnoreCase),
                EsCobranza = (x.ClasifMesa ?? "").Equals("Si", StringComparison.InvariantCultureIgnoreCase)
            }).ToList();
        });

        await Task.Run(() => { _ = _creaArchivoCsvService.CreaArchivoCsv(_archivosImagenesExpedientesJuridicoCorta, resultadoCorto); });
        return resultadoCorto;
    }

    #region Helpers de Expedientes juridicos
    private async Task CruzaInformacionImagenesExpedientesJuridicosAsync()
    {
        if (_expedientesJuridicos is null || !_expedientesJuridicos.Any())
        {
            return;
        }
        if (_imagenesCortasExpedientesJuridico is null || !_imagenesCortasExpedientesJuridico.Any())
        {
            return;
        }
        // _imagenesCortasExpedientesJuridico
        var expedientesDirectos = await Task.Run(() => { return (from exp in _expedientesJuridicos
                                                                 join img in _imagenesCortasExpedientesJuridico on new { exp.NumContrato, exp.NumCredito } equals new { NumContrato = (img.NumContrato ?? "")[..15], img.NumCredito } into gj
                                                                 from subimg in gj.DefaultIfEmpty()
                                                                 select new { exp, subimg }).ToList();
        });

        await Task.Run(() => {
            Parallel.ForEach(expedientesDirectos, (item) => {
                if (item.subimg is not null)
                {
                    item.exp.TieneImagenDirecta = item.subimg.EsTurno;
                    item.exp.TieneImagenIndirecta = item.subimg.EsCobranza;
                }
            });
        });
            


        var expedientesInDirectos = await Task.Run(() => {
            return (from exp in _expedientesJuridicos.Where(x=>!x.TieneImagenDirecta && !x.TieneImagenIndirecta)
                                  join img in _imagenesCortasExpedientesJuridico on new { exp.NumContrato } equals new { NumContrato = (img.NumContrato ?? "")[..15] } into gj
                                  from subimg in gj.DefaultIfEmpty()
                                  select new { exp, subimg }).ToList();
        });

        await Task.Run(() =>
        {
            Parallel.ForEach(expedientesInDirectos, (item) =>
            {
                if (item.subimg is not null)
                {
                    item.exp.TieneImagenDirecta = item.subimg.EsTurno;
                    item.exp.TieneImagenIndirecta = item.subimg.EsCobranza;
                }
            });
        });


        var expedientesInDirectosCredito = await Task.Run(() => {
            return (from exp in _expedientesJuridicos.Where(x => !x.TieneImagenDirecta && !x.TieneImagenIndirecta)
                    join img in _imagenesCortasExpedientesJuridico on new { exp.NumCredito } equals new { img.NumCredito } into gj
                    from subimg in gj.DefaultIfEmpty()
                    select new { exp, subimg }).ToList();
        });

        await Task.Run(() => {
            Parallel.ForEach(expedientesInDirectosCredito, (item) => {
                if (item.subimg is not null)
                {
                    item.exp.TieneImagenDirecta = item.subimg.EsTurno;
                    item.exp.TieneImagenIndirecta = item.subimg.EsCobranza;
                }
            });
        
        });
    }

    private async Task<bool> CruzaInformacionImagenesConExpedientesJuridicosExpedientesAsync(IEnumerable<ArchivoImagenCorta> imagenesCortas)
    {
        if (_expedientesJuridicos is not null)
        {
            var cruzaImagenes =
                await Task.Run(() => {
                    return (from scb in _expedientesJuridicos
                            join img in imagenesCortas on QuitaCastigo(scb.NumCredito) equals QuitaCastigo(img.NumCredito)
                            select new
                            {
                                Saldos = scb,
                                Imagenes = img
                            }).ToList();
                });
                
            foreach (var cruce in cruzaImagenes)
            {
                cruce.Saldos.TieneImagenExpediente = true;
            }
            //

            cruzaImagenes = (from scb in _expedientesJuridicos
                             join img in imagenesCortas on QuitaCastigoContrato(scb.NumCredito) equals QuitaCastigoContrato(img.NumCredito)
                             select new
                             {
                                 Saldos = scb,
                                 Imagenes = img
                             }).ToList();
            foreach (var cruce in cruzaImagenes)
            {
                cruce.Saldos.TieneImagenExpediente = true;
            }

        }
        return true;
    }
    #endregion
    #endregion

    #region Guarda Valores
    public async Task<bool> DescargaGuardaValoresAgencia(IEnumerable<GuardaValores> guardaValores, int agencia, string carpetaDestino, IProgress<ReporteProgresoDescompresionArchivos> avance)
    {
        var guardaValoresAgencia = guardaValores.Where(x=>x.Sucursal == agencia).ToList();
        return await _descargaGuardaValoresServices.DescargaGuardaValoresAgencia(guardaValoresAgencia, carpetaDestino, avance);    
    }
    #endregion
}