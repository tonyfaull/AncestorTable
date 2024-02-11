namespace AncestorTable.Services;

internal class ProgenitorsService
{
    public void WriteProgenitorsCsv(string? gedFile = null, string? csvFile = null, string? descendantId = null)
    {
        ConsoleReaderService consoleReaderService = new();
        FamilySearchIdService familySearchIdService = new();
        GedcomReaderService gedcomReaderService = new();
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
            var gedcom = gedcomReaderService.LoadGedcomFile(gedFile!);
            var familySearchIds = familySearchIdService.FamilySearchIds(File.ReadAllText(gedFile!)).ToDictionary(item => item.personId, item => item.familySearchId);
            var descendant = gedcomReaderService.Descendant(gedcom, familySearchIds, descendantId);
            var progenitors = pedigreeTraversalService.Progenitors(descendant).ToArray();

            foreach (var ancestor in progenitors.Where(ancestor => ancestor.PersonId is not null && familySearchIds.ContainsKey(ancestor.PersonId)))
            {
                ancestor.FamilySearchId = familySearchIds[ancestor.PersonId!];
            }

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
        bool IsConsoleInput() => gedFile is null or "";
    }
}