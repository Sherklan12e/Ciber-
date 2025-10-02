namespace Ciber.core;

public class Alquiler
{
    public int IdAlquiler { get; set; }
    public int Ncuenta { get; set; }
    public int Nmaquina { get; set; }
    public int Tipo { get; set; }
    public TimeSpan? CantidadTiempo { get; set; } // Si es nullable
    public bool? Pagado { get; set; } // Nullable
    public DateTime FechaInicio { get; set; } = DateTime.Now;
    public DateTime? FechaFin { get; set; }
    public decimal PrecioPorHora { get; set; }
    public decimal? TotalAPagar { get; set; }
    public decimal? MontoPagado { get; set; }
    public bool EsActivo => !FechaFin.HasValue; // Si no tiene fecha fin, está activo
}

