using Atlas.Domain.Companies;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHealthChecks();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.MapHealthChecks("/health");

app.MapGet("/api", () => Results.Ok(new
{
    application = "Atlas API",
    version = "0.1.0",
    status = "running"
}));

app.MapPost("/api/companies", (CreateCompanyRequest request) =>
{
    try
    {
        var company = new Company(request.Name, request.VatNumber);
        return Results.Created($"/api/companies/{company.Id}", new
        {
            company.Id,
            company.Name,
            company.VatNumber,
            company.IsActive,
            company.CreatedAtUtc
        });
    }
    catch (ArgumentException exception)
    {
        return Results.BadRequest(new { error = exception.Message });
    }
});

app.Run();

public sealed record CreateCompanyRequest(string Name, string VatNumber);
