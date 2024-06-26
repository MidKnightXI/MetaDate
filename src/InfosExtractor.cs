using System.Globalization;
using MetadataExtractor;
using MetadataExtractor.Formats.Exif;

using Metadate.Model;

namespace Metadate;

public static class InfosExtractor
{
    private static readonly string[] _validImageFormat =
    [
        ".jpg", ".png", ".gif", ".tiff", ".cr2", ".nef", ".arw", ".dng", ".raf",
        ".rw2", ".erf", ".nrw", ".crw", ".3fr", ".sr2", ".k25", ".kc2", ".mef",
        ".cs1", ".orf", ".mos", ".kdc", ".cr3", ".ari", ".srf", ".srw", ".j6i",
        ".fff", ".mrw", ".x3f", ".mdc", ".rwl", ".pef", ".iiq", ".cxi", ".nksc",
    ];

    public static List<ResultModel> RetrieveInformation(DirectoryInfo targetDirectory)
    {
        var outputInformation = new List<ResultModel>();
        var picturesPaths = targetDirectory.GetFiles()
            .Where(f => _validImageFormat.Contains(f.Extension.ToLower()))
            .Select(f => f.FullName);

        foreach (var path in picturesPaths)
        {
            try
            {
                using var stream = File.OpenRead(path);
                var directories = ImageMetadataReader.ReadMetadata(stream);
                var dateInformation = GetDateInformation(directories);

                if (dateInformation is null)
                {
                    Console.Error.WriteLine($"{path} does not have any dates in its metadatas.");
                    continue;
                }

                outputInformation.Add(new ResultModel()
                {
                    Path = path,
                    Date = dateInformation,
                });
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
            }
        }

        return outputInformation;
    }

    public static bool Check(DirectoryInfo targetDirectory)
    {
        if (targetDirectory.Exists is false)
        {
            Console.Error.WriteLine("Directory {0} does not exist", targetDirectory.FullName);
            return false;
        }
        var picturesPaths = targetDirectory.GetFiles().Any(f => _validImageFormat.Contains(f.Extension.ToLower()));
        if (picturesPaths is false)
        {
            Console.Error.WriteLine("Directory {0} has no valid files", targetDirectory.FullName);
            return false;
        }
        return true;
    }

    private static string? GetDateInformation(IReadOnlyList<MetadataExtractor.Directory>? directories)
    {
        if (directories is null)
        {
            return null;
        }
        var ifd0Directory = directories.OfType<ExifIfd0Directory>().FirstOrDefault();
        if (ifd0Directory is not null)
        {
            var date = ifd0Directory.GetDateTime(ExifDirectoryBase.TagDateTime);
            return date.ToString("yyyy-MM-dd");
        }
        return null;
    }

    public static void OrderPictures(List<ResultModel> results, string datePart, DirectoryInfo outputDirectory)
    {
        if (outputDirectory.Exists is false)
        {
            outputDirectory.Create();
        }

        foreach (var result in results)
        {
            string dirName = datePart switch
            {
                "day" => result.Date[..10],
                "month" => result.Date[..7],
                "year" => result.Date[..4],
                _ => throw new ArgumentException("The value for datePart must be 'day', 'month', or 'year'."),
            };

            var dateDir = Path.Combine(outputDirectory.FullName, dirName);
            if (!System.IO.Directory.Exists(dateDir))
            {
                System.IO.Directory.CreateDirectory(dateDir);
            }

            var fileName = Path.GetFileName(result.Path);
            var destinationPath = Path.Combine(dateDir, fileName);
            File.Copy(result.Path, destinationPath, overwrite: true);
        }
    }
}
