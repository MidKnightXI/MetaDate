using System.Text.Json;
using MetadataExtractor;
using MetadataExtractor.Formats.Exif;

using Metadate.Model;
using Metadate.Model.JsonContext;

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
                var cameraInformation = GetCameraInformation(directories);

                outputInformation.Add(new ResultModel()
                {
                    Success = cameraInformation is not null,
                    Path = path,
                });
            }
            catch (Exception ex)
            {
                outputInformation.Add(new ResultModel()
                {
                    Success = false,
                    Path = path,
                    Error = ex.Message
                });
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

        var picturesPaths = targetDirectory.GetFiles()
            .Any(f => _validImageFormat.Contains(f.Extension.ToLower()));

        if (picturesPaths is false)
        {
            Console.Error.WriteLine("Directory {0} has no valid files", targetDirectory.FullName);
            return false;
        }

        return true;
    }

    private static DateResult? GetCameraInformation(IReadOnlyList<MetadataExtractor.Directory>? directories)
    {
        if (directories is null)
        {
            return null;
        }

        var ifd0Directory = directories.OfType<ExifIfd0Directory>().FirstOrDefault();

        if (ifd0Directory is not null)
        {
            var date = ifd0Directory.GetDateTime(ExifDirectoryBase.TagDateTime);
            return new DateResult() { Date = date.ToString() };
        }
        else
        {
            return null;
        }
    }

    public static void SaveOutputInformation(List<ResultModel> outputInformation, FileInfo outputFile)
    {
        using var stream = outputFile.CreateText();
        var json = JsonSerializer.Serialize(outputInformation, Context.Default.ListResultModel);
        stream.Write(json);
    }
}
