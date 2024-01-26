using System.Text.RegularExpressions;
using AncestorTable.Models;

namespace AncestorTable.Services;

internal class CountryLookupService
{
    private readonly GeographyService _geographyService = new();

    public string OriginalCountryName(string? location)
    {
        if (location is null or "")
        {
            return "";
        }

        return Last(location);
    }

    public Country? Country(string? location)
    {
        if (location is null or "")
        {
            return null;
        }

        var name = OriginalCountryName(location);

        ReplaceUkWithEnglandScotlandWales();

        var exactMatch = _geographyService.Countries.FirstOrDefault(country => country.Name == name);
        if (exactMatch != null)
        {
            return exactMatch;
        }

        // TODO: Fix for The Netherlands
        var caseInsensitiveSubstringMatch = _geographyService.Countries.FirstOrDefault(country => name.Contains(country.Name, StringComparison.Ordinal));
        if (caseInsensitiveSubstringMatch != null)
        {
            return caseInsensitiveSubstringMatch;
        }

        var exactMatchAlias = _geographyService.Countries.FirstOrDefault(country => country.Name == name);
        if (exactMatchAlias != null)
        {
            return exactMatchAlias;
        }

        var caseInsensitiveSubstringMatchAlias = _geographyService.Countries.FirstOrDefault(country => name.Contains(country.Name, StringComparison.InvariantCultureIgnoreCase));
        if (caseInsensitiveSubstringMatchAlias != null)
        {
            return caseInsensitiveSubstringMatchAlias;
        }

        // Regex match
        var regexMatch = _geographyService.CountryAliases.Where(IsRegexPattern).Where(IsMatch).Select(countryAlias => countryAlias.Country).FirstOrDefault();
        if (regexMatch != null)
        {
            return regexMatch;
        }

        // Unmatched countries
        var newCountry = new Country { Name = name, OriginalName = name };
        _geographyService.Countries.Add(newCountry);
        return newCountry;

        void ReplaceUkWithEnglandScotlandWales()
        {
            if (Last(location).Equals("United Kingdom", StringComparison.InvariantCultureIgnoreCase) && SecondLast(location!) is not null)
            {
                name = SecondLast(location);
            }
        }

        bool IsRegexPattern(CountryAlias countryAlias) => countryAlias.Alias != null && new[] { '*', '+', '[' }.Any(countryAlias.Alias.Contains);

        bool IsMatch(CountryAlias countryAlias) => Regex.IsMatch(name, countryAlias.Alias!, RegexOptions.IgnoreCase);
    }

    string Last(string location) => Part(location, 0)!;

    string? SecondLast(string location) => Part(location, 1);

    private string? Part(string location, int index) => location.Split(",").Reverse().Select(place => place?.Trim()).Skip(index).FirstOrDefault();
}
