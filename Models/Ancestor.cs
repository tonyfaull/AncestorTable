namespace AncestorTable.Models;

internal record Ancestor
{
    public int AhnentafelNumber { get; init; }

    public int GenerationNumber { get; init; }

    public int? DuplicationNumber { get; init; }

    public decimal AncestryPercent { get; init; }

    public string? PersonId { get; init; }

    public string? ProgenitorStatus { get; init; }

    public string? CenturyBorn { get; init; }

    public DateTime? DateBorn { get; init; }

    public DateTime? DateDied { get; init; }

    public decimal? AgeDied { get; init; }

    public DateTime? DateChildBorn { get; init; }

    public decimal? AgeChildBorn { get; init; }

    public Country? CountryChildBorn { get; init; }

    public string? PlaceBorn { get; init; }

    public string? PlaceChildBorn { get; init; }

    public string? PlaceDied { get; init; }

    public Country? CountryDied { get; init; }

    public Country? CountryBorn { get; init; }

    public string? NationalFlagBorn { get; init; }

    public string? ContinentBorn { get; init; }

    public string? Sex { get; init; }

    public string? FirstName { get; init; }

    public string? Surname { get; init; }

    public string? SurnameDescendant { get; init; }

    public DateTime? DateDescendantBorn { get; init; }

    public string? PlaceDescendantBorn { get; init; }

    public Country? CountryDescendantBorn { get; init; }

    public string? Lineage { get; init; }
}