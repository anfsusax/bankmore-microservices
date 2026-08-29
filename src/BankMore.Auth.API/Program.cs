using System.Text;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using BankMore.Auth.Application.Commands;
using BankMore.Auth.Application.Behaviors;
using BankMore.Auth.Domain.Repositories;
using BankMore.Auth.Infrastructure.Persistence;
using BankMore.Auth.Infrastructure.Repositories;

using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

#region 🔧 Configuração básica
builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
    .AddEnvironmentVariables();
#endregion

#region 🗄️ Banco de Dados (EF Core - Dual Provider) // ====================================================
var isDocker = Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true";

if (isDocker)
{
    var connectionString = builder.Configuration.GetConnectionString("PostgreSql") 
                           ?? builder.Configuration.GetConnectionString("MySql");
    builder.Services.AddDbContext<BankMoreDbContext>(options =>
        options.UseNpgsql(connectionString));
}
else
{
    var connectionString = builder.Configuration.GetConnectionString("SqlServer");
    builder.Services.AddDbContext<BankMoreDbContext>(options =>
        options.UseSqlServer(connectionString));
}
#endregion // ===============================================================================================

#region 📦 Injeções de Dependência (Repositories) // ===================================================
// HttpContextAccessor necessário para alguns handlers
builder.Services.AddHttpContextAccessor();

// Repositórios EF Core (PostgreSQL / Dual Provider)
builder.Services.AddScoped<IContaCorrenteRepository, ContaCorrenteRepositoryEfCore>();
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepositoryEfCore>();
builder.Services.AddScoped<IMovimentoRepository, MovimentoRepositoryEfCore>();
builder.Services.AddScoped<ITransferenciaRepository, TransferenciaRepositoryEfCore>();
builder.Services.AddScoped<IIdempotenciaRepository, IdempotenciaRepositoryEfCore>();
builder.Services.AddScoped<ITransferenciaFinanceira, TransferenciaFinanceiraEfCore>();
#endregion // ===============================================================================================

#region ⚙️ MediatR + FluentValidation // ====================================================================
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(CriarUsuarioCommand).Assembly));

builder.Services.AddValidatorsFromAssembly(typeof(CriarUsuarioCommand).Assembly);
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
#endregion // ===============================================================================================

#region 🔐 JWT Authentication
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings.GetValue<string>("SecretKey");

if (string.IsNullOrEmpty(secretKey))
    throw new Exception("JWT SecretKey não está configurada no appsettings.json");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidateAudience = true,
            ValidAudience = jwtSettings["Audience"],
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
            ClockSkew = TimeSpan.Zero
        };
    });
#endregion

#region 🌐 Controllers + Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "BankMore API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Insira o token JWT neste formato: Bearer {seu_token}",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
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
            Array.Empty<string>()
        }
    });
});
builder.Services.AddRouting(options => options.LowercaseUrls = true);
#endregion

var app = builder.Build();

#region 🧱 Middleware Global de Erros
app.UseExceptionHandler(exceptionApp =>
{
    exceptionApp.Run(async context =>
    {
        context.Response.ContentType = "application/json";

        var exception = context.Features.Get<IExceptionHandlerFeature>()?.Error;
        object problem;

        if (exception is ValidationException validationEx)
        {
            problem = new
            {
                Status = 400,
                Title = "Erro de validação",
                Detail = "Dados inválidos fornecidos",
                Errors = validationEx.Errors.Select(e => new { Field = e.PropertyName, Message = e.ErrorMessage })
            };
        }
        else if (exception is ArgumentException argEx)
        {
            problem = new
            {
                Status = 400,
                Title = "Erro de argumento",
                Detail = argEx.Message
            };
        }
        else if (exception is UnauthorizedAccessException)
        {
            problem = new
            {
                Status = 401,
                Title = "Não autorizado",
                Detail = exception.Message
            };
        }
        else if (exception is InvalidOperationException)
        {
            problem = new
            {
                Status = 400,
                Title = "Operação inválida",
                Detail = exception.Message
            };
        }
        else
        {
            problem = new
            {
                Status = 500,
                Title = "Erro interno",
                Detail = "Ocorreu um erro inesperado."
            };
        }

        context.Response.StatusCode = ((dynamic)problem).Status;
        await context.Response.WriteAsJsonAsync(problem);
    });
});
#endregion

#region 🚀 Pipeline da Aplicação
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
#endregion
