using System.Text;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using BankMore.Auth.API.Middleware;
using BankMore.Auth.API.Seguranca;
using BankMore.Auth.Application.Abstractions;
using BankMore.Auth.Application.Commands;
using BankMore.Auth.Application.Behaviors;
using BankMore.Auth.Domain.Repositories;
using BankMore.Auth.Infrastructure.Persistence;
using BankMore.Auth.Infrastructure.Repositories;

using Microsoft.OpenApi;

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
    // EnableRetryOnFailure: deadlock/falha transitória refaz a transação inteira (ver TransferenciaFinanceiraEfCore).
    builder.Services.AddDbContext<BankMoreDbContext>(options =>
        options.UseNpgsql(connectionString, npgsql => npgsql.EnableRetryOnFailure(3)));
}
else
{
    var connectionString = builder.Configuration.GetConnectionString("SqlServer");
    builder.Services.AddDbContext<BankMoreDbContext>(options =>
        options.UseSqlServer(connectionString, sql => sql.EnableRetryOnFailure(3)));
}
#endregion // ===============================================================================================

#region 📦 Injeções de Dependência (Repositories) // ===================================================
// Usuário autenticado: a Application recebe IUsuarioAtual, só a API conhece HttpContext.
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IUsuarioAtual, UsuarioAtualHttp>();

// Repositórios EF Core (PostgreSQL / Dual Provider)
builder.Services.AddScoped<IContaCorrenteRepository, ContaCorrenteRepositoryEfCore>();
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepositoryEfCore>();
builder.Services.AddScoped<IMovimentoRepository, MovimentoRepositoryEfCore>();
builder.Services.AddScoped<ITransferenciaRepository, TransferenciaRepositoryEfCore>();
builder.Services.AddScoped<IIdempotenciaRepository, IdempotenciaRepositoryEfCore>();
builder.Services.AddScoped<ITransferenciaFinanceira, TransferenciaFinanceiraEfCore>();
builder.Services.AddScoped<IMovimentacaoFinanceira, MovimentacaoFinanceiraEfCore>();
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

// A chave NÃO fica no appsettings.json versionado. Local: appsettings.Development.json ou user-secrets.
// Docker: variável JwtSettings__SecretKey vinda do arquivo .env (não versionado).
if (string.IsNullOrWhiteSpace(secretKey) || Encoding.UTF8.GetByteCount(secretKey) < 32)
    throw new InvalidOperationException(
        "JwtSettings:SecretKey ausente ou com menos de 32 bytes. Configure via user-secrets ou variável de ambiente.");

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
    c.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference("Bearer", document)] = []
    });
});
builder.Services.AddRouting(options => options.LowercaseUrls = true);
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
#endregion

var app = builder.Build();

#region 🧱 Tratamento Global de Erros
// ProblemDetails (RFC 9457) + mapeamento exceção → status em GlobalExceptionHandler.
app.UseExceptionHandler();
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
