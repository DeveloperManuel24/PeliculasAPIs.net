using FluentValidation;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MinimalAPIPeliculas;
using MinimalAPIPeliculas.EndPoints;
using MinimalAPIPeliculas.Entidades;
using MinimalAPIPeliculas.Repositorios;
using MinimalAPIPeliculas.Servicios;
using MinimalAPIPeliculas.Utilidades;

var builder = WebApplication.CreateBuilder(args);
var origenesPermitidos = builder.Configuration.GetValue<string>("origenespermitidos")!;




// Inicio de área de los servicios

////////////Configuracion de Entity
builder.Services.AddDbContext<ApplicationDbContext>(opciones =>
    opciones.UseSqlServer("name=DefaultConnection"));
//////////////////////////


////Configuracion de Identity
builder.Services.AddIdentityCore<IdentityUser>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddScoped<UserManager<IdentityUser>>();
builder.Services.AddScoped<SignInManager<IdentityUser>>();
///////////////////////////////

builder.Services.AddCors(opciones =>
{
    opciones.AddDefaultPolicy(configuracion =>
    {
        configuracion.WithOrigins(origenesPermitidos).AllowAnyHeader().AllowAnyMethod();
    });

    opciones.AddPolicy("libre", configuracion =>
    {
        configuracion.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
    });
});

builder.Services.AddOutputCache();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IRepositorioGeneros, RepositorioGeneros>();//Para inyeccion de Inyeccion de dependencias
builder.Services.AddScoped<IRepositorioActores, RepositorioActores>();
builder.Services.AddScoped<IRepositorioPeliculas, RepositorioPeliculas>();
builder.Services.AddScoped<IRepositorioComentarios, RepositorioComentarios>();
builder.Services.AddScoped<IRepositorioErrores, RepositorioErrores>();

builder.Services.AddTransient<IServicioUsuarios, ServicioUsuarios>();

builder.Services.AddScoped<IAlmacenadorArchivos, AlmacenadorArchivosLocal>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddAutoMapper(typeof(Program));//Configuracion AutoMapper

builder.Services.AddValidatorsFromAssemblyContaining<Program>();

builder.Services.AddProblemDetails(); // Para el manejo de errores en caso una pantalla no se encuentra

//Protección de EndPoints con JWT
builder.Services.AddAuthentication().AddJwtBearer(opciones =>
{
    opciones.MapInboundClaims = false;//ESTO ES VITAL HE IMPORTANTE PORQUE SI QUIERO OBENER EL EMAIL DEL HTTPCONTEXT NO VOY A PODER
    opciones.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = Llaves.ObtenerLlave(builder.Configuration).First(),//Para una sola llave generada por nosotros mismo(lo mejor)
        //IssuerSigningKeys = Llaves.ObtenerTodasLasLlaves(builder.Configuration),//Para multiples llaves
        ClockSkew = TimeSpan.Zero
    };
});

//para la creacion de Roles
builder.Services.AddAuthorization(opciones =>
{
    opciones.AddPolicy("esadmin", politica => politica.RequireClaim("esadmin"));
});





// Fin de área de los servicios

var app = builder.Build();
// Inicio de área de los middleware

//Uso de la fantastica herramienta Swagger:
app.UseSwagger();
app.UseSwaggerUI();
app.UseExceptionHandler(exceptionHandlerApp => exceptionHandlerApp.Run(async context =>
{
    var exceptionHandleFeature = context.Features.Get<IExceptionHandlerFeature>();
    var excepcion = exceptionHandleFeature?.Error;

    var error = new Error();
    error.Fecha = DateTime.UtcNow;
    error.MensajeDeError = excepcion?.Message;
    error.StackTrace = excepcion?.StackTrace;

    var repositorio = context.RequestServices.GetRequiredService<IRepositorioErrores>();
    await repositorio.Crear(error);

    await TypedResults.BadRequest(new
    {
        tipo = "error",
        mensaje = "ha ocurrido un mensaje de error inesperado",
        estatus = 500
    }).ExecuteAsync(context);
}));//Sirve para usar el manejador de excepciones

app.UseStatusCodePages();//Configurar nuestra app para que retorne codigos de status cuando haya un error

app.UseStaticFiles();//Middleware para los archivos estaticos

app.UseCors();

app.UseOutputCache();

//middleware proteccion de EndPoint
app.UseAuthorization();

//EndPoints:

app.MapGroup("/generos").MapGeneros();
app.MapGroup("/actores").MapActores();
app.MapGroup("/peliculas").MapPeliculas();
app.MapGroup("/pelicula/{peliculaId:int}/comentarios").MapComentarios();
app.MapGroup("/usuarios").MapUsuarios();
// Fin de área de los middleware
app.Run();


