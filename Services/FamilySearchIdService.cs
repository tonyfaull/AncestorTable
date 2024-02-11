using System.Text.RegularExpressions;

namespace AncestorTable.Services;

internal partial class FamilySearchIdService
{
    public IEnumerable<(string personId, string familySearchId)> FamilySearchIds(string gedText)
    {
        string? personId = null;
        var matches = FamilySearchIdRegex().Matches(gedText).ToArray();
        foreach (var matchGroups in matches.Select(match => match.Groups))
        {
            if (matchGroups[1].Value is not "")
            {
                personId = matchGroups[1].Value;
                continue;
            }

            var familySearchId = matchGroups[2].Value;
            if (familySearchId is not "")
            {
                yield return (personId!, familySearchId);
            }
        }
    }

    [GeneratedRegex(@"\r\n0 (.*) INDI|\r\n1 EVEN .*([A-Z\d]{4}-[A-Z\d]{3})\r\n")]
    private static partial Regex FamilySearchIdRegex();
}
