new AncestorTable.Services.ProgenitorsService().WriteProgenitorsCsv(descendantYearBorn: Year(), gedFile: Arg(1), csvFile: Arg(2));
return;

// Local functions
int? Year() => Arg(0) is not null ? Convert.ToInt32(Arg(0)) : null;

string? Arg(int index) => args.Skip(index).FirstOrDefault();
