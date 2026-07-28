using Atlas.Api.Contracts;
using Atlas.Domain.Companies;
using Atlas.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Api.Endpoints;

public static class CompanyEndpoints
{
    public static IEndpointRouteBuilder MapCompanyEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/companies").WithTags("Companies").RequireAuthorization();

        group.MapGet("/", async (AtlasDbContext db, CancellationToken ct) => Results.Ok(await db.Companies.AsNoTracking().OrderBy(x => x.Name).ToListAsync(ct)));
        group.MapGet("/{id:guid}", async (Guid id, AtlasDbContext db, CancellationToken ct) => await db.Companies.AsNoTracking().SingleOrDefaultAsync(x => x.Id == id, ct) is { } item ? Results.Ok(item) : Results.NotFound());
        group.MapPost("/", async (CompanyRequest request, AtlasDbContext db, CancellationToken ct) =>
        {
            var item = new Company { Name = request.Name.Trim(), LegalName = request.LegalName, TaxNumber = request.TaxNumber, Email = request.Email, Phone = request.Phone, IsActive = request.IsActive };
            db.Companies.Add(item); await db.SaveChangesAsync(ct); return Results.Created($"/api/companies/{item.Id}", item);
        }).RequireAuthorization(p => p.RequireRole("SystemAdministrator"));
        group.MapPut("/{id:guid}", async (Guid id, CompanyRequest request, AtlasDbContext db, CancellationToken ct) =>
        {
            var item = await db.Companies.SingleOrDefaultAsync(x => x.Id == id, ct); if (item is null) return Results.NotFound();
            item.Name=request.Name.Trim(); item.LegalName=request.LegalName; item.TaxNumber=request.TaxNumber; item.Email=request.Email; item.Phone=request.Phone; item.IsActive=request.IsActive;
            await db.SaveChangesAsync(ct); return Results.Ok(item);
        }).RequireAuthorization(p => p.RequireRole("SystemAdministrator"));
        group.MapDelete("/{id:guid}", async (Guid id, AtlasDbContext db, CancellationToken ct) =>
        {
            var item = await db.Companies.SingleOrDefaultAsync(x => x.Id == id, ct); if (item is null) return Results.NotFound(); db.Companies.Remove(item); await db.SaveChangesAsync(ct); return Results.NoContent();
        }).RequireAuthorization(p => p.RequireRole("SystemAdministrator"));
        return app;
    }
}
