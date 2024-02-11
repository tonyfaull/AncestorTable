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
        const decimal averageDaysPerYear = 365.242199m;
        const string emDash = "—";

        CountryLookupService countryLookupService = new();
        DuplicationCounterService duplicationCounterService = new();
        GedcomReaderService gedcomReaderService = new();

        var closeRelatives = new string?[16];

        return TraverseAncestry(descendant);

        // Local functions
        IEnumerable<Ancestor> TraverseAncestry(
            Individu? person,
            string? lineage = null,
            int generationNumber = 1,
            int ahnentafelNumber = 1,
            string? countryOfChild = null,
            DateTime? dateChildBorn = null)
        {
            if (person != null)
                duplicationCounterService.IncrementCounter(person.Id);

            if (ahnentafelNumber < Math.Pow(2, 4))
                closeRelatives[ahnentafelNumber] = PersonName()?.Surname;

            if (IsLeaf())
                if (person == null)
                    yield return new Ancestor
                    {
                        AhnentafelNumber = ahnentafelNumber,
                        GenerationNumber = generationNumber,
                        GenerationsBack = generationNumber - 1,
                        Angle = Angle(ahnentafelNumber),

                        ParentNumber = Side(1),
                        GrandparentNumber = Side(2),
                        GreatGrandparentNumber = Side(3),
                        ParentName = closeRelatives[Side(1)],
                        GrandparentName = closeRelatives[Side(2)],
                        GreatGrandparentName = closeRelatives[Side(3)],

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
                        GenerationsBack = generationNumber - 1,
                        Angle = Angle(ahnentafelNumber),

                        ParentNumber = Side(1),
                        GrandparentNumber = Side(2),
                        GreatGrandparentNumber = Side(3),
                        ParentName = closeRelatives[Side(1)],
                        GrandparentName = closeRelatives[Side(2)],
                        GreatGrandparentName = closeRelatives[Side(3)],

                        DuplicationNumber = DuplicationNumber(),
                        PersonId = person.Id,
                        ProgenitorStatus = ProgenitorStatus(),
                        AncestryPercent = AncestryPercent(),
                        Sex = Sex(),

                        FirstName = PersonName()?.Given,
                        Surname = PersonName()?.Surname,
                        AgeChildBorn = Age(DateBorn(person), dateChildBorn),
                        AgeDied = Age(DateBorn(person), DateDied()),

                        ContinentBorn = ContinentBorn(),
                        NationalFlagBorn = Country(PlaceBorn(person))?.NationalFlag,
                        CountryBorn = Country(PlaceBorn(person)),

                        CenturyBorn = Century(DateBorn(person)),
                        DateBorn = DateBorn(person),
                        DateChildBorn = dateChildBorn,
                        DateDied = DateDied(),
                        YearDescendantBorn = DateBorn(descendant)?.Date.Year,
                        Lineage = ProgenitorLineage(),
                    };

            if (generationNumber >= maxGenerations)
                yield break;

            if (!IsLeaf())
                foreach (var ancestor in TraverseAncestry(Father(), Lineage(), generationNumber + 1,
                                 ahnentafelNumber * 2, CountryBorn(), DateBorn(person))
                             .Union(TraverseAncestry(Mother(), Lineage(), generationNumber + 1,
                                 ahnentafelNumber * 2 + 1, CountryBorn(), DateBorn(person)))
                             .OrderBy(ancestor => ancestor.AhnentafelNumber))
                    yield return ancestor;

            yield break;

            // Local functions
            decimal AncestryPercent() => (decimal)(1 / Math.Pow(2, generationNumber - 1));

            string? CountryBorn() => Country(PlaceBorn(person))?.Name ?? countryOfChild;

            string? PlaceBorn(Individu? individual) => gedcomReaderService.PlaceBorn(individual);

            DateTime? DateBorn(Individu? individual) => gedcomReaderService.DateBorn(individual) ?? DateBornDefault();

            DateTime? DateDied() => gedcomReaderService.DateDied(person);

            decimal? Years(TimeSpan? timeSpan) => timeSpan is not null && timeSpan.Value != TimeSpan.Zero
                ? Math.Round((decimal)timeSpan.Value.TotalDays / averageDaysPerYear, 2)
                : null;

            static TimeSpan? Duration(DateTime? from, DateTime? to) =>
                from is null || to is null ? null : to.Value - from.Value;

            decimal? Age(DateTime? from, DateTime? to) => Years(Duration(from, to));

            string? Century(DateTime? date) => date is not null ? $"{date.Value.Year / 100}00s" : null;

            string? ProgenitorLineage() => $"{Lineage()} of {CountryBorn()}";

            string? Lineage() => $"{lineage}{Separator()}{InitialsSurname()}";

            string? Separator() => lineage is not null ? emDash : "";

            string? InitialsSurname() => IsLiving() ? RelationshipToDescendant() : $"{Initials()} {Surname()}";

            string Initials() => InitialsRegex().Replace(Given() ?? "", "$1");

            string? Given() => person?.Names.FirstOrDefault()?.Given;

            bool IsLiving() => DateBorn(person) is not null &&
                               DateBorn(person)?.Year > DateTime.Today.Year - privacyYears;

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

            bool OfUnknownParentage() => Father() is null && Mother() is null;

            Country? Country(string? location) => countryLookupService.Country(location);

            string? ContinentBorn() => Country(PlaceBorn(person))?.Continent?.Name;

            string? ContinentBornChild() => Country(countryOfChild)?.Continent?.Name;

            DateTime? DateBornDefault() => dateChildBorn is not null
                ? new DateTime(dateChildBorn.Value.Year - averageMaternalAge, 1, 1, 0, 0, 0, DateTimeKind.Unspecified)
                : null;

            static double GenerationsBack(int ahnentafelNumber) => (int)Math.Floor(Math.Log2(ahnentafelNumber));

            static double Angle(int ahnentafelNumber) => (ahnentafelNumber - (int)Math.Pow(2, GenerationsBack(ahnentafelNumber))) / Math.Pow(2, GenerationsBack(ahnentafelNumber));

            int Side(int generation) => (int)Math.Pow(2, generation) + (int)Math.Floor(Angle(ahnentafelNumber) * (int)Math.Pow(2, generation));

            string RelationshipToDescendant() => ahnentafelNumber switch
            {
                1 => "Living",
                2 => "Father",
                3 => "Mother",
                4 or 6 => "Grandfather",
                5 or 7 => "Grandmother",
                _ => "Private"
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