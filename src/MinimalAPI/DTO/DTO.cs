
namespace MinimalAPI.DTO;

// obtener todos los arlquileres
public readonly record struct AlquilerDto(int Tipo , TimeSpan? CantidadTiempo , bool? Pagado);

// obtener todos los cuenta
public readonly record struct CuentaDto(string Nombre , int Dni, TimeSpan horaregistrada);
// obtener todas las maquinas
public readonly record struct MaquinaDto(bool Estado, string Caracteristicas);


// Alta Alquiler 
public readonly record struct AlquilerAltaDTO(int ncuenta, int nmaquina, int tipo, TimeSpan? cantidadTiempo, bool Pagado);
// Alta maquina 
public readonly record struct MaquinalAltaDTIO(bool Estado , string Caracteristicas);





public readonly record struct HistorialDto(int AlquilerId, DateTime Fecha, string Accion, string Comentario);
public readonly record struct PostAlquilerDto(int AlquilerId, string Comentario);
