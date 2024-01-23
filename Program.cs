using System.Text;
using System.Text.RegularExpressions;
using NanoGedcom;

namespace AncestorTable;

internal class Program
{
    static void Main(string[] args)
    {
        const string file = @"G:\My Drive\Home\Family Tree\GEDCOM files\Etienne 16gen 2022-10-25.ged";
        const int maxGenerations = 17;
        string[] originalCountryNames;
        string[] standardizedCountryNames;
        const string countriesToReplace = @"[South Africa],South Africa
Afrique du Sud,South Africa
Bundesrepublik Deutschland,Germany
Cabo de Goede Hoop,South Africa
Cape of Good Hope,South Africa
Cape Province South Africa,South Africa
de Caep de Goede Hoop,South Africa
Germany),Germany
Deutschland,Germany
Deutschland (Germany),Germany
Dutch Cape Colony,South Africa
Dutch Cape Colony South Africa,South Africa
German Empire,Germany
Heiliges R”misches Reich,Germany
Holy Roman Empire,Germany
Indian peninsula,India
Kaapstad,South Africa
Leeuwarden,Netherlands
Nederland,Netherlands
Netherland,Netherlands
Netherlands (Present day Belgium),Belgium
Paarl,South Africa
Poitiers,France
Provence-Alpes-C,France
Prussia,Germany
Roggeveld,South Africa
Sambuwa or Indonesia,Indonesia
So Af,South Africa
So. Africa,South Africa
Soth Africa,South Africa
South Africa (or Fort Beauford),South Africa
South Africa (Rep.),South Africa
South Africa.,South Africa
South Africa],South Africa
Swartland district,South Africa
Weskus van Indi,India
ZA,South Africa
Amsterdam Netherlands,Netherlands
North Holland,Netherlands
Zuid-Afrika,South Africa
Preussen,Germany
Schweden,Sweden
Suid-Afrika,South Africa
Belguim.,Belgium
Caep de Goede Hoop,South Africa
Dutch Cape Colony [South Africa],South Africa
Dutch East Indies,Indonesia
Heiliges R,Germany
HH Germany,Germany
Holland,Netherlands
On board the ship,South Africa
Provence-Alpes-C,France
Sa,South Africa
Spaanse Nederlanden,Netherlands
Spanish Netherlands,Netherlands
Timor-Leste,East Timor
Utrecht Holland,Netherlands
VOC,South Africa
Weskus van Indi,India
Zeeland Netherlands,Netherlands
Zuid-Holland,Netherlands
Zuid Afrika,South Africa";

        Dictionary<string, string> continents = Regex.Split(@"Angola,Africa
Austria,Europe
Belgium,Europe
Burma,Asia
Channel Islands,Europe
Denmark,Europe
East Timor,Asia
England,Europe
Europe,Europe
Finland,Europe
France,Europe
Germany,Europe
Guinea,Africa
India,Asia
Indonesia,Asia
Italy,Europe
Kenya,Africa
Lithuania,Europe
Madagascar,Africa
Mauritius,Africa
Netherlands,Europe
Norway,Europe
Poland,Europe
Portugal,Europe
South Africa,South Africa
Spanish Netherlands,Europe
Switzerland,Europe
Scotland,Europe",
            "\r\n|\r|\n").ToDictionary(line => line.Split(',')[0], line => line.Split(',')[1]);

        ParseCorrections();

        var tree = GedcomParser.Load(file);
        var pointPerson = tree.Individus.Skip(1).FirstOrDefault(); // Etienne

        Console.OutputEncoding = Encoding.UTF8;
        Console.WriteLine("Position,Generation,Admixture %,Person ID,Year Of Birth,Sex,Country Of Birth,Continent,First name,Surname,Immigrant Status,Reason To Keep");

        foreach (var line in NavigateAncestry(pointPerson))
        {
            Console.WriteLine(line);
        }

        // local functions
        void ParseCorrections()
        {
            var lines = Regex.Split(countriesToReplace, Environment.NewLine);
            originalCountryNames = lines.Select(line => line.Split(",")[0]).ToArray();
            standardizedCountryNames = lines.Select(line => line.Split(",")[1]).ToArray();
        }

        IEnumerable<string?> NavigateAncestry(Individu? person, int generation = 1, int ahnentafelNumber = 1, string? countryOfChild = null)
        {
            if (IsLeaf())
            {
                if (person == null)
                    yield return $"{ahnentafelNumber},{generation},{AncestryPercent()},,,,,,,,,";
                else
                    yield return $"{ahnentafelNumber},{generation},{AncestryPercent()},{person.Id},{YearOfBirth()},{person.Sexe},{Country(person)},{Continent()},{Given()},{Surname()},{ImmigrantStatus()},{ReasonToKeep()}";
            }

            if (generation >= maxGenerations)
                yield break;

            if (!IsLeaf())
            {
                foreach (var line in NavigateAncestry(person?.ChildInFamilies?[0]?.Husband, generation + 1, ahnentafelNumber * 2, Country(person)))
                {
                    yield return line;
                }

                foreach (var line in NavigateAncestry(person?.ChildInFamilies?[0]?.Wife, generation + 1, ahnentafelNumber * 2 + 1, Country(person)))
                {
                    yield return line;
                }
            }

            // Local functions
            Event? Birth(Individu? child) => child?.Events?.Find(e => e.Type == "BIRT");

            string? Country(Individu? individual)
            {
                var location = Birth(individual)?.Location;
                if (location is null or "")
                    return countryOfChild;
                if (!location.Contains(","))
                    return CleanCountry(location);
                var places = location.Split(",").Reverse().Select(place => place.Trim()).ToArray();
                if (places[0] == "United Kingdom" && places.Length >= 2)
                    return CleanCountry(places.Skip(1).FirstOrDefault());
                return CleanCountry(places[0]);
            }

            string? Continent() => Country(person) == null || !continents.ContainsKey(Country(person!)!)
                ? null
                : continents[Country(person)!];

            double? AncestryPercent() => 1 / Math.Pow(2, generation - 1);

            string? YearOfBirth() => Regex.Match(Birth(person)?.Date ?? "", @"\b(\d\d\d\d)\b").Captures.FirstOrDefault()?.Value;

            string? Given() => person?.Names?.FirstOrDefault()?.Given;

            string? Surname() => person?.Names?.FirstOrDefault()?.Surname;

            bool IsProgenitor() => countryOfChild != null && countryOfChild != Country(person);

            string ImmigrantStatus() =>
                 generation > 1 && IsProgenitor()
                     ? "Progenitor"
                     : "";

            bool IsLeaf()
            {
                if (IsProgenitor())
                    return true;
                if (generation == maxGenerations)
                    return true;
                if (!HasFatherOrMother())
                    return true;
                return false;
            }

            string ReasonToKeep()
            {
                if (IsProgenitor())
                    return "Progenitor";
                if (generation == maxGenerations)
                    return "Reached generation limit";
                if (!HasFatherOrMother())
                    return "No parents";
                return "";
            }

            Family? Family(Individu? individual) => individual?.ChildInFamilies?.FirstOrDefault();

            Individu? Father(Individu? individual) => generation < maxGenerations ? Family(individual)?.Husband : null;

            Individu? Mother(Individu? individual) => generation < maxGenerations ? Family(individual)?.Wife : null;

            bool HasFatherOrMother() => Father(person) != null || Mother(person) != null;

            string? CleanCountry(string? country)
            {
                if (country is null or "")
                    return countryOfChild;
                country = country.Trim();
                if (country is "")
                    return null;
                for (var i = 0; i < originalCountryNames.Length; i++)
                {
                    if (country.StartsWith(originalCountryNames[i]))
                        return standardizedCountryNames[i];
                }
                return country;
            }
        }
    }
}
