using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using SWGEmuModManager.ViewModels;

namespace SWGEmuModManager.Util
{
    internal class ManifestGenerator
    {
        private static string GetModName(string modsDirectory, string directory)
        {
            string relativePath = Path.GetRelativePath(modsDirectory, directory);

            string modName = "";

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

        private static async Task<string> CreateZipFile(string directory, string modName)
        {
            string archiveName = $"{Path.GetDirectoryName(directory)!}/{modName.Replace(" ", "")}.zip";

            File.Move(Path.Join(directory, "modinfo.txt"), Path.Join(Directory.GetParent(directory)!.FullName, "modinfo.txt"));
            await Task.Run(() => ZipFile.CreateFromDirectory(directory, archiveName));
            File.Move(Path.Join(Directory.GetParent(directory)!.FullName, "modinfo.txt"), Path.Join(directory, "modinfo.txt"));

            return Path.GetFileName(archiveName);
        }

        internal static async Task GenerateModManifest(string modsDirectory)
        {
            // Return if user cancels
            if (string.IsNullOrEmpty(modsDirectory)) return;

            // Top level directories
            List<string> modDirectories = Directory.EnumerateDirectories(
                modsDirectory, "*.*", SearchOption.TopDirectoryOnly).ToList();

            List<Mod> mods = new();

            int i = 0;
            foreach (string directory in modDirectories)
            {
                string[] modInfo = await File.ReadAllLinesAsync($"{directory}/modinfo.txt");

                string modName = GetModName(modsDirectory, directory);

                mods.Add(new Mod()
                {
                    Id = i,
                    Name = modName,
                    BannerUrl = GetModInfo(modInfo, 0),
                    Description = GetModInfo(modInfo, 1),
                    Author = GetModInfo(modInfo, 2),
                    Version = GetModInfo(modInfo, 3),
                    Size = GetModSize(directory),
                    Downloads = 0,
                    Released = DateTime.Now,
                    Archive = await CreateZipFile(directory, modName),
                    FileList = GetFileList(directory),
                    ConflictList = new List<int>()
                });

                i++;
            }

            await File.WriteAllTextAsync($"{Microsoft.VisualBasic.FileIO.SpecialDirectories.Desktop}/mods.json",
                JsonSerializer.Serialize(mods, new JsonSerializerOptions() { WriteIndented = true }));
        }
    }
}
