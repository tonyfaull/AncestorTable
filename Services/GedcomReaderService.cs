using System.Globalization;
using System.Text.RegularExpressions;
using AncestorTable.Enums;
using NanoGedcom;

namespace AncestorTable.Services;

internal partial class GedcomReaderService
{
    public Gedcom LoadGedcomFile(string file) => GedcomParser.Load(file)!;

    public Individu Descendant(Gedcom gedcom, Dictionary<string, string> familySearchIds, string? ancestorId = null)
    {
        if (ancestorId is null or "")
        {
            return YoungestPerson();
        }

        if (YearPattern().IsMatch(ancestorId))
        {
            return gedcom.Individus.Find(person => DateBorn(person)?.Year == Convert.ToInt32(ancestorId)) ?? YoungestPerson();
        }

        if (FamilySearchIdPattern().IsMatch(ancestorId))
        {
            var personId = familySearchIds.ContainsValue(ancestorId)
                ? familySearchIds.Where(pair => pair.Value == ancestorId)
                    .Select(pair => pair.Key)
                    .FirstOrDefault()
                : null;
            return gedcom.Individus.Find(person => person.Id == personId) ?? YoungestPerson();
        }

        if (FullNamePattern().IsMatch(ancestorId))
        {
            return gedcom.Individus.Find(person => person.Names[0].Given + " " + person.Names[0] == ancestorId) ?? YoungestPerson();
        }

        if (!FamilySearchIdPattern().IsMatch(ancestorId))
        {
            return gedcom.Individus.Find(person => person.Id == ancestorId) ?? YoungestPerson();
        }

        return YoungestPerson();

        Individu YoungestPerson() => gedcom.Individus.MaxBy(DateBorn) ?? gedcom.Individus[0];
    }

    public string? Place(Event? even) => even?.Location;

    public string? PlaceBorn(Individu? person) => Place(Birth(person) ?? Christening(person));

    public DateTime? DateBorn(Individu? person) => Date(Birth(person) ?? Christening(person));

    public DateTime? DateDied(Individu? person) => Date(Death(person) ?? Burial(person));

    private static Event? Birth(Individu? person) => person?.Events.Find(even => even.Type == EventType.Birth);

    private static Event? Christening(Individu? person) =>
        person?.Events.Find(even => even.Type == EventType.Christening);

    private static Event? Death(Individu? person) => person?.Events.Find(even => even.Type == EventType.Death);

    private static Event? Burial(Individu? person) => person?.Events.Find(even => even.Type == EventType.Burial);

    private static DateTime? Date(Event? even)
    {
        var dateString = even?.Date;
        if (dateString is null or "")
            return null;

        if (YearPattern().IsMatch(dateString))
        {
            var yearMatch = YearPattern().Match(dateString);
            var year = Convert.ToInt32(yearMatch.Groups[0].Value);
            return new DateTime(year, 1, 1,0,0,0, DateTimeKind.Unspecified);
        }

        dateString = DateUncertaintyPattern().Replace(dateString, "");

        dateString = DateRangePattern().Replace(dateString, "");
        if (DateBetweenPattern().IsMatch(dateString))
        {
            var match = DateBetweenPattern().Match(dateString);
            dateString = match.Groups[1].Value;
        }

        if (DateTime.TryParse(dateString, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out var date))
            return date;

        if (DateTime.TryParse($"{dateString}-01-01", CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out date))
            return date;

        return null;
    }

    [GeneratedRegex(@"\b[12]\d{3}\b")]
    private static partial Regex YearPattern();

    [GeneratedRegex(@"[\w\d]{4}-[\w\d]{3}")]
    private static partial Regex FamilySearchIdPattern();

    [GeneratedRegex(".+ .+")]
    private static partial Regex FullNamePattern();

    [GeneratedRegex("Unknown|Abt |Bef |Bef Abt |Aft |Aft Abt ")]
    private static partial Regex DateUncertaintyPattern();

    [GeneratedRegex("(Bet ).*( and .*)")]
    private static partial Regex DateRangePattern();

    [GeneratedRegex("Bet (.*) and .*")]
    private static partial Regex DateBetweenPattern();
}