using System.Globalization;
using System.Text.RegularExpressions;
using AncestorTable.Enums;
using NanoGedcom;

namespace AncestorTable.Services;

internal class GedcomReaderService
{
    public Gedcom LoadGedcomFile(string file) => GedcomParser.Load(file)!;

    public Individu Descendant(Gedcom gedcom, int? yearOfBirth = null) =>
        yearOfBirth is > 0
            ? gedcom.Individus.Find(person => DateBorn(person)?.Year == yearOfBirth) ??
              gedcom.Individus[0]
            : gedcom.Individus[0];

    public string? Place(Event? even) => even?.Location;

    public string? PlaceBorn(Individu? person) => Place(Birth(person) ?? Christening(person));

    public string? PlaceDied(Individu? person) => Place(Death(person) ?? Burial(person));

    public DateTime? DateBorn(Individu? person) => Date(Birth(person) ?? Christening(person));

    public DateTime? DateDied(Individu? person) => Date(Death(person) ?? Burial(person));

    private static Event? Birth(Individu? person) => person?.Events.Find(even => even.Type == EventType.Birth);

    private static Event? Christening(Individu? person) => person?.Events.Find(even => even.Type == EventType.Christening);

    private static Event? Death(Individu? person) => person?.Events.Find(even => even.Type == EventType.Death);

    private static Event? Burial(Individu? person) => person?.Events.Find(even => even.Type == EventType.Burial);

    private static DateTime? Date(Event? even)
    {
        var dateString = even?.Date;
        if (dateString is null or "")
            return null;

        dateString = new Regex("Unknown|Abt |Bef |Bef Abt |Aft |Aft Abt ").Replace(dateString, "");

        dateString = new Regex("(Bet ).*( and .*)").Replace(dateString, "");
        if (dateString.StartsWith("Bet "))
        {
            var match = Regex.Match(dateString, "Bet (.*) and .*");
            dateString = match.Groups[1].Value;
        }

        if (dateString is "")
            return null;

        if (DateTime.TryParse(dateString, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out var date))
            return date;

        if (DateTime.TryParse($"{dateString}-01-01", CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out date))
            return date;

        return null;
    }

    private static TimeSpan? Age(DateTime? from, DateTime? to) => from == null || to == null ? null : to.Value - from.Value;
}