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
        ////nameof(Ancestor.PlaceBorn),
        nameof(Ancestor.CountryBorn),
        nameof(Ancestor.NationalFlagBorn),
        nameof(Ancestor.ContinentBorn),

        nameof(Ancestor.DateDied),
        nameof(Ancestor.AgeDied),
        ////nameof(Ancestor.PlaceDied),
        nameof(Ancestor.CountryDied),

        nameof(Ancestor.DateChildBorn),
        nameof(Ancestor.AgeChildBorn),
        ////nameof(Ancestor.PlaceChildBorn),
        nameof(Ancestor.CountryChildBorn),
        ////nameof(Ancestor.SurnameDescendant),
        nameof(Ancestor.DateDescendantBorn),
        nameof(Ancestor.CountryDescendantBorn),
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
        $"{ancestor.DateBorn:d MMM yyyy}",
        ancestor.CenturyBorn,
        ////ancestor.PlaceBorn,
        $"{ancestor.CountryBorn}",
        ancestor.NationalFlagBorn,
        ancestor.ContinentBorn,
        $"{ancestor.DateDied:d MMM yyyy}",
        $"{ancestor.AgeDied}",
        ////ancestor.PlaceDied,
        $"{ancestor.CountryDied}",
        $"{ancestor.DateChildBorn:d MMM yyyy}",
        $"{ancestor.AgeChildBorn}",
        ////ancestor.PlaceChildBorn,
        $"{ancestor.CountryChildBorn}",
        ////ancestor.SurnameDescendant,
        $"{ancestor.DateDescendantBorn:d MMM yyyy}",
        $"{ancestor.CountryDescendantBorn}",
    };
}
