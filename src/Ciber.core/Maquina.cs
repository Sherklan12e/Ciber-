namespace Ciber.core;

public class Maquina
{
    public int Nmaquina { get; set; }
    public bool Estado { get; set; }
    public string Caracteristicas { get; set; } = "";
    public decimal PrecioPorHora { get; set; } = 5.00m; // Precio por defecto
    public string TipoMaquina { get; set; } = "Estándar"; // Gaming, Estándar, Premium
}

