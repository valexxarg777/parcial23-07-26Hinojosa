using CatalogoAPI.Data;
using Microsoft.EntityFrameworkCore;

// Program.cs es el punto de arranque de toda la aplicación.
// Acá se configuran los "servicios" (piezas que la app va a usar)
// y el "pipeline" (el camino que recorre cada request HTTP que llega).
var builder = WebApplication.CreateBuilder(args);

// --- SERVICIOS: todo lo que la app va a tener disponible ---

// Habilita el uso de Controllers (clases con [ApiController], como
// nuestro ProductosController) para responder a las rutas HTTP.
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(); // Genera la interfaz de Swagger para probar la API a mano

// Acá está la magia de la conexión a la base de datos:
// le decimos "cuando alguien pida un CatalogoContext, dale una instancia
// configurada para usar SQL Server, en la base CatalogoDB de tu LocalDB".
// Esto es lo que después el constructor de ProductosController recibe
// automáticamente (Dependency Injection: vos no hacés "new CatalogoContext()"
// en ningún lado, ASP.NET Core te lo entrega solo).
//
// "(localdb)\mssqllocaldb" es el motor de SQL Server que viene
// instalado junto con Visual Studio / SSMS. Si vos ya usás SSMS y te
// conectás a otro servidor (ej: uno con usuario y contraseña, o
// "localhost"), reemplazá esta cadena por la que usás normalmente
// para conectarte en SSMS.
builder.Services.AddDbContext<CatalogoContext>(options =>
    options.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=CatalogoDB;Trusted_Connection=True;MultipleActiveResultSets=true"));

// CORS = Cross-Origin Resource Sharing. Por seguridad, un navegador
// por defecto bloquea que una página web (ej: tu frontend en
// localhost:5500) llame a una API en otro puerto (localhost:5150).
// Con esto le decimos a la API "permití que te llamen desde cualquier
// origen". En un proyecto real de producción esto se restringe,
// pero para el parcial está perfecto así.
builder.Services.AddCors(options =>
{
    options.AddPolicy("PermitirTodo", policy =>
    {
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});

var app = builder.Build();

// --- PIPELINE: el camino que recorre cada request ---

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Permite servir la página web de la carpeta wwwroot (index.html, script.js, style.css)
app.UseDefaultFiles();
app.UseStaticFiles();

// El orden acá importa: CORS tiene que ir antes de que se procesen
// las rutas, si no los headers de permiso no llegan a tiempo.
app.UseCors("PermitirTodo");

app.UseAuthorization();

// Le dice a la app "las rutas de mis Controllers (los [HttpGet], [HttpPost], etc.
// que definiste) están activas, empezá a escuchar requests".
app.MapControllers();

app.Run();