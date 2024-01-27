new AncestorTable.Services.ProgenitorsService().WriteProgenitorsCsv(yearOfBirth: Arg(0), gedFile: Arg(1), csvFile: Arg(2));

// Local functions
string? Arg(int index) => args.Skip(index).FirstOrDefault();
