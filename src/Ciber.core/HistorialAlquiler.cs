
namespace Ciber.core;

public class HistorialdeAlquiler
{
    public int IdHistorial { get; set; }
    public int Ncuenta { get; set; }
    public int Nmaquina { get; set; }
    public DateTime FechaInicio { get; set; }
    public DateTime FechaFin { get; set; }
    public decimal TotalPagar { get; set; }
    public decimal MontoPagado { get; set; }
    public decimal PrecioPorHora { get; set; }
    public TimeSpan DuracionTotal => FechaFin - FechaInicio;
    public decimal HorasUtilizadas => (decimal)DuracionTotal.TotalHours;
    public decimal MontoPendiente => TotalPagar - MontoPagado;
    public bool EstaCompletamentePagado => MontoPendiente <= 0;
}

