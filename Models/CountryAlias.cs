namespace AncestorTable.Models;

internal record CountryAlias
{
    public string? Alias { get; init; }

    public Country? Country { get; init; }
}