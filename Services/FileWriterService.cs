using System.Text;

namespace AncestorTable.Services;

internal class FileWriterService
{
    public void WriteFile(string fileName, IEnumerable<string> lines)
    {
        File.WriteAllLines(fileName, lines.ToArray(), Encoding.UTF8);
    }
}