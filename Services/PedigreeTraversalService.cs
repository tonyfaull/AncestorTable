using System.Text.RegularExpressions;
using AncestorTable.Models;
using NanoGedcom;

namespace AncestorTable.Services;

internal partial class PedigreeTraversalService
{
    public IEnumerable<Ancestor> Progenitors(Individu descendant)
    {
        const int maxGenerations = 16;
        const int averageMaternalAge = 30;
        const int privacyYears = 100;
        const string emDash = "—";
        const string ellipsis = "…";
        const int maxLineageCharacters = 1000;

        CountryLookupService countryLookupService = new();
        DuplicationCounterService duplicationCounterService = new();
        GedcomReaderService gedcomReaderService = new();

        return TraverseAncestry(descendant);

        // Local functions
        IEnumerable<Ancestor> TraverseAncestry(Individu? person, string? lineage = null, Individu? child = null,
            int generationNumber = 1, int ahnentafelNumber = 1, string? countryOfChild = null)
        {
            if (person != null)
                duplicationCounterService.IncrementCounter(person.Id);

            if (IsLeaf())
            {
                if (person == null)
                {
                    yield return new Ancestor
                    {
                        AhnentafelNumber = ahnentafelNumber,
                        GenerationNumber = generationNumber,
                        AncestryPercent = AncestryPercent(),
                        Sex = Sex(),

                        ContinentBorn = ContinentBornChild(),
                        CountryBorn = Country(countryOfChild),
                        DateBorn = DateBornDefault(),
                        CenturyBorn = Century(DateBornDefault()),
                        ProgenitorStatus = "Of unknown parentage",

                        SurnameDescendant = descendant?.Names[0].Surname,
                        DateDescendantBorn = DateBorn(descendant),
                        PlaceDescendantBorn = PlaceBorn(descendant),
                        CountryDescendantBorn = Country(PlaceBorn(descendant)),

                        Lineage = ProgenitorLineage(),
                    };
                }
                else
                {
                    yield return new Ancestor
                    {
                        AhnentafelNumber = ahnentafelNumber,
                        GenerationNumber = generationNumber,
                        DuplicationNumber = DuplicationNumber(),
                        PersonId = person.Id,
                        AncestryPercent = AncestryPercent(),
                        Sex = Sex(),

                        ProgenitorStatus = ProgenitorStatus(),
                        FirstName = PersonName()?.Given,
                        Surname = PersonName()?.Surname,

                        DateBorn = DateBorn(person),
                        PlaceBorn = PlaceBorn(person),
                        CountryBorn = Country(PlaceBorn(person)),
                        NationalFlagBorn = Country(PlaceBorn(person))?.NationalFlag,
                        ContinentBorn = ContinentBorn(),
                        CenturyBorn = Century(DateBorn(person)),

                        DateDied = DateDied(),
                        AgeDied = Age(DateBorn(person), DateDied()),
                        PlaceDied = PlaceDied(),
                        CountryDied = Country(PlaceDied()),

                        AgeChildBorn = Age(DateBorn(person), DateBorn(child)),
                        DateChildBorn = DateBorn(child),
                        PlaceChildBorn = PlaceBorn(child),
                        CountryChildBorn = Country(PlaceBorn(child)),

                        SurnameDescendant = descendant?.Names[0].Surname,
                        DateDescendantBorn = DateBorn(descendant),
                        PlaceDescendantBorn = PlaceBorn(descendant),
                        CountryDescendantBorn = Country(PlaceBorn(descendant)),

                        Lineage = ProgenitorLineage(),
                    };
                }
            }

            if (generationNumber >= maxGenerations)
                yield break;

            if (!IsLeaf())
            {
                foreach (var ancestor in TraverseAncestry(Father(), Lineage(), person, generationNumber + 1,
                             ahnentafelNumber * 2, CountryBorn()).OrderBy(ancestor => ancestor.AhnentafelNumber))
                    yield return ancestor;

                foreach (var ancestor in TraverseAncestry(Mother(), Lineage(), person, generationNumber + 1,
                             ahnentafelNumber * 2 + 1, CountryBorn()).OrderBy(ancestor => ancestor.AhnentafelNumber))
                    yield return ancestor;
            }

            yield break;

            // Local functions
            decimal AncestryPercent() => (decimal)(1 / Math.Pow(2, generationNumber - 1));

            string? CountryBorn() => Country(PlaceBorn(person))?.Name ?? countryOfChild;

            string? PlaceBorn(Individu? individual) => gedcomReaderService.PlaceBorn(individual);

            string? PlaceDied() => gedcomReaderService.PlaceDied(person);

            DateTime? DateBorn(Individu? individual) => gedcomReaderService.DateBorn(individual) ?? gedcomReaderService.DateBorn(child)?.AddYears(-averageMaternalAge);

            DateTime? DateDied() => gedcomReaderService.DateDied(person);

            decimal? Years(TimeSpan? timeSpan) => timeSpan is not null && timeSpan.Value != TimeSpan.Zero ? Math.Round((decimal)timeSpan.Value.TotalDays / 365.28m, 4) : null;

            static TimeSpan? Duration(DateTime? from, DateTime? to) => from is null || to is null ? null : to.Value - from.Value;

            decimal? Age(DateTime? from, DateTime? to) => Years(Duration(from, to));

            string? Century(DateTime? date) => date is not null ? $"{date.Value.Year / 100}00s" : null;

            string? Truncate(string? value, int maxLength) => value?.Length > maxLength ? $"{ellipsis}{value[(value.Length - maxLength - 1)..]}" : value;

            string? ProgenitorLineage() => $"{Truncate(Lineage(), maxLineageCharacters)} of {CountryBorn()}";

            string? Lineage() => $"{lineage}{Separator()}{ShortName()}";

            string? Separator() => lineage is not null ? emDash : "";

            string? ShortName() => IsLiving() ? "Living" : $"{Initials()}.{Surname()}";

            string Initials() => InitialsRegex().Replace(Given() ?? "", "$1");

            string? Given() => person?.Names.FirstOrDefault()?.Given;

            bool IsLiving() => DateBorn(person) is not null && DateBorn(person)?.Year > DateTime.Today.Year - privacyYears;

            string? Surname() => person?.Names.FirstOrDefault()?.Surname;

            IndividualName? PersonName() => person?.Names.FirstOrDefault();

            string Sex() => ahnentafelNumber % 2 == 0 ? "Male" : "Female";

            bool IsProgenitor() => generationNumber > 1 && CountryBorn() != countryOfChild;

            bool IsLeaf() => IsProgenitor() || generationNumber == maxGenerations || OfUnknownParentage();

            string PersonId() => person.Id ?? "";

            int DuplicationNumber() => duplicationCounterService.Counter[PersonId()];

            Family? Family() => person?.ChildInFamilies?.FirstOrDefault();

            Individu? Father() => generationNumber < maxGenerations ? Family()?.Husband : null;

            Individu? Mother() => generationNumber < maxGenerations ? Family()?.Wife : null;

            bool OfUnknownParentage() => Father() == null && Mother() == null;

            Country? Country(string? location) => countryLookupService.Country(location);

            string? ContinentBorn() => Country(PlaceBorn(person))?.Continent?.Name;

            string? ContinentBornChild() => Country(countryOfChild)?.Continent?.Name;

            DateTime? DateBornDefault() => DateBorn(child)?.AddYears(-averageMaternalAge);

            string ProgenitorStatus()
            {
                if (IsProgenitor())
                    return "Progenitor";
                if (generationNumber == maxGenerations)
                    return "Generation limit reached";
                if (OfUnknownParentage())
                    return "Of unknown parentage";
                return "";
            }
        }
    }

    [GeneratedRegex("(\\b[a-zA-Z])[a-zA-Z]* ?")]
    private static partial Regex InitialsRegex();
}