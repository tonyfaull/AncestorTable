using AncestorTable.Models;

namespace AncestorTable.Services;

internal class FieldMapperService
{
    public IEnumerable<string?[]> Cells(IEnumerable<Ancestor> ancestors) => new[] { HeaderValues() }.Concat(ancestors.OrderBy(ancestor => ancestor.AhnentafelNumber).Select(RowValues));

    private static string?[] HeaderValues() => new[]
    {
        nameof(Ancestor.AhnentafelNumber),
        nameof(Ancestor.GenerationNumber),
        nameof(Ancestor.EndYear),
        nameof(Ancestor.DuplicationNumber),
        nameof(Ancestor.PersonId),
        nameof(Ancestor.Year),
        nameof(Ancestor.Sex),
        nameof(Ancestor.ProgenitorStatus),
        nameof(Ancestor.FirstName),
        nameof(Ancestor.Surname),
        nameof(Ancestor.Century),
        nameof(Ancestor.Country),
        nameof(Ancestor.NationalFlag),
        nameof(Ancestor.Continent),
        nameof(Ancestor.AncestryPercent)
    };

    private static string?[] RowValues(Ancestor ancestor) => new[]
    {
        $"{ancestor.AhnentafelNumber}",
        $"{ancestor.GenerationNumber}",
        ancestor.EndYear,
        $"{ancestor.DuplicationNumber}",
        $"{ancestor.PersonId}",
        ancestor.Year,
        ancestor.Sex,
        ancestor.ProgenitorStatus,
        ancestor.FirstName,
        ancestor.Surname,
        ancestor.Century,
        ancestor.Country,
        ancestor.NationalFlag,
        ancestor.Continent,
        $"{ancestor.AncestryPercent}",
    };
}
