using AncestorTable.Models;

namespace AncestorTable.Services;

internal class FieldMapperService
{
    public IEnumerable<string?[]> Cells(IEnumerable<Ancestor> ancestors) => new[] { HeaderValues() }.Concat(ancestors.Select(RowValues));

    private static string?[] HeaderValues() =>
        new[]
        {
            nameof(Ancestor.PersonId),
            nameof(Ancestor.Dupes),
            nameof(Ancestor.AhnentafelNumber),
            nameof(Ancestor.GenerationNumber),
            nameof(Ancestor.Sex),
            nameof(Ancestor.ProgenitorStatus),
            nameof(Ancestor.FirstName),
            nameof(Ancestor.Surname),
            nameof(Ancestor.Century),
            nameof(Ancestor.Country),
            nameof(Ancestor.Year),
            nameof(Ancestor.NationalFlag),
            nameof(Ancestor.OriginalCountry),
            nameof(Ancestor.Continent),
            nameof(Ancestor.AncestryPercent)
        };

    private static string?[] RowValues(Ancestor ancestor) =>
        new[]
        {
            $"{ancestor.PersonId}",
            $"{ancestor.Dupes}",
            $"{ancestor.AhnentafelNumber}",
            $"{ancestor.GenerationNumber}",
            ancestor.Sex,
            ancestor.ProgenitorStatus,
            ancestor.FirstName,
            ancestor.Surname,
            ancestor.Century,
            ancestor.Country,
            ancestor.Year,
            ancestor.NationalFlag,
            ancestor.OriginalCountry,
            ancestor.Continent,
            $"{ancestor.AncestryPercent}",
        };
}
