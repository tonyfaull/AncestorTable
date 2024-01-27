namespace AncestorTable.Services;

internal class ProgenitorsService
{
    public void WriteProgenitorsCsv(string? yearOfBirth = null, string? gedFile = null, string? csvFile = null)
    {
        ConsoleReaderService consoleReaderService = new();
        GedcomReaderService gedcomService = new();
        PedigreeTraversalService pedigreeTraversalService = new();
        FieldMapperService fieldMapperService = new();
        CsvWriterService csvWriterService = new();
        TextOutputService textOutputService = new();

        if (IsConsoleInput())
        {
            gedFile = consoleReaderService.WriteTempFile()!;
        }

        try
        {
            var gedcom = gedcomService.LoadGedcomFile(gedFile!);
            var pointPerson = gedcomService.PointPerson(gedcom, yearOfBirth);
            var progenitors = pedigreeTraversalService.Progenitors(pointPerson);
            var cells = fieldMapperService.Cells(progenitors);
            var csvLines = csvWriterService.CsvLines(cells);
            textOutputService.GenerateOutput(csvFile, csvLines);
        }
        finally
        {
            if (IsConsoleInput())
            {
                File.Delete(gedFile!);
            }
        }

        return;

        // Local functions
        bool IsConsoleInput()
        {
            return gedFile is null or "";
        }
    }
}