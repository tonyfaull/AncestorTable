namespace AncestorTable.Services;

internal class CsvWriterService
{
    public IEnumerable<string> CsvLines(IEnumerable<string?[]> cells) => cells.Select(JoinWithCommas);

    private static string JoinWithCommas(IEnumerable<string?> values) => string.Join(",", values.Select(EscapeQuotesAndCommas));

    private static string? EscapeQuotesAndCommas(string? value)
    {
        if (value == null)
        {
            return null;
        }

        if (value.Contains('"'))
        {
            return "\"" + value.Replace("\"", "\"\"") + "\"";
        }

        if (value.Contains(','))
        {
            return "\"" + value + "\"";
        }

        return value;
    }
}