using AncestorTable.Models;

namespace AncestorTable.Services;

internal static class FileName
{
    public static string Countries { get; } = @"StaticData\countries.csv";
    public static string CountryAliases { get; } = @"StaticData\country-aliases.csv";
}

internal class GeographyService
{
    private readonly CsvReaderService _csvReaderService = new();

    private readonly Continent[] Continents =
    {
        new() { Code = "AF", Name = "Africa" },
        new() { Code = "EU", Name = "Europe" },
        new() { Code = "SA", Name = "South America" },
        new() { Code = "NA", Name = "North America" },
        new() { Code = "OC", Name = "Oceania" },
        new() { Code = "AN", Name = "Antarctica" },
        new() { Code = "AS", Name = "Asia" },
    };

    public List<Country> Countries { get; }

    public CountryAlias[] CountryAliases { get; }

    public GeographyService()
    {
        Countries = LoadCountries();
        CountryAliases = LoadCountryAliases();
        return;

        // Local functions
        List<Country> LoadCountries() => _csvReaderService.ReadCsv(FileName.Countries, 9).Select(Country).ToList();

        CountryAlias[] LoadCountryAliases() => _csvReaderService.ReadCsv(FileName.CountryAliases, 2).Select(CountryAlias).ToArray();

        Country Country(string[] values) => new()
        {
            Name = values[4],
            Continent = Array.Find(Continents, continent => continent.Code == values[8]),
            TwoLetterCode = values[0],
        };

        CountryAlias CountryAlias(string[] values) => new()
        {
            Alias = values[0],
            Country = Countries.Find(country => country.Name == values[1]),
            MatchType = values[2],
        };
    }
}