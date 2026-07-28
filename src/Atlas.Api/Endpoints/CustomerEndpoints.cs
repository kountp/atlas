using Atlas.Api.Contracts;
using Atlas.Domain.Companies;
using Atlas.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Api.Endpoints;

public static class CustomerEndpoints
{
    public static IEndpointRouteBuilder MapCustomerEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/customers").WithTags("Customers").RequireAuthorization();
        group.MapGet("/", async (Guid? companyId, AtlasDbContext db, CancellationToken ct) =>
        {
            var q = db.Customers.AsNoTracking(); if (companyId.HasValue) q = q.Where(x => x.CompanyId == companyId.Value);
            return Results.Ok(await q.OrderBy(x => x.Name).ToListAsync(ct));
        });
        group.MapGet("/{id:guid}", async (Guid id, AtlasDbContext db, CancellationToken ct) => await db.Customers.AsNoTracking().SingleOrDefaultAsync(x => x.Id == id, ct) is { } item ? Results.Ok(item) : Results.NotFound());
        group.MapPost("/", async (CustomerRequest request, AtlasDbContext db, CancellationToken ct) =>
        {
            if (!await db.Companies.AnyAsync(x => x.Id == request.CompanyId, ct)) return Results.BadRequest(new { error = "Company does not exist." });
            var item = new Customer { CompanyId=request.CompanyId, Name=request.Name.Trim(), LegalName=request.LegalName, TaxNumber=request.TaxNumber, Email=request.Email, Phone=request.Phone, AddressLine1=request.AddressLine1, City=request.City, PostalCode=request.PostalCode, CountryCode=request.CountryCode.ToUpperInvariant(), IsActive=request.IsActive };
            db.Customers.Add(item); await db.SaveChangesAsync(ct); return Results.Created($"/api/customers/{item.Id}", item);
        }).RequireAuthorization(p => p.RequireRole("SystemAdministrator", "CompanyAdministrator", "ServiceManager"));
        group.MapPut("/{id:guid}", async (Guid id, CustomerRequest request, AtlasDbContext db, CancellationToken ct) =>
        {
            var item=await db.Customers.SingleOrDefaultAsync(x=>x.Id==id,ct); if(item is null)return Results.NotFound();
            item.CompanyId=request.CompanyId; item.Name=request.Name.Trim(); item.LegalName=request.LegalName; item.TaxNumber=request.TaxNumber; item.Email=request.Email; item.Phone=request.Phone; item.AddressLine1=request.AddressLine1; item.City=request.City; item.PostalCode=request.PostalCode; item.CountryCode=request.CountryCode.ToUpperInvariant(); item.IsActive=request.IsActive;
            await db.SaveChangesAsync(ct); return Results.Ok(item);
        }).RequireAuthorization(p => p.RequireRole("SystemAdministrator", "CompanyAdministrator", "ServiceManager"));
        group.MapDelete("/{id:guid}", async (Guid id, AtlasDbContext db, CancellationToken ct) =>
        {
            var item=await db.Customers.SingleOrDefaultAsync(x=>x.Id==id,ct); if(item is null)return Results.NotFound(); db.Customers.Remove(item); await db.SaveChangesAsync(ct); return Results.NoContent();
        }).RequireAuthorization(p => p.RequireRole("SystemAdministrator", "CompanyAdministrator"));
        return app;
    }
}
