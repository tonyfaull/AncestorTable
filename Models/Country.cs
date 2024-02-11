namespace AncestorTable.Models;

public record Country
{
    public string Name { get; init; } = "";

    public string? TwoLetterCode { get; init; }

    public Continent? Continent { get; init; }

    public string? NationalFlag => TwoLetterCode is not null ? string.Concat(TwoLetterCode.Select(x => char.ConvertFromUtf32(x + 0x1F1A5))) : null;

    public override string ToString() => Name;
}