using System;
using System.IO;
using System.IO.Compression;

namespace SWGEmuModManager.Util;

public static class ZipArchiveExtension
{
    public static Action<int, int>? OnInstallProgressUpdated { get; set; }
    public static Action? OnInstallDone { get; set; }

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
                throw new IOException("Trying to extract file outside of destination directory.");
            }

            if (file.Name == "")
            {
                Directory.CreateDirectory(Path.GetDirectoryName(completeFileName)!);
                continue;
            }

            OnInstallProgressUpdated?.Invoke(i, archive.Entries.Count);

            file.ExtractToFile(completeFileName, true);

            i++;
        }

        OnInstallDone?.Invoke();
    }
}

