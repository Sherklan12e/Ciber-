using System.Data;
using MySqlConnector;
using Scalar.AspNetCore;
using Ciber.Dapper;
using Ciber.core;
using MinimalAPI.DTO;


var builder = WebApplication.CreateBuilder(args);

//  Obtener la cadena de conexión desde appsettings.json
var connectionString = builder.Configuration.GetConnectionString("MySQL");

//  Registrando IDbConnection para que se inyecte como dependencia
//  Cada vez que se inyecte, se creará una nueva instancia con la cadena de conexión
builder.Services.AddScoped<IDbConnection>(sp => new MySqlConnection(connectionString));

//Cada vez que necesite la interfaz, se va a instanciar automaticamente AdoDapper y se va a pasar al metodo de la API
builder.Services.AddScoped<IDAO, ADOD>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger(options =>
    {
        options.RouteTemplate = "/openapi/{documentName}.json";
    });
    app.MapScalarApiReference();
}


// ALQUILERES

app.MapGet("/alquileres", async (IDAO db) =>
{
    var alquileres = await db.ObtenerTodosLosAlquileresAsync();
    return Results.Ok(alquileres.Select(alquileres => new AlquilerDto(alquileres.Tipo, alquileres.CantidadTiempo, alquileres.Pagado )));
});

app.MapGet("/alquileres/{id}", async (int id, IDAO db) =>
{
    var alquiler = await db.ObtenerAlquilerPorIdAsync(id);
    return alquiler is not null ? Results.Ok(alquiler) : Results.NotFound();
});

app.MapPost("/alquileres", async (AlquilerAltaDTO alquiler, IDAO db) =>
{

    Alquiler alquileralta = new Alquiler
    {
        Ncuenta = alquiler.ncuenta,
        Nmaquina = alquiler.nmaquina,
        Tipo = alquiler.tipo,
        CantidadTiempo = alquiler.cantidadTiempo,
        Pagado = alquiler.Pagado
    };

    await db.AgregarAlquilerAsync(alquileralta, true);
    return Results.Created($"/alquileres/{alquileralta.IdAlquiler}", alquiler);
});



// maquinas

app.MapGet("/maquinas", async (IDAO db) =>
{
    var maquinas = await db.ObtenerTodasLasMaquinasAsync();
    return Results.Ok(maquinas.Select(maquina => new MaquinaDto(maquina.Estado, maquina.Caracteristicas)));
});

app.MapGet("/maquinas/{id}", async (int id, IDAO db) =>
{
    var maquina = await db.ObtenerMaquinaPorIdAsync(id);
    return maquina is not null ? Results.Ok(maquina) : Results.NotFound();
});

app.MapPost("/maquinas", async (MaquinaDto maquina, IDAO db) =>
{
    Maquina maquinaAlta = new Maquina
    {
        Estado = maquina.Estado,
        Caracteristicas = maquina.Caracteristicas
    };
    await db.AgregarMaquinaAsync(maquinaAlta);
    return Results.Created($"/maquinas/{maquinaAlta.Nmaquina}", maquina);
});

// Cuentas

app.MapGet("/cuentas", async (IDAO db) =>
{
    var cuentas = await db.ObtenerTodasLasCuentasAsync();
    return Results.Ok(cuentas.Select(cuenta => new CuentaDto(cuenta.Nombre, cuenta.Dni, cuenta.HoraRegistrada )) );
});

app.MapGet("/cuentas/{id}", async (int id, IDAO db) =>
{
    var cuenta = await db.ObtenerCuentaPorIdAsync(id);
    return cuenta is not null ? Results.Ok(cuenta) : Results.NotFound();
});

app.MapPost("/cuentas", async (Cuenta cuenta, IDAO db) =>
{
    await db.AgregarCuentaAsync(cuenta);
    return Results.Created($"/cuentas/{cuenta.Ncuenta}", cuenta);
});

app.MapGet("/maquinas/disponibles", async (IDAO db) =>
{
    var disponibles = await db.ObtenerMaquinaDisponiblesAsync();
    return Results.Ok(disponibles.Select(disponibles => new MaquinaDto(disponibles.Estado , disponibles.Caracteristicas)));
});



app.MapGet("/maquinas/no-disponibles", async (IDAO db) =>
{
    var noDisponibles = await db.ObtenerMaquinaNoDisponiblesesAsync();
    return Results.Ok(noDisponibles.Select(noDisponibles=> new MaquinaDto(noDisponibles.Estado ,noDisponibles.Caracteristicas)));
});


app.Run();