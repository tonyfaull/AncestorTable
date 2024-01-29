﻿namespace AncestorTable.Services;

internal class ProgenitorsService
{
    public void WriteProgenitorsCsv(int? descendantYearBorn = null, string? gedFile = null, string? csvFile = null)
    {
        ConsoleReaderService consoleReaderService = new();
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
            var descendant = gedcomReaderService.Descendant(gedcom, descendantYearBorn);
            var progenitors = pedigreeTraversalService.Progenitors(descendant);
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