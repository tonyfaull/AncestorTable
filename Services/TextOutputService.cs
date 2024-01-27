using System.Text;

namespace AncestorTable.Services;

internal class TextOutputService
{
    public void GenerateOutput(string? file, IEnumerable<string> linesOfText)
    {
        if (file == null)
        {
            Console.OutputEncoding = Encoding.UTF8;
            linesOfText.ToList().ForEach(Console.WriteLine);
        }
        else
        {
            var fileWriterService = new FileWriterService();
            fileWriterService.WriteFile(file, linesOfText);
        }
    }
}