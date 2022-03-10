using System;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using SWGEmuModManager.Models;

namespace SWGEmuModManager.Util;

public static class ZipArchiveExtension
{
    public static Action? OnInstallStarted { get; set; }
    public static Action<int, int>? OnInstallProgressUpdated { get; set; }
    public static Action? OnInstallDone { get; set; }

    public static async Task<string> CreateZipFileAsync(string directory, string modName)
    {
        string archiveName = $"{Path.GetDirectoryName(directory)!}/{modName.Replace(oldValue: " ", newValue: "")}.zip";

        File.Move(sourceFileName: Path.Join(directory, "modinfo.txt"),
            destFileName: Path.Join(Directory.GetParent(directory)!.FullName, "modinfo.txt"));

        if (File.Exists(archiveName)) File.Delete(archiveName);

        await Task.Run(() => ZipFile.CreateFromDirectory(directory, archiveName));

        File.Move(sourceFileName: Path.Join(Directory.GetParent(directory)!.FullName, "modinfo.txt"),
            destFileName: Path.Join(directory, "modinfo.txt"));

        return Path.GetFileName(archiveName);
    }

    public static async Task UnzipModAsync(int modId, string archiveName)
    {
        await Task.Run(() =>
        {
            ConfigFile? config = ConfigFile.GetConfig();

            if (config is not null && !string.IsNullOrEmpty(config.SwgDirectory))
            {
                OnInstallStarted?.Invoke();

                using FileStream stream = File.OpenRead(Path.Join(config.SwgDirectory, archiveName));

                ExtractToDirectory(archive: new ZipArchive(stream), config.SwgDirectory, overwrite: true);

                stream.Dispose();

                File.Delete(Path.Join(config.SwgDirectory, archiveName));
            }

            config!.InstalledMods!.Add(modId);

            ConfigFile.SetConfig(config);
        });
    }

    public static void ExtractToDirectory(this ZipArchive archive, string destinationDirectoryName, bool overwrite)
    {
        if (!overwrite)
        {
            archive.ExtractToDirectory(destinationDirectoryName);
            return;
        }

        DirectoryInfo di = Directory.CreateDirectory(destinationDirectoryName);
        string destinationDirectoryFullPath = di.FullName;

        int i = 1;
        foreach (ZipArchiveEntry file in archive.Entries)
        {
            string completeFileName = Path.GetFullPath(Path.Combine(destinationDirectoryFullPath, file.FullName));

            if (!completeFileName.StartsWith(destinationDirectoryFullPath, StringComparison.OrdinalIgnoreCase))
            {
                throw new IOException(message: "Trying to extract file outside of destination directory.");
            }

            if (file.Name == "")
            {
                Directory.CreateDirectory(Path.GetDirectoryName(completeFileName)!);
                continue;
            }

            OnInstallProgressUpdated?.Invoke(i, archive.Entries.Count);

            if (!Directory.Exists(Path.GetDirectoryName(completeFileName)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(completeFileName)!);
            }

            file.ExtractToFile(completeFileName, overwrite: true);

            i++;
        }

        OnInstallDone?.Invoke();
    }
}

