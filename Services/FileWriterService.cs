namespace AncestorTable.Services;

internal class FileWriterService
{
    public void WriteFile(string fileName, IEnumerable<string> lines)
    {
        using var outputFile = new StreamWriter(fileName);
        foreach (var line in lines)
        {
            outputFile.WriteLine(line);
        }
    }
}