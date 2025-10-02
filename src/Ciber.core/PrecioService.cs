using Ciber.core;

namespace Ciber.core
{
    public interface IPrecioService
    {
        decimal CalcularPrecioTotal(DateTime inicio, DateTime fin, decimal precioPorHora);
        decimal CalcularPrecioTotal(TimeSpan duracion, decimal precioPorHora);
        TimeSpan CalcularDuracion(DateTime inicio, DateTime fin);
        decimal RedondearAPorHoras(TimeSpan duracion);
    }

    public class PrecioService : IPrecioService
    {
        public decimal CalcularPrecioTotal(DateTime inicio, DateTime fin, decimal precioPorHora)
        {
            var duracion = CalcularDuracion(inicio, fin);
            return CalcularPrecioTotal(duracion, precioPorHora);
        }

        public decimal CalcularPrecioTotal(TimeSpan duracion, decimal precioPorHora)
        {
            var horasRedondeadas = RedondearAPorHoras(duracion);
            return horasRedondeadas * precioPorHora;
        }

        public TimeSpan CalcularDuracion(DateTime inicio, DateTime fin)
        {
            return fin - inicio;
        }

        public decimal RedondearAPorHoras(TimeSpan duracion)
        {
            // Redondear hacia arriba (si usa 1 hora y 5 minutos, cobrar 2 horas)
            var horasExactas = duracion.TotalHours;
            return Math.Ceiling((decimal)horasExactas);
        }
    }
}
