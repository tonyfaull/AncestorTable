using NanoGedcom;

namespace AncestorTable.Services;

internal class GedcomReaderService
{
    public Gedcom LoadGedcomFile(string fileName) => GedcomParser.Load(fileName)!;

    public Individu PointPerson(Gedcom gedcom, int skip = 0) => gedcom.Individus.Skip(skip).First();
}