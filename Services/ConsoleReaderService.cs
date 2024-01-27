using System.Text;

namespace AncestorTable.Services;

internal class ConsoleReaderService
{
    public string WriteTempFile()
    {
        var tempFile = TempFile();
        File.WriteAllLines(tempFile, ReadAllLines(), Encoding.UTF8);
        return tempFile;

        string TempFile() => Path.Combine(Path.GetTempPath(), Path.ChangeExtension(Path.GetRandomFileName(), "ged"));

        static IEnumerable<string> ReadAllLines()
        {
            for (var line = Console.ReadLine();
                 line is not null and not "";
                 line = Console.ReadLine())
            {
                yield return line;
            }
        }
    }
}