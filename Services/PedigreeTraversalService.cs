using AncestorTable.Models;
using NanoGedcom;

namespace AncestorTable.Services;

internal class PedigreeTraversalService
{
    public IEnumerable<Ancestor> Progenitors(Individu descendant)
    {
        const int maxGenerations = 25;

        CountryLookupService countryLookupService = new();
        DuplicationCounterService duplicationCounterService = new();
        GedcomReaderService gedcomReaderService = new();

        return TraverseAncestry(descendant);

        // Local functions
        IEnumerable<Ancestor> TraverseAncestry(Individu? person, Individu? child = null, int generationNumber = 1, int ahnentafelNumber = 1, string? countryOfChild = null)
        {
            if (person != null)
            {
                duplicationCounterService.IncrementCounter(person.Id);
            }

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

                        SurnameDescendant = descendant?.Names[0].Surname,
                        DateDescendantBorn = DateBorn(descendant),
                        PlaceDescendantBorn = PlaceBorn(descendant),
                        CountryDescendantBorn = Country(PlaceBorn(descendant)),
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
                        ContinentBorn = Country(PlaceBorn(person))?.Continent?.Name,
                        CenturyBorn = Century(DateBorn(person)),
                        
                        DateDied =  DateDied(),
                        AgeDied = Age(DateBorn(person), DateDied()),
                        PlaceDied = PlaceDied(),
                        CountryDied = Country(PlaceDied()),
                        
                        AgeChildBorn = Age(DateBorn(person), DateBorn(person)),
                        DateChildBorn = DateBorn(child),
                        PlaceChildBorn = PlaceBorn(child),
                        CountryChildBorn = Country(PlaceBorn(child)),

                        SurnameDescendant = descendant?.Names[0].Surname,
                        DateDescendantBorn = DateBorn(descendant),
                        PlaceDescendantBorn = PlaceBorn(descendant),
                        CountryDescendantBorn = Country(PlaceBorn(descendant)),
                    };
                }
            }

            if (generationNumber >= maxGenerations)
                yield break;

            if (!IsLeaf())
            {
                var countryBorn = CountryBorn();
                foreach (var ancestor in TraverseAncestry(Father(), person, generationNumber + 1, ahnentafelNumber * 2, countryBorn?.Name).OrderBy(ancestor => ancestor.AhnentafelNumber))
                {
                    yield return ancestor;
                }

                foreach (var ancestor in TraverseAncestry(Mother(), person, generationNumber + 1, ahnentafelNumber * 2 + 1, countryBorn?.Name).OrderBy(ancestor => ancestor.AhnentafelNumber))
                {
                    yield return ancestor;
                }
            }

            yield break;

            // Local functions
            double AncestryPercent() => 1 / Math.Pow(2, generationNumber - 1);

            Country? CountryBorn() => Country(PlaceBorn(person));

            string? PlaceBorn(Individu? individual) => gedcomReaderService.PlaceBorn(individual);

            string? PlaceDied() => gedcomReaderService.PlaceDied(person);

            DateTime? DateBorn(Individu? individual) => gedcomReaderService.DateBorn(individual) ?? gedcomReaderService.DateBorn(child)?.AddYears(-30);

            DateTime? DateDied() => gedcomReaderService.DateDied(person);

            double? Years(TimeSpan? timeSpan) => timeSpan?.TotalDays / 365.28;

            static TimeSpan? TimeSpan(DateTime? from, DateTime? to) => from is null || to is null ? null : to.Value - from.Value;

            double? Age(DateTime? from, DateTime? to) => Years(TimeSpan(from, to));

            string? Century(DateTime? date) => date is not null ? $"{(date.Value.Year / 100)}00s" : null;

            IndividualName? PersonName() => person.Names.FirstOrDefault();

            string Sex() => ahnentafelNumber % 2 == 0 ? "Male" : "Female";

            bool IsProgenitor() => generationNumber > 1 && countryOfChild != null && countryOfChild != Country(PlaceBorn(child))?.Name;

            bool IsLeaf() => IsProgenitor() || generationNumber == maxGenerations || OfUnknownParentage();

            string PersonId() => person.Id ?? "";

            int DuplicationNumber() => duplicationCounterService.Counter[PersonId()];

            Family? Family() => person?.ChildInFamilies?.FirstOrDefault();

            Individu? Father() => generationNumber < maxGenerations ? Family()?.Husband : null;

            Individu? Mother() => generationNumber < maxGenerations ? Family()?.Wife : null;

            bool OfUnknownParentage() => Father() == null && Mother() == null;

            Country? Country(string? location) => countryLookupService.Country(location);

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
}