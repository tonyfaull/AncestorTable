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
        const int maxLineageCharacters = 1000;
        const decimal averageDaysPerYear = 365.242199m;
        const string emDash = "—";
        const string ellipsis = "…";

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
                if (person == null)
                    yield return new Ancestor
                    {
                        AhnentafelNumber = ahnentafelNumber,
                        GenerationNumber = generationNumber,
                        AncestryPercent = AncestryPercent(),
                        ProgenitorStatus = $"Unknown {Gender()}",
                        Sex = Sex(),

                        ContinentBorn = ContinentBornChild(),
                        NationalFlagBorn = Country(PlaceBorn(person))?.NationalFlag,
                        CountryBorn = Country(countryOfChild),

                        CenturyBorn = Century(DateBornDefault()),
                        DateBorn = DateBornDefault(),

                        YearDescendantBorn = DateBorn(descendant)?.Year,

                        Lineage = ProgenitorLineage(),
                    };
                else
                    yield return new Ancestor
                    {
                        AhnentafelNumber = ahnentafelNumber,
                        GenerationNumber = generationNumber,
                        DuplicationNumber = DuplicationNumber(),
                        PersonId = person.Id,
                        ProgenitorStatus = ProgenitorStatus(),
                        AncestryPercent = AncestryPercent(),
                        Sex = Sex(),

                        FirstName = PersonName()?.Given,
                        Surname = PersonName()?.Surname,
                        AgeChildBorn = Age(DateBorn(person), DateBorn(child)),
                        AgeDied = Age(DateBorn(person), DateDied()),

                        ContinentBorn = ContinentBorn(),
                        NationalFlagBorn = Country(PlaceBorn(person))?.NationalFlag,
                        CountryBorn = Country(PlaceBorn(person)),

                        CenturyBorn = Century(DateBorn(person)),
                        DateBorn = DateBorn(person),
                        DateChildBorn = DateBorn(child),
                        DateDied = DateDied(),
                        YearDescendantBorn = DateBorn(descendant)?.Date.Year,

                        Lineage = ProgenitorLineage(),
                    };

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

            DateTime? DateBorn(Individu? individual) => gedcomReaderService.DateBorn(individual) ?? gedcomReaderService.DateBorn(child)?.AddYears(-averageMaternalAge);

            DateTime? DateDied() => gedcomReaderService.DateDied(person);

            decimal? Years(TimeSpan? timeSpan) => timeSpan is not null && timeSpan.Value != TimeSpan.Zero ? Math.Round((decimal)timeSpan.Value.TotalDays / averageDaysPerYear, 2) : null;

            static TimeSpan? Duration(DateTime? from, DateTime? to) => from is null || to is null ? null : to.Value - from.Value;

            decimal? Age(DateTime? from, DateTime? to) => Years(Duration(from, to));

            string? Century(DateTime? date) => date is not null ? $"{date.Value.Year / 100}00s" : null;

            string? Truncate(string? value, int maxLength) => value?.Length > maxLength ? $"{ellipsis}{value[(value.Length - maxLength - 1)..]}" : value;

            string? ProgenitorLineage() => $"{Truncate(Lineage(), maxLineageCharacters)} of {CountryBorn()}";

            string? Lineage() => $"{lineage}{Separator()}{ShortName()}";

            string? Separator() => lineage is not null ? emDash : "";

            string? ShortName() => IsLiving() ? RelationshipToDescendant() : $"{Initials()}.{Surname()}";
            
            string Initials() => InitialsRegex().Replace(Given() ?? "", "$1");

            string? Given() => person?.Names.FirstOrDefault()?.Given;

            bool IsLiving() => DateBorn(person) is not null && DateBorn(person)?.Year > DateTime.Today.Year - privacyYears;

            string? Surname() => person?.Names.FirstOrDefault()?.Surname;

            IndividualName? PersonName() => person?.Names.FirstOrDefault();

            string Sex() => ahnentafelNumber % 2 == 0 ? "Male" : "Female";

            string Gender() => ahnentafelNumber % 2 == 0 ? "man" : "woman";

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

            string RelationshipToDescendant() => ahnentafelNumber switch
            {
                2 => "Father",
                3 => "Mother",
                4 => "Father",
                5 => "Mother",
                6 => "Father",
                7 => "Mother",
                _ => "Living"
            };

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