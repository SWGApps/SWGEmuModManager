using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text.Json;
using System.Threading.Tasks;
using SWGEmuModManager.ViewModels;

namespace SWGEmuModManager.Util;

internal class ManifestGenerator
{
    private static string GetModName(string modsDirectory, string directory)
    {
        string relativePath = Path.GetRelativePath(modsDirectory, directory);

        if (relativePath.Contains('_')) return relativePath.Replace("_", " ");
        if (relativePath.Contains('-')) return relativePath.Replace("-", " ");

        return relativePath;
    }

    private static ulong GetModSize(string directory)
    {
        File.Move(Path.Join(directory, "modinfo.txt"), 
            Path.Join(Directory.GetParent(directory)!.FullName, "modinfo.txt"));

        ulong size = (ulong)Directory.GetFiles(directory, "*", SearchOption.AllDirectories)
            .Sum(file => (new FileInfo(file).Length));

        File.Move(Path.Join(Directory.GetParent(directory)!.FullName, "modinfo.txt"),
            Path.Join(directory, "modinfo.txt"));

        return size;
    }

    private static List<string> GetFileList(string directory)
    {
        List<string> fileList = new();

        List<string> files = Directory.EnumerateFiles(directory, "*.*", SearchOption.AllDirectories).ToList();

        files.ForEach(file =>
        {
            if (!file.Contains("modinfo.txt"))
            {
                fileList.Add(Path.GetRelativePath(directory, file).Replace("\\", "/"));
            }
        });

        return fileList;
    }

    private static string GetModInfo(string[] modInfo, int index) => modInfo[index].Split("=")[1];



    private static async Task<bool> HasFileConflictAsync(string directory, string directory2)
    {
        bool hasConflict = false;

        IEnumerable<string> list1 = Directory.EnumerateFiles(path: directory, searchPattern: "*.*", SearchOption.AllDirectories)
            .Select(x => Path.GetRelativePath(relativeTo: directory, path: x))
            .Where(x => x != "modinfo.txt");

        IEnumerable<string> list2 = Directory.EnumerateFiles(path: directory2, searchPattern: "*.*", SearchOption.AllDirectories)
            .Select(x => Path.GetRelativePath(relativeTo: directory2, path: x))
            .Where(x => x != "modinfo.txt");

        foreach (string file in list1.Intersect(list2).ToList())
        {
            if (!await IsFileEqualAsync(file, directory, directory2))
            {
                hasConflict = true;
            }
        }

        return hasConflict;
    }

    private static async Task<bool> IsFileEqualAsync(string file, string directory1, string directory2)
    {
        await Task.Run(() =>
        {
            using MD5 md5 = MD5.Create();

            using FileStream stream1 = File.OpenRead(Path.Join(directory1, file));
            using FileStream stream2 = File.OpenRead(Path.Join(directory2, file));

            string hash1 = BitConverter.ToString(md5.ComputeHash(stream1)).Replace("-", "").ToLowerInvariant();
            string hash2 = BitConverter.ToString(md5.ComputeHash(stream2)).Replace("-", "").ToLowerInvariant();

            if (hash1 == hash2)
            {
                return true;
            }

            return false;
        });

        return false;
    }

    internal static async Task GenerateModManifestAsync(string modsDirectory)
    {
        // Return if user cancels
        if (string.IsNullOrEmpty(modsDirectory)) return;

        // Top level directories
        List<string> modDirectories = Directory.EnumerateDirectories(
            modsDirectory, "*.*", SearchOption.TopDirectoryOnly).ToList();

        List<MainWindowViewModelResponses.Mod> mods = new();

        int i = 0;
        foreach (string directory in modDirectories)
        {
            List<int> conflictList = new();

            int conflictCheckIter = 0;
            foreach (string conflictCheckDirectory in modDirectories)
            {
                if (directory != conflictCheckDirectory)
                {
                    if (await HasFileConflictAsync(directory, conflictCheckDirectory))
                    {
                        conflictList.Add(conflictCheckIter);
                    }
                }

                conflictCheckIter++;
            }

            string[] modInfo = await File.ReadAllLinesAsync($"{directory}/modinfo.txt");

            string modName = GetModName(modsDirectory, directory);

            mods.Add(new MainWindowViewModelResponses.Mod()
            {
                Id = i,
                Name = modName,
                BannerUrl = GetModInfo(modInfo, 0),
                Description = GetModInfo(modInfo, 1),
                Author = GetModInfo(modInfo, 2),
                Version = GetModInfo(modInfo, 3),
                Size = GetModSize(directory),
                Source = GetModInfo(modInfo, 4),
                Downloads = 0,
                Released = DateTime.Now,
                Archive = await ZipArchiveExtension.CreateZipFileAsync(directory, modName),
                FileList = GetFileList(directory),
                ConflictList = conflictList
            });

            i++;
        }

        await File.WriteAllTextAsync(path: $"{Microsoft.VisualBasic.FileIO.SpecialDirectories.Desktop}/mods.json",
            contents: JsonSerializer.Serialize(mods, new JsonSerializerOptions() { WriteIndented = true }));
    }
}
