using System.Text.RegularExpressions;
using AncestorTable.Models;

namespace AncestorTable.Services;

internal class CountryLookupService
{
    private readonly GeographyService _geographyService = new();

    private string OriginalCountryName(string? location) => location is null or "" ? "" : Last(location);

    public Country? Country(string? location)
    {
        if (location is null or "")
        {
            return null;
        }

        var name = OriginalCountryName(location);

        ReplaceUkWithEnglandScotlandWales();
        ReplaceChannelIslandsWithJerseyGuernsey();

        var exactCountry = _geographyService.Countries.FirstOrDefault(country => IsCanonical(country) && name == country.Name);
        if (exactCountry != null)
        {
            return exactCountry;
        }

        var substringCountry = _geographyService.Countries.FirstOrDefault(country => IsCanonical(country) && name.Contains(country.Name, StringComparison.Ordinal));
        if (substringCountry != null)
        {
            return substringCountry;
        }

        var exactAlias = _geographyService.CountryAliases.FirstOrDefault(countryAlias => countryAlias.Alias == name);
        if (exactAlias != null)
        {
            return exactAlias.Country;
        }

        var substringAlias = _geographyService.CountryAliases.FirstOrDefault(countryAlias => name.Contains(countryAlias.Alias!, StringComparison.Ordinal));
        if (substringAlias != null)
        {
            return substringAlias.Country;
        }

        // Regex match
        var aliasRegexCountry = _geographyService.CountryAliases.Where(IsRegexPattern).Where(IsMatch).Select(countryAlias => countryAlias.Country).FirstOrDefault();
        if (aliasRegexCountry != null)
        {
            return aliasRegexCountry;
        }

        // Unmatched
        var unmatchedCountry = new Country { Name = name };
        _geographyService.Countries.Add(unmatchedCountry);
        return unmatchedCountry;

        //Local functions
        void ReplaceUkWithEnglandScotlandWales()
        {
            if (Last(location).Equals("United Kingdom", StringComparison.InvariantCultureIgnoreCase) && SecondLast(location!) is not null)
            {
                name = SecondLast(location);
            }
        }

        void ReplaceChannelIslandsWithJerseyGuernsey()
        {
            if (Last(location).Equals("Channel Islands", StringComparison.InvariantCultureIgnoreCase) && SecondLast(location!) is not null)
            {
                name = SecondLast(location);
            }
        }

        bool IsRegexPattern(CountryAlias countryAlias) => countryAlias.Alias != null && new[] { '*', '+', '[' , '\\' }.Any(countryAlias.Alias.Contains);

        bool IsMatch(CountryAlias countryAlias) => Regex.IsMatch(name, countryAlias.Alias!, RegexOptions.IgnoreCase);

        bool IsCanonical(Country country) => country.TwoLetterCode != null;
    }

    string Last(string location) => Part(location, 0)!;

    string? SecondLast(string location) => Part(location, 1);

    private static string? Part(string location, int index) => location.Split(",").Reverse().Select(place => place?.Trim()).Skip(index).FirstOrDefault();
}
