namespace AncestorTable.Models;

internal record Ancestor
{
    public int AhnentafelNumber { get; init; }

    public int DuplicationNumber { get; init; }

    public string? PersonId { get; init; }

    public int GenerationNumber { get; init; }

    public string? Year { get; init; }

    public string? Sex { get; init; }

    public string? ProgenitorStatus { get; init; }

    public string? FirstName { get; init; }

    public string? Surname { get; init; }

    public string? Century { get; init; }

    public string? Country { get; init; }

    public string? NationalFlag { get; set; }

    public string? Continent { get; init; }

    public string? EndYear { get; init; }

    public double AncestryPercent { get; init; }
}