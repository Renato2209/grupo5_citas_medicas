using System.Reflection;
using System.Text.Json.Serialization;
using CitasMedicas.Api.Exceptions;
using CitasMedicas.Api.Services;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Rutas en minúsculas (/api/citas) y estados del enum aceptados como número o como texto
builder.Services.AddRouting(options => options.LowercaseUrls = true);
builder.Services.AddControllers()
    .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

// Respuesta uniforme para errores de validación (anotaciones) -> HTTP 400
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var problem = new ValidationProblemDetails(context.ModelState)
        {
            Status = StatusCodes.Status400BadRequest,
            Title = "Datos de entrada inválidos",
            Detail = "Revise los campos indicados en 'errors'.",
            Instance = context.HttpContext.Request.Path
        };
        return new BadRequestObjectResult(problem) { ContentTypes = { "application/problem+json" } };
    };
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    // Documentación de endpoints a partir de los comentarios XML (///)
    var xml = Path.Combine(AppContext.BaseDirectory, $"{Assembly.GetExecutingAssembly().GetName().Name}.xml");
    options.IncludeXmlComments(xml);
});

// Singleton: los datos en memoria deben vivir mientras corre la aplicación
builder.Services.AddSingleton<ICatalogoService, CatalogoService>();
builder.Services.AddSingleton<ICitaService, CitaService>();

// Manejo de excepciones centralizado
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

var app = builder.Build();

app.UseExceptionHandler();
app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();

app.Run();
