using Atlas.Domain.Common;

namespace Atlas.Domain.Companies;

public sealed class Company : BaseEntity
{
    public string Name { get; private set; } = string.Empty;
    public string VatNumber { get; private set; } = string.Empty;
    public bool IsActive { get; private set; } = true;

    private Company() { }

    public Company(string name, string vatNumber)
    {
        Rename(name);
        ChangeVatNumber(vatNumber);
    }

    public void Rename(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Η επωνυμία είναι υποχρεωτική.", nameof(name));

        Name = name.Trim();
        MarkUpdated();
    }

    public void ChangeVatNumber(string vatNumber)
    {
        if (string.IsNullOrWhiteSpace(vatNumber))
            throw new ArgumentException("Το ΑΦΜ είναι υποχρεωτικό.", nameof(vatNumber));

        VatNumber = vatNumber.Trim();
        MarkUpdated();
    }

    public void Deactivate()
    {
        IsActive = false;
        MarkUpdated();
    }
}
