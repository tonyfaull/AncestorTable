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
        nameof(Ancestor.FamilySearchId),
        nameof(Ancestor.ProgenitorStatus),
        nameof(Ancestor.AncestryPercent),
        nameof(Ancestor.Sex),
        nameof(Ancestor.FirstName),
        nameof(Ancestor.Surname),
        nameof(Ancestor.AgeChildBorn),
        nameof(Ancestor.AgeDied),
        nameof(Ancestor.ContinentBorn),
        nameof(Ancestor.NationalFlagBorn),
        nameof(Ancestor.CountryBorn),
        nameof(Ancestor.CenturyBorn),
        nameof(Ancestor.DateBorn),
        nameof(Ancestor.DateChildBorn),
        nameof(Ancestor.DateDied),
        nameof(Ancestor.YearDescendantBorn),
        nameof(Ancestor.ParentNumber),
        nameof(Ancestor.ParentName),
        nameof(Ancestor.GrandparentNumber),
        nameof(Ancestor.GrandparentName),
        nameof(Ancestor.GreatGrandparentNumber),
        nameof(Ancestor.GreatGrandparentName),
        nameof(Ancestor.Angle),
        nameof(Ancestor.Lineage),
    };

    private static string?[] RowValues(Ancestor ancestor) => new[]
    {
        $"{ancestor.AhnentafelNumber}",
        $"{ancestor.GenerationNumber}",
        $"{ancestor.DuplicationNumber}",
        $"{ancestor.PersonId}",
        ancestor.FamilySearchId,
        ancestor.ProgenitorStatus,
        $"{ancestor.AncestryPercent}",
        ancestor.Sex,
        ancestor.FirstName,
        ancestor.Surname,
        $"{ancestor.AgeChildBorn}",
        $"{ancestor.AgeDied}",
        ancestor.ContinentBorn,
        ancestor.NationalFlagBorn,
        $"{ancestor.CountryBorn}",
        ancestor.CenturyBorn,
        DateString(ancestor.DateBorn),
        DateString(ancestor.DateChildBorn),
        DateString(ancestor.DateDied),
        $"{ancestor.YearDescendantBorn}",
        $"{ancestor.ParentNumber}",
        $"{ancestor.ParentName}",
        $"{ancestor.GrandparentNumber}",
        $"{ancestor.GrandparentName}",
        $"{ancestor.GreatGrandparentNumber}",
        $"{ancestor.GreatGrandparentName}",
        $"{ancestor.Angle}",
        ancestor.Lineage
    };

    static string? DateString(DateTime? date)
    {
        if (date is null)
            return null;
        if (date.Value.DayOfYear == 1)
            return $"{date:yyyy}";
        return $"{date:yyyy-MM-dd}";
    }
}
