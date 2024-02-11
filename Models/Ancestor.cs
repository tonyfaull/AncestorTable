namespace AncestorTable.Models;

public record Ancestor
{
    public int AhnentafelNumber { get; init; }

    public int GenerationNumber { get; init; }

    public int? DuplicationNumber { get; init; }

    public string? PersonId { get; init; }

    public string? ProgenitorStatus { get; init; }

    public decimal AncestryPercent { get; init; }

    public string? Sex { get; init; }

    public string? FirstName { get; init; }

    public string? Surname { get; init; }

    public decimal? AgeChildBorn { get; init; }

    public decimal? AgeDied { get; init; }

    public Country? CountryBorn { get; init; }

    public string? NationalFlagBorn { get; init; }

    public string? ContinentBorn { get; init; }

    public string? CenturyBorn { get; init; }

    public DateTime? DateBorn { get; init; }

    public DateTime? DateChildBorn { get; init; }

    public DateTime? DateDied { get; init; }

    public int? YearDescendantBorn { get; init; }

    public int GenerationsBack { get; init; }

    public double Angle { get; init; }

    public string? ParentName{ get; init; }

    public string? GrandparentName { get; init; }

    public string? GreatGrandparentName { get; init; }

    public int? ParentNumber { get; init; }

    public int? GrandparentNumber { get; init; }

    public int? GreatGrandparentNumber { get; init; }

    public string? Lineage { get; init; }

    public string? FamilySearchId { get; set; }
}
