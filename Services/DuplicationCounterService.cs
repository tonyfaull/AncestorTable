namespace AncestorTable.Services;

internal class DuplicationCounterService
{
    public Dictionary<string, int> Counter { get; } = new();

    public void IncrementCounter(string personId)
    {
        Counter[personId] = Counter.TryGetValue(personId, out var value) ? value + 1 : 1;
    }
}