using System.Text;
using Atlas.Api.Endpoints;
using Atlas.Api.Security;
using Atlas.Infrastructure;
using Atlas.Infrastructure.Seed;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;

Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateBootstrapLogger();
try
{
    var builder = WebApplication.CreateBuilder(args);
    builder.Host.UseSerilog((context, services, logger) => logger.ReadFrom.Configuration(context.Configuration).ReadFrom.Services(services).Enrich.FromLogContext().WriteTo.Console().WriteTo.File("logs/atlas-.log", rollingInterval: RollingInterval.Day));
    builder.Services.AddInfrastructure(builder.Configuration);
    builder.Services.AddScoped<TokenService>();

    var jwtKey = builder.Configuration["Jwt:Key"] ?? throw new InvalidOperationException("Jwt:Key missing");
    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true, ValidateAudience = true, ValidateLifetime = true, ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"], ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)), ClockSkew = TimeSpan.FromSeconds(30)
        };
    });
    builder.Services.AddAuthorization();
    builder.Services.AddHealthChecks();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(options =>
    {
        options.SwaggerDoc("v1", new OpenApiInfo { Title = "ATLAS API", Version = "v0.2" });
        options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme { Name="Authorization", Type=SecuritySchemeType.Http, Scheme="bearer", BearerFormat="JWT", In=ParameterLocation.Header });
        options.AddSecurityRequirement(new OpenApiSecurityRequirement { [new OpenApiSecurityScheme { Reference = new OpenApiReference { Type=ReferenceType.SecurityScheme, Id="Bearer" } }] = Array.Empty<string>() });
    });

    var app = builder.Build();
    app.UseSerilogRequestLogging();
    app.UseSwagger(); app.UseSwaggerUI();
    app.UseHttpsRedirection(); app.UseAuthentication(); app.UseAuthorization();
    app.MapGet("/api", () => Results.Ok(new { name="ATLAS API", version="0.2.0", status="running" })).AllowAnonymous();
    app.MapHealthChecks("/health").AllowAnonymous();
    app.MapAuthEndpoints(); app.MapCompanyEndpoints(); app.MapCustomerEndpoints();
    await DatabaseSeeder.MigrateAndSeedAsync(app.Services, app.Configuration);
    await app.RunAsync();
}
catch (Exception ex) { Log.Fatal(ex, "ATLAS terminated unexpectedly"); }
finally { await Log.CloseAndFlushAsync(); }
