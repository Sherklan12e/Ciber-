using Dapper;
using MySqlConnector;
using Ciber.core;
using System.Data;
namespace Ciber.Dapper
{
public class ADOD: IDAO
{
        private readonly IDbConnection _dbConnection;
        private readonly IPrecioService _precioService;

        public ADOD(IDbConnection connectionString)
        {
            _dbConnection = connectionString;
            _precioService = new PrecioService();
        }
                                        
       public void AgregarCuenta(Cuenta cuenta)
        {
            var parameters = new DynamicParameters();
            parameters.Add("uNcuenta", dbType: DbType.Int32, direction: ParameterDirection.Output);
            parameters.Add("unnombre", cuenta.Nombre);
            parameters.Add("unPas", cuenta.Pass);  // Suponiendo que el pass viene ya en su formato
            parameters.Add("dni", cuenta.Dni);
            parameters.Add("hora", cuenta.HoraRegistrada);

            _dbConnection.Execute("Cuentas", parameters, commandType: CommandType.StoredProcedure);
            
            // Obtener el valor del id generado por el procedimiento almacenado
            cuenta.Ncuenta = parameters.Get<int>("uNcuenta");
        }



        public Cuenta ObtenerCuentaPorId(int ncuenta)
        {
            var sql = "SELECT * FROM Cuenta WHERE Ncuenta = @Ncuenta";
            return _dbConnection.QueryFirstOrDefault<Cuenta>(sql, new { Ncuenta = ncuenta });
        }   

        public void ActualizarCuenta(Cuenta cuenta)
        {
            var sql = "UPDATE Cuenta SET nombre = @Nombre, pass = sha2(@Pass, 256), dni = @Dni, horaRegistrada = @HoraRegistrada WHERE Ncuenta = @Ncuenta";
            _dbConnection.Execute(sql, cuenta);
        }

        public void EliminarCuenta(int ncuenta)
        {
            var sql = "DELETE FROM Cuenta WHERE Ncuenta = @Ncuenta";
            _dbConnection.Execute(sql, new { Ncuenta = ncuenta });
        }


        public IEnumerable<Cuenta> ObtenerTodasLasCuentas()
        {
            var sql = "SELECT * FROM Cuenta";
            return _dbConnection.Query<Cuenta>(sql);
        }


       public void AgregarMaquina(Maquina maquina)
        {
            var sql = "INSERT INTO Maquina (estado, caracteristicas, precioPorHora, tipoMaquina) VALUES (@Estado, @Caracteristicas, @PrecioPorHora, @TipoMaquina); SELECT LAST_INSERT_ID();";
            var id = _dbConnection.QuerySingle<int>(sql, maquina);
            maquina.Nmaquina = id;
        }


      
        public Maquina ObtenerMaquinaPorId(int nmaquina)
        {
            var sql = "SELECT * FROM Maquina WHERE Nmaquina = @Nmaquina";
            return _dbConnection.QueryFirstOrDefault<Maquina>(sql, new { Nmaquina = nmaquina });
        }

        public void ActualizarMaquina(Maquina maquina)
        {
            var sql = "UPDATE Maquina SET estado = @Estado, caracteristicas = @Caracteristicas, precioPorHora = @PrecioPorHora, tipoMaquina = @TipoMaquina WHERE Nmaquina = @Nmaquina";
            _dbConnection.Execute(sql, maquina);
        }

        public void EliminarMaquina(int nmaquina)
        {
            var sql = "DELETE FROM Maquina WHERE Nmaquina = @Nmaquina";
            _dbConnection.Execute(sql, new { Nmaquina = nmaquina });
        }

        public IEnumerable<Maquina> ObtenerTodasLasMaquinas(){
            var sql = "SELECT * FROM Maquina";
            return _dbConnection.Query<Maquina>(sql);
        }




        public void AgregarTipo(Tipo tipo)
        {
            var sql = "INSERT INTO Tipo (IdTipo, TipoDescripcion) VALUES (@IdTipo, @TipoDescripcion)";
            _dbConnection.Execute(sql, tipo);
        }

       public void AgregarAlquiler(Alquiler alquiler, bool tipoAlquiler)
        {
            // Obtener el precio de la máquina
            var maquina = ObtenerMaquinaPorId(alquiler.Nmaquina);
            alquiler.PrecioPorHora = maquina.PrecioPorHora;
            
            // Calcular el total si es tipo 2 (tiempo definido)
            if (tipoAlquiler && alquiler.CantidadTiempo.HasValue)
            {
                alquiler.TotalAPagar = _precioService.CalcularPrecioTotal(alquiler.CantidadTiempo.Value, alquiler.PrecioPorHora);
            }
            
            var sql = @"INSERT INTO Alquiler (Ncuenta, Nmaquina, tipo, cantidadTiempo, pagado, fechaInicio, precioPorHora, totalAPagar, montoPagado) 
                       VALUES (@Ncuenta, @Nmaquina, @Tipo, @CantidadTiempo, @Pagado, @FechaInicio, @PrecioPorHora, @TotalAPagar, @MontoPagado); 
                       SELECT LAST_INSERT_ID();";
            
            var id = _dbConnection.QuerySingle<int>(sql, alquiler);
            alquiler.IdAlquiler = id;
            
            // Marcar la máquina como ocupada
            ActualizarEstadoMaquina(alquiler.Nmaquina, false);
        }



        public Alquiler ObtenerAlquilerPorId(int idAlquiler)
        {
            var sql = "SELECT * FROM Alquiler WHERE idAlquiler = @IdAlquiler";
            return _dbConnection.QueryFirstOrDefault<Alquiler>(sql, new { IdAlquiler = idAlquiler });
        }

        public void EliminarAlquiler(int idAlquiler)
        {
            var sql = "DELETE FROM Alquiler WHERE idAlquiler = @IdAlquiler";
            _dbConnection.Execute(sql, new { IdAlquiler = idAlquiler });
        }

        public IEnumerable<Alquiler> ObtenerTodosLosAlquileres()
        {
            var sql = "SELECT * FROM Alquiler";
            return _dbConnection.Query<Alquiler>(sql);
        }

        public void AgregarHistorial(HistorialdeAlquiler historial)
        {
            var sql = "INSERT INTO HistorialdeAlquiler (Ncuenta, Nmaquina, fechaInicio, fechaFin, TotalPagar) VALUES (@Ncuenta, @Nmaquina, @FechaInicio, @FechaFin, @TotalPagar)";
            _dbConnection.Execute(sql, historial);
        }

        public HistorialdeAlquiler ObtenerHistorialPorId(int idHistorial)
        {
            var sql = "SELECT * FROM HistorialdeAlquiler WHERE idHistorial = @IdHistorial";
            return _dbConnection.QueryFirstOrDefault<HistorialdeAlquiler>(sql, new { IdHistorial = idHistorial });
        }
       

        public IEnumerable<HistorialdeAlquiler> ObtenerTodoElHistorial()

        {
            var sql = "SELECT * FROM HistorialdeAlquiler";
            return _dbConnection.Query<HistorialdeAlquiler>(sql);
        }
        

        // Métodos asincronos ------------------------------------------------------------

        public async Task AgregarCuentaAsync(Cuenta cuenta)
        {
            var parameters = new DynamicParameters();
            parameters.Add("uNcuenta", dbType: DbType.Int32, direction: ParameterDirection.Output);
            parameters.Add("unnombre", cuenta.Nombre);
            parameters.Add("unPas", cuenta.Pass);
            parameters.Add("dni", cuenta.Dni);
            parameters.Add("hora", cuenta.HoraRegistrada);
            await _dbConnection.ExecuteAsync("Cuentas", parameters, commandType: CommandType.StoredProcedure);
            cuenta.Ncuenta = parameters.Get<int>("uNcuenta");
        }

        public async Task<Cuenta> ObtenerCuentaPorIdAsync(int ncuenta)
        {
            var sql = "SELECT * FROM Cuenta WHERE Ncuenta = @Ncuenta";
            return await _dbConnection.QueryFirstOrDefaultAsync<Cuenta>(sql, new { Ncuenta = ncuenta });
        }

        public async Task ActualizarCuentaAsync(Cuenta cuenta)
        {
            var sql = "UPDATE Cuenta SET nombre = @Nombre, pass = sha2(@Pass, 256), dni = @Dni, horaRegistrada = @HoraRegistrada WHERE Ncuenta = @Ncuenta";
            await _dbConnection.ExecuteAsync(sql, cuenta);
        }

        public async Task EliminarCuentaAsync(int ncuenta)
        {
            var sql = "DELETE FROM Cuenta WHERE Ncuenta = @Ncuenta";
            await _dbConnection.ExecuteAsync(sql, new { Ncuenta = ncuenta });
        }

        public async Task<IEnumerable<Cuenta>> ObtenerTodasLasCuentasAsync()
        {
            var sql = "SELECT * FROM Cuenta";
            return await _dbConnection.QueryAsync<Cuenta>(sql);
        }

        public async Task AgregarMaquinaAsync(Maquina maquina)
        {
            var parameters = new DynamicParameters();
            parameters.Add("uNmaquina", dbType: DbType.Int32, direction: ParameterDirection.Output);
            parameters.Add("unestado", maquina.Estado);
            parameters.Add("UnaCaracteristicas", maquina.Caracteristicas);
            await _dbConnection.ExecuteAsync("Maquinas", parameters, commandType: CommandType.StoredProcedure);
            maquina.Nmaquina = parameters.Get<int>("uNmaquina");
        }

        public async Task<Maquina> ObtenerMaquinaPorIdAsync(int nmaquina)
        {
            var sql = "SELECT * FROM Maquina WHERE Nmaquina = @Nmaquina";
            return await _dbConnection.QueryFirstOrDefaultAsync<Maquina>(sql, new { Nmaquina = nmaquina });
        }

        public async Task ActualizarMaquinaAsync(Maquina maquina)
        {
            var sql = "UPDATE Maquina SET estado = @Estado, caracteristicas = @Caracteristicas WHERE Nmaquina = @Nmaquina";
            await _dbConnection.ExecuteAsync(sql, maquina);
        }

        public async Task EliminarMaquinaAsync(int nmaquina)
        {
            var sql = "DELETE FROM Maquina WHERE Nmaquina = @Nmaquina";
            await _dbConnection.ExecuteAsync(sql, new { Nmaquina = nmaquina });
        }

        public async Task<IEnumerable<Maquina>> ObtenerTodasLasMaquinasAsync()
        {
            var sql = "SELECT * FROM Maquina";
            return await _dbConnection.QueryAsync<Maquina>(sql);
        }

        public async Task AgregarTipoAsync(Tipo tipo)
        {
            var sql = "INSERT INTO Tipo (IdTipo, TipoDescripcion) VALUES (@IdTipo, @TipoDescripcion)";
            await _dbConnection.ExecuteAsync(sql, tipo);
        }

        public async Task AgregarAlquilerAsync(Alquiler alquiler, bool tipoAlquiler)
        {
            // Obtener precio de la máquina y setear en el alquiler
            var maquina = await ObtenerMaquinaPorIdAsync(alquiler.Nmaquina);
            alquiler.PrecioPorHora = maquina.PrecioPorHora;

            // Si es tipo 2 (cantidad de tiempo definida), calcular el total
            if (tipoAlquiler && alquiler.CantidadTiempo.HasValue)
            {
                var horasRedondeadas = Math.Ceiling(alquiler.CantidadTiempo.Value.TotalHours);
                alquiler.TotalAPagar = (decimal)horasRedondeadas * alquiler.PrecioPorHora;
            }

            var sql = @"INSERT INTO Alquiler 
                        (Ncuenta, Nmaquina, tipo, cantidadTiempo, pagado, fechaInicio, precioPorHora, totalAPagar, montoPagado)
                        VALUES (@Ncuenta, @Nmaquina, @Tipo, @CantidadTiempo, @Pagado, @FechaInicio, @PrecioPorHora, @TotalAPagar, @MontoPagado);
                        SELECT LAST_INSERT_ID();";

            var id = await _dbConnection.QuerySingleAsync<int>(sql, alquiler);
            alquiler.IdAlquiler = id;

            // Marcar máquina como ocupada
            await ActualizarEstadoMaquinaAsync(alquiler.Nmaquina, false);
        }

        public async Task<Alquiler> ObtenerAlquilerPorIdAsync(int idAlquiler)
        {
            var sql = "SELECT * FROM Alquiler WHERE idAlquiler = @IdAlquiler";
            return await _dbConnection.QueryFirstOrDefaultAsync<Alquiler>(sql, new { IdAlquiler = idAlquiler });
        }

        public async Task EliminarAlquilerAsync(int idAlquiler)
        {
            var sql = "DELETE FROM Alquiler WHERE idAlquiler = @IdAlquiler";
            await _dbConnection.ExecuteAsync(sql, new { IdAlquiler = idAlquiler });
        }

        public async Task<IEnumerable<Alquiler>> ObtenerTodosLosAlquileresAsync()
        {
            var sql = "SELECT * FROM Alquiler";
            return await _dbConnection.QueryAsync<Alquiler>(sql);
        }

        public async Task AgregarHistorialAsync(HistorialdeAlquiler historial)
        {
            var sql = "INSERT INTO HistorialdeAlquiler (Ncuenta, Nmaquina, fechaInicio, fechaFin, TotalPagar) VALUES (@Ncuenta, @Nmaquina, @FechaInicio, @FechaFin, @TotalPagar)";
            await _dbConnection.ExecuteAsync(sql, historial);
        }

        public async Task<HistorialdeAlquiler> ObtenerHistorialPorIdAsync(int idHistorial)
        {
            var sql = "SELECT * FROM HistorialdeAlquiler WHERE idHistorial = @IdHistorial";
            return await _dbConnection.QueryFirstOrDefaultAsync<HistorialdeAlquiler>(sql, new { IdHistorial = idHistorial });
        }

        public async Task<IEnumerable<HistorialdeAlquiler>> ObtenerTodoElHistorialAsync()
        {
            var sql = "SELECT * FROM HistorialdeAlquiler";
            return await _dbConnection.QueryAsync<HistorialdeAlquiler>(sql);
        }
        public async Task<IEnumerable<Maquina>> ObtenerMaquinaDisponiblesAsync()
        {
            var sql = "SELECT * FROM Maquina WHERE estado";
            return await _dbConnection.QueryAsync<Maquina>(sql);
        }

        public async Task<IEnumerable<Maquina>> ObtenerMaquinaNoDisponiblesesAsync()
        {
            var sql = "SELECT * FROM Maquina WHERE NOT estado";
            return await _dbConnection.QueryAsync<Maquina>(sql);
        }

        // Métodos auxiliares
        private void ActualizarEstadoMaquina(int nmaquina, bool estado)
        {
            var sql = "UPDATE Maquina SET estado = @Estado WHERE Nmaquina = @Nmaquina";
            _dbConnection.Execute(sql, new { Estado = estado, Nmaquina = nmaquina });
        }

        // Nuevos métodos para la lógica mejorada
        public void FinalizarAlquiler(int idAlquiler, decimal montoPagado)
        {
            var alquiler = ObtenerAlquilerPorId(idAlquiler);
            if (alquiler != null)
            {
                var fechaFin = DateTime.Now;
                var totalAPagar = _precioService.CalcularPrecioTotal(alquiler.FechaInicio, fechaFin, alquiler.PrecioPorHora);
                
                var sql = @"UPDATE Alquiler SET fechaFin = @FechaFin, totalAPagar = @TotalAPagar, montoPagado = @MontoPagado, pagado = @Pagado 
                           WHERE idAlquiler = @IdAlquiler";
                
                _dbConnection.Execute(sql, new { 
                    FechaFin = fechaFin, 
                    TotalAPagar = totalAPagar, 
                    MontoPagado = montoPagado,
                    Pagado = montoPagado >= totalAPagar,
                    IdAlquiler = idAlquiler 
                });
                
                // Liberar la máquina
                ActualizarEstadoMaquina(alquiler.Nmaquina, true);
                
                // Agregar al historial
                var historial = new HistorialdeAlquiler
                {
                    Ncuenta = alquiler.Ncuenta,
                    Nmaquina = alquiler.Nmaquina,
                    FechaInicio = alquiler.FechaInicio,
                    FechaFin = fechaFin,
                    TotalPagar = totalAPagar,
                    MontoPagado = montoPagado,
                    PrecioPorHora = alquiler.PrecioPorHora
                };
                
                AgregarHistorial(historial);
            }
        }

        public async Task FinalizarAlquilerAsync(int idAlquiler, decimal montoPagado)
        {
            var alquiler = await ObtenerAlquilerPorIdAsync(idAlquiler);
            if (alquiler != null)
            {
                var fechaFin = DateTime.Now;
                var totalAPagar = _precioService.CalcularPrecioTotal(alquiler.FechaInicio, fechaFin, alquiler.PrecioPorHora);
                
                var sql = @"UPDATE Alquiler SET fechaFin = @FechaFin, totalAPagar = @TotalAPagar, montoPagado = @MontoPagado, pagado = @Pagado 
                           WHERE idAlquiler = @IdAlquiler";
                
                await _dbConnection.ExecuteAsync(sql, new { 
                    FechaFin = fechaFin, 
                    TotalAPagar = totalAPagar, 
                    MontoPagado = montoPagado,
                    Pagado = montoPagado >= totalAPagar,
                    IdAlquiler = idAlquiler 
                });
                
                // Liberar la máquina
                await ActualizarEstadoMaquinaAsync(alquiler.Nmaquina, true);
                
                // Agregar al historial
                var historial = new HistorialdeAlquiler
                {
                    Ncuenta = alquiler.Ncuenta,
                    Nmaquina = alquiler.Nmaquina,
                    FechaInicio = alquiler.FechaInicio,
                    FechaFin = fechaFin,
                    TotalPagar = totalAPagar,
                    MontoPagado = montoPagado,
                    PrecioPorHora = alquiler.PrecioPorHora
                };
                
                await AgregarHistorialAsync(historial);
            }
        }

        public IEnumerable<Alquiler> ObtenerAlquileresActivos()
        {
            var sql = "SELECT * FROM Alquiler WHERE fechaFin IS NULL";
            return _dbConnection.Query<Alquiler>(sql);
        }

        public async Task<IEnumerable<Alquiler>> ObtenerAlquileresActivosAsync()
        {
            var sql = "SELECT * FROM Alquiler WHERE fechaFin IS NULL";
            return await _dbConnection.QueryAsync<Alquiler>(sql);
        }

        private async Task ActualizarEstadoMaquinaAsync(int nmaquina, bool estado)
        {
            var sql = "UPDATE Maquina SET estado = @Estado WHERE Nmaquina = @Nmaquina";
            await _dbConnection.ExecuteAsync(sql, new { Estado = estado, Nmaquina = nmaquina });
        }

    }   

}