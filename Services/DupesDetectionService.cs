namespace AncestorTable.Services;

internal class DupesDetectionService
{
    public Dictionary<string, int> Dupes { get; } = new();

    public void IncrementCounter(string personId)
    {
        Dupes[personId] = Dupes.TryGetValue(personId, out var value) ? value + 1 : 1;
    }
}