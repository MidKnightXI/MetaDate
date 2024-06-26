using System.CommandLine;
using Metadate;

internal static class Program
{
    private static int Main(string[] args)
    {
        var rootCommand = new RootCommand("MetaDate - Date detector");

        var target = new Argument<DirectoryInfo>(
            name: "target",
            description: "The directory containing the images to analyze.")
        {
            Arity = ArgumentArity.ExactlyOne
        };

        var output = new Option<DirectoryInfo>(
            name: "--output",
            description: "The output file path for the prediction results",
            getDefaultValue: () => new DirectoryInfo("./"));

        var dateOption = new Option<string>(
            name: "--orderBy",
            description: "Specify the type of date information to process: 'day', 'month', or 'year'",
            getDefaultValue: () => "day");

        dateOption.AddValidator(result =>
        {
            var value = result.GetValueOrDefault<string>();
            if (value is not "day" && value is not "month" && value is not "year")
            {
                result.ErrorMessage = "The value for --date must be 'day', 'month', or 'year'.";
            }
        });

        rootCommand.AddArgument(target);
        rootCommand.AddOption(output);
        rootCommand.AddOption(dateOption);

        rootCommand.SetHandler((DirectoryInfo targ, DirectoryInfo outputPath, string date) =>
        {
            if (InfosExtractor.Check(targ) is false)
            {
                return;
            }
            var results = InfosExtractor.RetrieveInformation(targ);
            InfosExtractor.OrderPictures(results, date, outputPath);
        }, target, output, dateOption);

        return rootCommand.Invoke(args);
    }
}
