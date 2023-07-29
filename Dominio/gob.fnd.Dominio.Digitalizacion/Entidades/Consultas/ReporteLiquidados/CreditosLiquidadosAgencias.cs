using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gob.fnd.Dominio.Digitalizacion.Entidades.Consultas.ReporteLiquidados
{
    public class CreditosLiquidadosAgencias
    {
        public int Region { get; set; }
        public int Agencia { get; set; }
        public string? CatAgencia { get; set; }
        public int TotalClientes { get; set; }
        public int CantidadAnterioresJul2020 { get; set; }
        public int CantidadSegundoSemestre2020 { get; set; }
        public int Cantidad2021 { get; set; }
        public int Cantidad2022 { get; set; }
        public int Cantidad2023 { get; set; }
        public int TotalCreditos { get; set; }
        public int CantidadReestructura { get; set; }
        public int CantidadCancelaciones { get; set; }
        public int CantidadLiquidados { get; set; }
        public decimal PagoCapitalAntesPrimerSemestre2020 { get; set; }
        public decimal PagoInteresAntesPrimerSemestre2020 { get; set; }
        public decimal PagoMoratoriosAntesPrimerSemestre2020 { get; set; }
        public decimal PagoTotalAntesPrimerSemestre2020 { get; set; }
        public decimal PagoCapitalDespuesSegundoSemestre2020 { get; set; }
        public decimal PagoInteresDespuesSegundoSemestre2020 { get; set; }
        public decimal PagoMoratoriosDespuesSegundoSemestre2020 { get; set; }
        public decimal PagoTotalDespuesSegundoSemestre2020 { get; set; }
    }
}
