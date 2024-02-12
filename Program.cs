if (args.Length == 0)
{
    Console.WriteLine(
        $"""
           Converts a pedigree into a spreadsheet containing the foreign-born ancestors and their admixture percent

           Syntax:
               {AppDomain.CurrentDomain.FriendlyName} [.ged file] [.csv file] [year of birth | FamilySearch ID | GED person ID || full name]

           e.g. {AppDomain.CurrentDomain.FriendlyName} "StaticData/ehf-16gen.ged" "progenitors.csv" 2013

           """);
    return;
}

new AncestorTable.Services.ProgenitorsService().WriteProgenitorsCsv(
    gedFile: args[0],
    csvFile: args.Skip(1).FirstOrDefault() ?? "progenitors.csv",
    descendantId: args.Skip(2).FirstOrDefault());
