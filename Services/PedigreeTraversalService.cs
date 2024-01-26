using System.Text.RegularExpressions;
using AncestorTable.Models;
using NanoGedcom;

namespace AncestorTable.Services;

internal partial class PedigreeTraversalService
{
    public IEnumerable<Ancestor> Progenitors(Individu pointPerson, int maxGenerations = 17)
    {
        CountryLookupService countryLookupService = new();
        DuplicationCounterService duplicationCounterService = new();

        return TraverseAncestry(pointPerson);

        IEnumerable<Ancestor> TraverseAncestry(Individu? person, int generationNumber = 1, int ahnentafelNumber = 1, string? countryOfChild = null)
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
                        AncestryPercent = AncestryPercent()
                    };
                }
                else
                {
                    var country = Country();
                    var name = PersonName();
                    yield return new Ancestor
                    {
                        PersonId = person.Id,
                        DuplicationNumber = DuplicationNumber(),
                        AhnentafelNumber = ahnentafelNumber,
                        GenerationNumber = generationNumber,
                        Sex = Sex(),
                        ProgenitorStatus = ProgenitorStatus(),
                        FirstName = name?.Given,
                        Surname = name?.Surname,
                        Century = Century(),
                        Country = country?.Name,
                        OriginalCountry = OriginalCountry(),
                        Year = Year(),
                        NationalFlag = country?.NationalFlag,
                        Continent = country?.Continent?.Name,
                        AncestryPercent = AncestryPercent(),
                    };
                }
            }

            if (generationNumber >= maxGenerations)
                yield break;

            if (!IsLeaf())
            {
                var country = Country();
                foreach (var ancestor in TraverseAncestry(Father(), generationNumber + 1, ahnentafelNumber * 2, country?.Name))
                {
                    yield return ancestor;
                }

                foreach (var ancestor in TraverseAncestry(Mother(), generationNumber + 1, ahnentafelNumber * 2 + 1, country?.Name))
                {
                    yield return ancestor;
                }
            }

            // Local functions
            Event? Birth() => person?.Events?.Find(e => e.Type == "BIRT");

            Country? Country() => countryLookupService.Country(Birth()?.Location);

            string OriginalCountry() => countryLookupService.OriginalCountryName(Birth()?.Location);

            double AncestryPercent() => 1 / Math.Pow(2, generationNumber - 1);

            string? Year() => YearRegex().Match(Birth()?.Date ?? "").Captures.FirstOrDefault()?.Value;

            string? Century() => Year() is null or "" ? "" : $"{Year()![..2]}00s";

            IndividualName? PersonName() => person.Names.FirstOrDefault();

            string Sex() => ahnentafelNumber % 2 == 0 ? "Male" : "Female";

            bool IsProgenitor() => generationNumber > 1 && countryOfChild != null && countryOfChild != Country()?.Name;

            bool IsLeaf() => IsProgenitor() || generationNumber == maxGenerations || OfUnknownParentage();

            string PersonId() => person.Id ?? "";

            int DuplicationNumber() => duplicationCounterService.Counter[PersonId()];

            Family? Family() => person?.ChildInFamilies?.FirstOrDefault();

            Individu? Father() => generationNumber < maxGenerations ? Family()?.Husband : null;

            Individu? Mother() => generationNumber < maxGenerations ? Family()?.Wife : null;

            bool OfUnknownParentage() => Father() == null && Mother() == null;

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

    [GeneratedRegex(@"\b(\d{4})\b")]
    private static partial Regex YearRegex();
}