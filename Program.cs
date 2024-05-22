using System.CommandLine;

namespace Metadate;

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

        rootCommand.AddArgument(target);

        rootCommand.SetHandler((targ) =>
        {
            if (InfosExtractor.Check(targ) is false)
            {
                return;
            }

            var infos = InfosExtractor.RetrieveInformation(targ);
            var outputPath = Path.Join(AppContext.BaseDirectory, "prediction.json");

            InfosExtractor.SaveOutputInformation(infos, new FileInfo(outputPath));
        }, target);

        return rootCommand.Invoke(args);
    }
}