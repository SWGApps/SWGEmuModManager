using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
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
            return (ulong)Directory.GetFiles(directory, "*", SearchOption.AllDirectories)
                .Sum(file => (new FileInfo(file).Length));
        }

        private static List<string> GetFileList(string directory)
        {
            List<string> fileList = new();

            List<string> files = Directory.EnumerateFiles(directory, "*.*", SearchOption.AllDirectories).ToList();

            files.ForEach(file =>
            {
                if (!file.Contains("modinfo.txt"))
                {
                    fileList.Add(file.Replace("\\", "/"));
                }
            });

            return fileList;
        }

        private static string GetModInfo(string[] modInfo, int index) => modInfo[index].Split("=")[1];

        internal static void GenerateModManifest(string modsDirectory)
        {
            // Return if user cancels
            if (string.IsNullOrEmpty(modsDirectory)) return;

            // Top level directories
            List<string> modDirectories = Directory.EnumerateDirectories(
                modsDirectory, "*.*", SearchOption.TopDirectoryOnly).ToList();

            List<Mod> mods = new();

            int i = 0;
            modDirectories.ForEach(directory =>
            {
                string[] modInfo = File.ReadAllLines($"{directory}/modinfo.txt");

                mods.Add(new Mod()
                {
                    Id = i,
                    Name = GetModName(modsDirectory, directory),
                    Description = GetModInfo(modInfo, 0),
                    Author = GetModInfo(modInfo, 1),
                    Version = GetModInfo(modInfo, 2),
                    Size = GetModSize(directory),
                    Downloads = 0,
                    Released = DateTime.Now,
                    Rating = 0,
                    FileList = GetFileList(directory),
                    ConflictList = new List<Mod>()
                });

                i++;
            });

            File.WriteAllText($"{Microsoft.VisualBasic.FileIO.SpecialDirectories.Desktop}/mods.json",
                JsonSerializer.Serialize(mods, new JsonSerializerOptions() { WriteIndented = true }));
        }
    }
}
