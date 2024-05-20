using System.CommandLine;
using System.Reflection;

namespace Metadate;

internal static class Program
{
    private static int Main(string[] args)
    {
        var rootCommand = new RootCommand("MetaDate - Date detector");
        var target = new Option<DirectoryInfo>(
            name: "--target",
            description: "The directory containing the images to analyze."){
            IsRequired = true };

        rootCommand.AddOption(target);

        rootCommand.SetHandler((targ) =>
        {
            if (InfosExtractor.Check(targ) is false)
                return;

            var infos = InfosExtractor.RetrieveInformation(targ);
            var outputPath = Path.Join(AppContext.BaseDirectory, "prediction.json");
            InfosExtractor.SaveOutputInformation(infos, new FileInfo(outputPath));
        }, target);

        return rootCommand.Invoke(args);
    }
}