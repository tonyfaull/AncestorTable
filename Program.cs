using AncestorTable.Services;

namespace AncestorTable;

internal static class Program
{
    public static void Main(params string[] args)
    {
        const string gedcomFileName = @"G:\My Drive\Home\Family Tree\GEDCOM files\Etienne 16gen 2022-10-25.ged";
        const string ancestorsFileName = "ancestors.csv";
        const int numberToSkip = 0;
        const int maxGenerations = 17;

        GedcomReaderService gedcomService = new();
        PedigreeTraversalService pedigreeTraversalService = new();
        FieldMapperService fieldMapperService = new();
        CsvWriterService csvWriterService = new();
        FileWriterService fileWriterService = new();

        var gedcom = gedcomService.LoadGedcomFile(gedcomFileName);
        var pointPerson = gedcomService.PointPerson(gedcom, numberToSkip);
        var progenitors = pedigreeTraversalService.Progenitors(pointPerson, maxGenerations);
        var cells = fieldMapperService.Cells(progenitors);
        var csvLines= csvWriterService.CsvLines(cells);
        fileWriterService.WriteFile(ancestorsFileName, csvLines);
    }
}