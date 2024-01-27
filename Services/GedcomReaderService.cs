using System.Text.RegularExpressions;
using NanoGedcom;

namespace AncestorTable.Services;

internal partial class GedcomReaderService
{
    public Gedcom LoadGedcomFile(string file) => GedcomParser.Load(file)!;

    public Individu PointPerson(Gedcom gedcom, string? yearOfBirth = null) =>
        yearOfBirth is not null and not "" && YearRegex().IsMatch(yearOfBirth)
            ? gedcom.Individus.Find(person => YearOfBirth(person) == yearOfBirth) ??
              gedcom.Individus[0]
            : gedcom.Individus[0];

    public string? YearOfBirth(Individu person) => Year(Birth(person));

    private static string? Year(Event? birth) => birth is null ? null : YearRegex().Match(birth.Date ?? "").Captures[0].Value;

    private static Event? Birth(Individu person) => person.Events.Find(even => even.Type == "BIRT");

    [GeneratedRegex(@"\b([1-2][0-9]{3})\b")]
    private static partial Regex YearRegex();
}