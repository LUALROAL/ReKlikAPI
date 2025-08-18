using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using ReKlik.API.Middleware;
using ReKlik.BLL.Services;
using ReKlik.IOC;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Configuración Swagger
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "ReKlik API",
        Version = "v1",
        Description = "API para el sistema de reciclaje ReKlik",
        Contact = new OpenApiContact
        {
            Name = "Equipo de Desarrollo",
            Email = "soporte@reklik.com"
        }
    });

    // Configuración JWT para Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = @"JWT Authorization header usando el esquema Bearer.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });

    // Opcional: Comentarios XML (asegúrate de generar el archivo XML)
    try
    {
        var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        if (System.IO.File.Exists(xmlPath))
        {
            c.IncludeXmlComments(xmlPath);
        }
    }
    catch { /* Ignorar si no hay archivo XML */ }
});

// Inyección de dependencias
builder.Services.InjectDependencys(builder.Configuration);

// Configuración JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });

// Configuración de autorización
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdministradorOnly", policy => policy.RequireRole("administrador"));
    options.AddPolicy("RecicladorOnly", policy => policy.RequireRole("reciclador"));
    options.AddPolicy("PuntoAcopioOnly", policy => policy.RequireRole("punto_acopio"));
    options.AddPolicy("CiudadanoOnly", policy => policy.RequireRole("ciudadano"));
    options.AddPolicy("PuntoAcopioOrAdministrador", policy =>
        policy.RequireRole("punto_acopio", "administrador"));
});

builder.Services.AddAutoMapper(typeof(ReKlik.BLL.Mapping.MappingProfile));

// Agrega esto después de AddControllers() y antes de Build()
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularDev",
        builder => builder
            //.WithOrigins("http://localhost:4203") 
            .WithOrigins("http://localhost:4200")
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials());
});


var app = builder.Build();

// Configuración del pipeline
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();

    // Configuración Swagger UI
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "ReKlik API v1");
        c.RoutePrefix = "swagger"; // Accesible en /swagger
    });
}

app.UseHttpsRedirection();
app.UseRouting();
// Agrega esto después de UseRouting() y antes de UseAuthentication()
app.UseCors("AllowAngularDev");
app.UseAuthentication();
app.UseAuthorization();

// Middleware personalizado
app.UseMiddleware<ExceptionHandlerMiddleware>();

app.MapControllers();

app.Run();