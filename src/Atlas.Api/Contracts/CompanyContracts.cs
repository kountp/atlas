namespace Atlas.Api.Contracts;

public sealed record CompanyRequest(string Name, string? LegalName, string? TaxNumber, string? Email, string? Phone, bool IsActive = true);
public sealed record CustomerRequest(Guid CompanyId, string Name, string? LegalName, string? TaxNumber, string? Email, string? Phone, string? AddressLine1, string? City, string? PostalCode, string CountryCode = "GR", bool IsActive = true);
