using AncestorTable.Models;

namespace AncestorTable.Services;

internal class FieldMapperService
{
    public IEnumerable<string?[]> Cells(IEnumerable<Ancestor> ancestors) => new[] { HeaderValues() }.Concat(ancestors.OrderBy(ancestor => ancestor.AhnentafelNumber).Select(RowValues));

    private static string?[] HeaderValues() => new[]
    {
        nameof(Ancestor.AhnentafelNumber),
        nameof(Ancestor.GenerationNumber),
        nameof(Ancestor.DuplicationNumber),
        nameof(Ancestor.PersonId),
        nameof(Ancestor.ProgenitorStatus),
        nameof(Ancestor.AncestryPercent),
        nameof(Ancestor.Sex),
        nameof(Ancestor.FirstName),
        nameof(Ancestor.Surname),
        nameof(Ancestor.DateBorn),
        nameof(Ancestor.CenturyBorn),
        nameof(Ancestor.CountryBorn),
        nameof(Ancestor.NationalFlagBorn),
        nameof(Ancestor.ContinentBorn),
        nameof(Ancestor.DateDied),
        nameof(Ancestor.AgeDied),
        nameof(Ancestor.CountryDied),
        nameof(Ancestor.DateChildBorn),
        nameof(Ancestor.AgeChildBorn),
        nameof(Ancestor.CountryChildBorn),
        nameof(Ancestor.DateDescendantBorn),
        nameof(Ancestor.CountryDescendantBorn),
        nameof(Ancestor.Lineage),
    };

    private static string?[] RowValues(Ancestor ancestor) => new[]
    {
        $"{ancestor.AhnentafelNumber}",
        $"{ancestor.GenerationNumber}",
        $"{ancestor.DuplicationNumber}",
        $"{ancestor.PersonId}",
        ancestor.ProgenitorStatus,
        $"{ancestor.AncestryPercent}",
        ancestor.Sex,
        ancestor.FirstName,
        ancestor.Surname,
        $"{ancestor.DateBorn:yyyy-MM-dd}",
        ancestor.CenturyBorn,
        $"{ancestor.CountryBorn}",
        ancestor.NationalFlagBorn,
        ancestor.ContinentBorn,
        $"{ancestor.DateDied:yyyy-MM-dd}",
        $"{ancestor.AgeDied}",
        $"{ancestor.CountryDied}",
        $"{ancestor.DateChildBorn:yyyy-MM-dd}",
        $"{ancestor.AgeChildBorn}",
        $"{ancestor.CountryChildBorn}",
        $"{ancestor.DateDescendantBorn:yyyy-MM-dd}",
        $"{ancestor.CountryDescendantBorn}",
        ancestor.Lineage,
    };
}
