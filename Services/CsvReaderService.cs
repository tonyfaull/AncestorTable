using Microsoft.VisualBasic.FileIO;

namespace AncestorTable.Services;

public class CsvReaderService
{
    public IEnumerable<string[]> ReadCsv(string fileName, int columnCount)
    {
        var csv = OpenCsv(fileName);
        while (!csv.EndOfData)
        {
            var values = csv.ReadFields();
            if (values == null || values.Length < columnCount)
            {
                throw new MalformedLineException($"Expected {columnCount} columns in {fileName}, found {values?.Length}");
            }

            yield return values;
        }
    }

    private TextFieldParser OpenCsv(string fileName)
    {
        var textFieldParser = new TextFieldParser(fileName)
        {
            Delimiters = new[] { "," },
            HasFieldsEnclosedInQuotes = true,
            TrimWhiteSpace = true
        };
        SkipTheHeaderRow();
        return textFieldParser;

        // Local functions
        void SkipTheHeaderRow() => _ = textFieldParser.ReadLine();
    }
}